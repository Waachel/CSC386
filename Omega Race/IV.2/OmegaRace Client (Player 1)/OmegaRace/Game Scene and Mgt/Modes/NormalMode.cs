using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OmegaRace.MessageQueueManager;

namespace OmegaRace.Game_Scene_and_Mgt.Modes
{
    public class NormalMode : GameMode
    {
        public override void SetupMode()
        {
            //nothing needed for Normal
        }
        public override void ProcessInputQueue(Queue<QueueMessage> pInputQueue)
        {
            while (pInputQueue.Count > 0)
            {
                QueueMessage qmsg = pInputQueue.Dequeue();

                qmsg.msg.dataMsg.Execute();
            }
        }

        public override void ProcessOutputQueue(Queue<QueueMessage> pOutputQueue)
        {
            MessageQueueManager msgQueueMgr = GameSceneCollection.ScenePlay.MsgQueueMgr;

            while (pOutputQueue.Count > 0)
            {
                QueueMessage msg = pOutputQueue.Dequeue();

                msgQueueMgr.AddToInputQueue(msg);
            }

            //ScreenLog.Add("Net msg count: " + msgcounter);
        }

    }
}
