using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OmegaRace.Data_Queues;
using Lidgren.Network;
using OmegaRace.Managers.NetworkManager;
using System.IO;
using static OmegaRace.MessageQueueManager;
using System.Windows.Interop;
using OmegaRace.Managers.MessageManager;
using OmegaRace.Game_Scene_and_Mgt.Modes;
using Box2DX.Common;
using OmegaRace.Managers.Player;
using System.Reflection;

namespace OmegaRace
{
    public class GameScenePlay : IGameScene
    {
        public PlayerManager PlayerMgr { get; }
        public MessageQueueManager MsgQueueMgr { get; private set; }

        DisplayManager DisplayMgr;

        public NetworkManager NetworkMgr;

        public MessageManager MsgMgr;
        public PlaybackMode mPlayback;
        public RecordMode mRecord;

        public PredictionData prediction;
        public GameScenePlay()
        {
            PlayerMgr = new PlayerManager();
            MsgQueueMgr = new MessageQueueManager(new NormalMode());
            //mRecord = new RecordMode();
            //mRecord.SetupMode();
            //MsgQueueMgr = new MessageQueueManager(mRecord);
            //mPlayback = new PlaybackMode();
            //mPlayback.SetupMode();
            //MsgQueueMgr = new MessageQueueManager(mPlayback);
            DisplayMgr = new DisplayManager();
            NetworkMgr = new NetworkManager("localhost", 14240);
            MsgMgr = new MessageManager(10);

            prediction = new PredictionData();
        }

        void IGameScene.Enter()
        {
            LoadLevel();
        }

        public void MessageFromServer(NetIncomingMessage im)
        {
            // if data, read bytes from incoming message.
            byte[] msg = im.ReadBytes(im.LengthBytes);

            // create binary reader.
            BinaryReader reader = new BinaryReader(new MemoryStream(msg));

            // create mixed message instance.
            Message dataMsg = new Message();

            // deserialize data.
            dataMsg.Deserialize(ref reader);

            QueueMessage queueMsg = new QueueMessage();
            queueMsg.msg = dataMsg;

            // add to output queue to process.
            MsgQueueMgr.AddToOutputQueue(queueMsg);
        }

        public void MessageToServer(Message m)
        {
            QueueMessage queueMsg = new QueueMessage();
            queueMsg.msg = m;
           // MsgQueueMgr.AddToOutputQueue(queueMsg);

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            m.Serialize(ref writer);

            NetworkMgr.SendMessage(stream.ToArray(), m.deliveryMethod, m.sequenceChannel);
        }

