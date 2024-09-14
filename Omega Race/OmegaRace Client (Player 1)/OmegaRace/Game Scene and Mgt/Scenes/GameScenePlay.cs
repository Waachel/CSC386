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
            NetworkMgr = new NetworkManager("localhost", 14240);
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

            // Process reactions to inputs for Player 1
            int p1_H = InputManager.GetAxis(INPUTAXIS.HORIZONTAL_P1);
            int p1_V = InputManager.GetAxis(INPUTAXIS.VERTICAL_P1);
            //PlayerMgr.P1Data.ship.Rotate(p1_H);
            //PlayerMgr.P1Data.ship.Move(p1_V);
            

            PlayerMovementMessage msg1 = new PlayerMovementMessage();
            msg1.playerNum = 1;
            msg1.horzInput = p1_H;
            msg1.vertInput = p1_V;

            Message sendMsg = new Message();
            sendMsg.populateMessage(msg1);
            MessageToServer(sendMsg);

            FireMessage msg1F = new FireMessage();
            msg1F.playerNum = 1;
            msg1F.fire = InputManager.GetButtonDown(INPUTBUTTON.P1_FIRE);
            Message sendMsgF = new Message();
            sendMsgF.populateMessage(msg1F);
            MessageToServer(sendMsgF);

            MineMessage msg1M = new MineMessage();
            msg1M.playerNum = 1;
            msg1M.dropMine = InputManager.GetButtonDown(INPUTBUTTON.P1_LAYMINE);
            Message sendMsgM = new Message();
            sendMsgM.populateMessage(msg1M);
            MessageToServer(sendMsgM);





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
