using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OmegaRace.Data_Queues;
using Lidgren.Network;
using System.IO;
using static OmegaRace.MessageQueueManager;
using System.Windows.Interop;
using AzulNetworkBase;
using Box2DX.Common;
using OmegaRace.Managers.MessageManager;
using OmegaRace.Game_Scene_and_Mgt.Modes;
using OmegaRace.Managers.Player;

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

        public ServerPredictionControl prediction;
        public float pThreshold = 1.0f;
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
            NetworkMgr = new NetworkManager(14240);
            MsgMgr = new MessageManager(10);
            prediction = new ServerPredictionControl();

        }

        void IGameScene.Enter()
        {
            LoadLevel();
        }

        public void MessageFromClient(NetIncomingMessage im)
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
            MsgQueueMgr.AddToOutputQueue(queueMsg);

            //MemoryStream stream = new MemoryStream();
            //BinaryWriter writer = new BinaryWriter(stream);
            //m.Serialize(ref writer);

            //NetworkMgr.SendMessage(stream.ToArray(), m.deliveryMethod, m.sequenceChannel);
        }

        public void MessageToClient(Message m)
        {
            QueueMessage queueMsg = new QueueMessage();
            queueMsg.msg = m;
            //MsgQueueMgr.AddToOutputQueue(queueMsg);

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            m.Serialize(ref writer);

            NetworkMgr.SendMessage(stream.ToArray(), m.deliveryMethod, m.sequenceChannel);
        }
        void IGameScene.Update()
        {
            // First, update the physics engine
            if (MsgQueueMgr.mode.type != GameMode.modeType.PLAYBACK)
            {
                PhysicWorld.Update();
            }

            //Queue processing goes here
            MsgQueueMgr.Process();

            //Network processing
            NetworkMgr.ProcessIncoming(this);

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
                // Process reactions to inputs for Player 2
                int p2_H = InputManager.GetAxis(INPUTAXIS.HORIZONTAL_P2);
                int p2_V = InputManager.GetAxis(INPUTAXIS.VERTICAL_P2);
                //* Direct call version
                // PlayerMgr.P2Data.ship.Rotate(p2_H);
                //PlayerMgr.P2Data.ship.Move(p2_V);
                //*/
                //Data driven example for P2 movement
                //only generate message if player moves
                if (p2_H != 0 || p2_V != 0)
                {
                    PlayerMovementMessage msg2 = MsgMgr.GetPlayerMovementMessage();
                    msg2.playerNum = 2;
                    msg2.horzInput = p2_H;
                    msg2.vertInput = p2_V;
                    Message sendMsg = new Message();
                    sendMsg.PopulateMessage(msg2);
                    MessageToServer(sendMsg);

                    MsgMgr.ReleasePlayerMovementMessage(msg2);
                }


                //limit of 3 missiles
                if (InputManager.GetButtonDown(INPUTBUTTON.P2_FIRE) && (PlayerMgr.P2Data.missileCount > 0))
                {
                    MissileEvent msg2F = MsgMgr.GetMissileEvent();
                    msg2F.playerNum = 2;
                    Message sendMsgF = new Message();
                    sendMsgF.PopulateMessage(msg2F);
                    MessageToServer(sendMsgF);
                    MsgMgr.ReleaseMissileEvent(msg2F);
                }


                //limit of 5 mines
                if (InputManager.GetButtonDown(INPUTBUTTON.P2_LAYMINE) && (PlayerMgr.P2Data.mineCount > 0))
                {
                    MineEvent msg2M = MsgMgr.GetMineEvent();
                    msg2M.playerNum = 2;
                    Message sendMsgM = new Message();
                    sendMsgM.PopulateMessage(msg2M);
                    MessageToServer(sendMsgM);
                    MsgMgr.ReleaseMineEvent(msg2M);
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

            //list of active mines
            if (ActiveMineList.mineDropped)
            {
                foreach (GameObject obj in GameManager.getInstance().getObjList())
                {
                    if (obj.type == GAMEOBJECT_TYPE.MINE && !ActiveMineList.activeMines.Contains((Mine)obj))
                    {
                        // add missile to active missile list.
                        ActiveMineList.activeMines.Add((Mine)obj);
                    }
                }

                // set to false.
                ActiveMineList.mineDropped = false;
            }

                //Send ship information to client
                if ((PlayerMgr.P1Data.ship.GetPixelVelocity() != Vec2.Zero) || (PlayerMgr.P2Data.ship.GetPixelVelocity() != Vec2.Zero))
                {
                    //get predicted position
                    Vec2 predictedPosition = prediction.PredictPos(PlayerMgr.P1Data.ship);

                    //get actual position
                    Vec2 positionFromClient = PlayerMgr.P1Data.ship.GetPixelPosition();

                    //if the actual position is outside of the threshold, adjust the prediction and send to client
                    if (Vec2.Distance(positionFromClient, predictedPosition) > pThreshold)
                    {
                        //player1
                        //PlayerMgr.P1Data.ship.predictedPos = predictedPosition; //store what is sent to client
                        //PlayerMgr.P1Data.ship.SetPosAndAngle(predictedPosition.X, predictedPosition.Y, PlayerMgr.P1Data.ship.GetAngle_Deg());
                        UpdatePlayerMovementMessage move1 = MsgMgr.GetUpdatePlayerMovementMessage();
                        move1.Set(PlayerMgr.P1Data.ship);
                        move1.playerNum = 1;

                        Message moveMsg1 = new Message();
                        moveMsg1.PopulateMessage(move1);
                        MessageToClient(moveMsg1);

                        MsgMgr.ReleaseUpdatePlayerMovementMessage(move1);

                        prediction.Update(PlayerMgr.P1Data.ship);
                    }

                    //get predicted position
                    predictedPosition = prediction.PredictPos(PlayerMgr.P2Data.ship);

                    //get actual position
                    positionFromClient = PlayerMgr.P2Data.ship.GetPixelPosition();

                    //if the actual position is outside of the threshold, adjust the prediction and send to client
                    if (Vec2.Distance(positionFromClient, predictedPosition) > pThreshold)
                    {
                        //player1
                        //PlayerMgr.P1Data.ship.predictedPos = predictedPosition; //store what is sent to client
                        //PlayerMgr.P1Data.ship.SetPosAndAngle(predictedPosition.X, predictedPosition.Y, PlayerMgr.P1Data.ship.GetAngle_Deg());
                        UpdatePlayerMovementMessage move2 = MsgMgr.GetUpdatePlayerMovementMessage();
                        move2.Set(PlayerMgr.P2Data.ship);
                        move2.playerNum = 2;

                        Message moveMsg2 = new Message();
                        moveMsg2.PopulateMessage(move2);
                        MessageToClient(moveMsg2);

                        MsgMgr.ReleaseUpdatePlayerMovementMessage(move2);

                        prediction.Update(PlayerMgr.P2Data.ship);
                    }
                }


                if (ActiveMissileList.missileFired)
            {
                // search the game object list to find new missiles
                foreach (GameObject obj in GameManager.getInstance().getObjList())
                {
                    // if object is a missile object and if missile list doesn't already contains the object.
                    if (obj.type == GAMEOBJECT_TYPE.MISSILE && !ActiveMissileList.activeMissiles.Contains((Missile)obj))
                    {
                        //  add missile to active missile list.
                        ActiveMissileList.activeMissiles.Add((Missile)obj);
                    }
                }

                // set to false.
                ActiveMissileList.missileFired = false;
            }

            //Send missile information to client
            if (ActiveMissileList.activeMissiles.Count > 0)
            {
                // iterate all active missiles.
                UpdateMissileMessage fire1 = MsgMgr.GetUpdateMissileMessage();
                foreach (Missile activeMissile in ActiveMissileList.activeMissiles)
                {


                        //get predicted position
                        Vec2 predictedPosition = prediction.PredictPos(activeMissile);

                        //get actual position
                        Vec2 positionFromClient = activeMissile.GetPixelPosition();

                        float value = Vec2.Distance(positionFromClient, predictedPosition);

                        //if the actual position is outside of the threshold, adjust the prediction and send to client
                        if (Vec2.Distance(positionFromClient, predictedPosition) > pThreshold)
                        {
                            // add the data for each active missile.
                            fire1.Set(activeMissile);

                            Message fireMsg = new Message();
                            fireMsg.PopulateMessage(fire1);
                            MessageToClient(fireMsg);

                            MsgMgr.ReleaseUpdateMissileMessage(fire1);

                            prediction.Update(activeMissile);
                        }
                    }


            }

            //list of active mines
            if (ActiveMineList.mineDropped)
            {
                foreach (GameObject obj in GameManager.getInstance().getObjList())
                {
                    if (obj.type == GAMEOBJECT_TYPE.MINE && !ActiveMineList.activeMines.Contains((Mine)obj))
                    {
                        // add missile to active missile list.
                        ActiveMineList.activeMines.Add((Mine)obj);
                    }
                }

                // set to false.
                ActiveMineList.mineDropped = false;
            }

            //Send mine information to client
            if (ActiveMineList.activeMines.Count > 0)
            {
                // iterate all active mines.
                UpdateMineMessage drop1 = MsgMgr.GetUpdateMineMessage();
                foreach (Mine activeMine in ActiveMineList.activeMines)
                {
                    // add the data for each active missile.
                    drop1.Set(activeMine);
                }

                Message dropMsg = new Message();
                dropMsg.PopulateMessage(drop1);
                MessageToClient(dropMsg);
                MsgMgr.ReleaseUpdateMineMessage(drop1);
            }

            //Send collisions information to client

            // send collision list only if there was a new collision event
            if (CollisionEvent.collisions.Count > 0)
            {
                // send collision list
                CollisionsList msgList = new CollisionsList(CollisionEvent.collisions);
                Message colMsg = new Message();
                colMsg.PopulateMessage(msgList);
                MessageToServer(colMsg);
                MessageToClient(colMsg);

                // clear list for next set of collisions.
                CollisionEvent.collisions.Clear();
            }

            /* Screen log example
            ScreenLog.Add(string.Format("Frame Time: {0:0.0}", 1 / TimeManager.GetFrameTime()));
            ScreenLog.Add(Colors.DarkKhaki, string.Format("P1 ammo: {0}", PlayerMgr.P1Data.missileCount));
            ScreenLog.Add(Colors.Orchid, string.Format("P2 ammo: {0}", PlayerMgr.P2Data.missileCount));
            //*/
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