        public void MessageToClient(Message m)
        {
            QueueMessage queueMsg = new QueueMessage();
            queueMsg.msg = m;
            MsgQueueMgr.AddToInputQueue(queueMsg);
        }
        void IGameScene.Update()
        { 
            // First, update the physics engine
            // PhysicWorld.Update();

            //Queue processing goes here
            MsgQueueMgr.Process();

            //Network processing
            NetworkMgr.ProcessIncoming(this);

            int p1_H = 0;
            int p1_V = 0;

            //Playback from file
            if (MsgQueueMgr.mode.type == GameMode.modeType.PLAYBACK)
            {
                mPlayback.ReadFromFile();

                if (!mPlayback.playbackFinished)
                {
                    PhysicWorld.Update();
                }
            }
            else 
            {
                // Process reactions to inputs for Player 1
                p1_H = InputManager.GetAxis(INPUTAXIS.HORIZONTAL_P1);
                p1_V = InputManager.GetAxis(INPUTAXIS.VERTICAL_P1);
                //PlayerMgr.P1Data.ship.Rotate(p1_H);
                //PlayerMgr.P1Data.ship.Move(p1_V);

                //only generate message if player moves
                if (p1_H != 0 || p1_V != 0)
                {
                    PlayerMovementMessage msg1 = MsgMgr.GetPlayerMovementMessage();
                    msg1.playerNum = 1;
                    msg1.horzInput = p1_H;
                    msg1.vertInput = p1_V;

                    Message sendMsg = new Message();
                    sendMsg.PopulateMessage(msg1);
                    MessageToServer(sendMsg);

                    MsgMgr.ReleasePlayerMovementMessage(msg1);
                }

                //limit of 3 missiles
                if (InputManager.GetButtonDown(INPUTBUTTON.P1_FIRE) && (PlayerMgr.P1Data.missileCount > 0))
                {
                    MissileEvent msg2F = MsgMgr.GetMissileEvent();
                    msg2F.playerNum = 1;
                    Message sendMsgF = new Message();
                    sendMsgF.PopulateMessage(msg2F);
                    MessageToServer(sendMsgF);
                    MsgMgr.ReleaseMissileEvent(msg2F);
                }

                //limit of 5 mines
                if (InputManager.GetButtonDown(INPUTBUTTON.P1_LAYMINE) && (PlayerMgr.P1Data.mineCount > 0))
                {
                    MineEvent msg1M = MsgMgr.GetMineEvent();
                    msg1M.playerNum = 1;
                    Message sendMsgM = new Message();
                    sendMsgM.PopulateMessage(msg1M);
                    MessageToServer(sendMsgM);
                    MsgMgr.ReleaseMineEvent(msg1M);
                }

            }

            //list of active missiles
            if (ActiveMissileList.missileFired)
            {
                foreach (GameObject obj in GameManager.getInstance().getObjList())
                {
                    if (obj.type == GAMEOBJECT_TYPE.MISSILE && !ActiveMissileList.activeMissiles.Contains((Missile)obj))
                    {
                        // add missile to active missile list.
                        ActiveMissileList.activeMissiles.Add((Missile)obj);
                    }
                }

                // set to false.
                ActiveMissileList.missileFired = false;
            }

            //Update position of ships
            Vec2 predictedPosition = prediction.PredictPos(PlayerMgr.P1Data.ship);

            PlayerMgr.P1Data.ship.SetPosAndAngle(predictedPosition.X, predictedPosition.Y, PlayerMgr.P1Data.ship.GetAngle_Deg());

            predictedPosition = prediction.PredictPos(PlayerMgr.P2Data.ship);

            PlayerMgr.P2Data.ship.SetPosAndAngle(predictedPosition.X, predictedPosition.Y, PlayerMgr.P2Data.ship.GetAngle_Deg());

            //Update position of missile
            if (ActiveMissileList.activeMissiles.Count > 0)
            {
                // iterate all active missiles.
                //UpdateMissileMessage fire1 = MsgMgr.GetUpdateMissileMessage();
                foreach (Missile activeMissile in ActiveMissileList.activeMissiles)
                {


                    //get predicted position
                    predictedPosition = prediction.PredictPos(activeMissile);

                    activeMissile.SetPosAndAngle(predictedPosition.X, predictedPosition.Y, activeMissile.missileAngle);
                }


            }

        }
        void IGameScene.Draw()
        {
            DisplayMgr.DisplayHUD(PlayerMgr.P1Data, PlayerMgr.P2Data);
        }
        void IGameScene.Leave()
        {
            
        }

        void LoadLevel()
        {
            GameManager.AddGameObject(PlayerMgr.P1Data.ship);
            GameManager.AddGameObject(PlayerMgr.P2Data.ship);

            // Fence OutsideBox

            GameManager.AddGameObject(new Fence(new Azul.Rect(100, 5, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 5, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 5, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(700, 5, 8, 200), 90));
  
            GameManager.AddGameObject(new Fence(new Azul.Rect(100, 495, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 495, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 495, 8, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(700, 495, 8, 200), 90));

            GameManager.AddGameObject(new Fence(new Azul.Rect(5, 125, 8, 250), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(5, 375, 8, 250), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(795, 125, 8, 250), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(795, 375, 8, 250), 0));

            // Fence InsideBox
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 170, 10, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 170, 10, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(300, 330, 10, 200), 90));
            GameManager.AddGameObject(new Fence(new Azul.Rect(500, 330, 10, 200), 90));

            GameManager.AddGameObject(new Fence(new Azul.Rect(200, 250, 10, 160), 0));
            GameManager.AddGameObject(new Fence(new Azul.Rect(600, 250, 10, 160), 0));


            // OutsideBox
            GameManager.AddGameObject(new FencePost(new Azul.Rect(5, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 5, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(800 - 5, 5, 10, 10)));

            GameManager.AddGameObject(new FencePost(new Azul.Rect(0 + 5, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 495, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(800 - 5, 495, 10, 10)));

            GameManager.AddGameObject(new FencePost(new Azul.Rect(5, 250, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(795, 250, 10, 10)));

            // InsideBox

            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 170, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 170, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 170, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(200, 330, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(400, 330, 10, 10)));
            GameManager.AddGameObject(new FencePost(new Azul.Rect(600, 330, 10, 10)));
        }


    }
}
