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

namespace OmegaRace
{
    public class GameScenePlay : IGameScene
    {
        public PlayerManager PlayerMgr { get; }
        public MessageQueueManager MsgQueueMgr { get; private set; }

        DisplayManager DisplayMgr;

        NetworkManager NetworkMgr;

        public GameScenePlay()
        {
            PlayerMgr = new PlayerManager();
            MsgQueueMgr = new MessageQueueManager();
            DisplayMgr = new DisplayManager();
            NetworkMgr = new NetworkManager(14240);
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

        public void MessageToClient(Message m)
        {
            QueueMessage queueMsg = new QueueMessage();
            queueMsg.msg = m;
            MsgQueueMgr.AddToOutputQueue(queueMsg);

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            m.Serialize(ref writer);

            NetworkMgr.SendMessage(stream.ToArray());
        }
        void IGameScene.Update()
        { 
            // First, update the physics engine
            PhysicWorld.Update();

            //Queue processing goes here
            MsgQueueMgr.Process();

            //Network processing
            NetworkMgr.ProcessIncoming(this);

            // Process reactions to inputs for Player 2
            int p2_H = InputManager.GetAxis(INPUTAXIS.HORIZONTAL_P2);
            int p2_V = InputManager.GetAxis(INPUTAXIS.VERTICAL_P2);
            //* Direct call version
           // PlayerMgr.P2Data.ship.Rotate(p2_H);
            //PlayerMgr.P2Data.ship.Move(p2_V);
            //*/
            //Data driven example for P2 movement
            PlayerMovementMessage msg2 = new PlayerMovementMessage();
            msg2.playerNum = 2;
            msg2.horzInput = p2_H;
            msg2.vertInput = p2_V;
            Message sendMsg = new Message();
            sendMsg.populateMessage(msg2);
            MessageToClient(sendMsg);

            FireMessage msg2F = new FireMessage();
            msg2F.playerNum = 2;
            msg2F.fire = InputManager.GetButtonDown(INPUTBUTTON.P2_FIRE);
            Message sendMsgF = new Message();
            sendMsgF.populateMessage(msg2F);
            MessageToClient(sendMsgF);

            MineMessage msg2M = new MineMessage();
            msg2M.playerNum = 2;
            msg2M.dropMine = InputManager.GetButtonDown(INPUTBUTTON.P2_LAYMINE);
            Message sendMsgM = new Message();
            sendMsgM.populateMessage(msg2M);
            MessageToClient(sendMsgM);

            
            

            /* Screen log example
            ScreenLog.Add(string.Format("Frame Time: {0:0.0}", 1 / TimeManager.GetFrameTime()));
            ScreenLog.Add(Colors.DarkKhaki, string.Format("P1 ammo: {0}", PlayerMgr.P1Data.missileCount));
            ScreenLog.Add(Colors.Orchid, string.Format("P2 ammo: {0}", PlayerMgr.P2Data.missileCount));
            //*/
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
