using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OmegaRace.MessageQueueManager;

namespace OmegaRace.Game_Scene_and_Mgt.Modes
{
    public class RecordMode : GameMode
    {
        private FileStream myfs;
        private BinaryWriter writer;
        private int orderNum;
        public override void SetupMode()
        {
            orderNum = 0;
            type = modeType.RECORD;
            myfs = new FileStream("../bin/Debug/" + fileName, FileMode.Create, FileAccess.Write);
            writer = new BinaryWriter(myfs);
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

               WriteToFile(msg);

                msgQueueMgr.AddToInputQueue(msg);
            }

            //ScreenLog.Add("Net msg count: " + msgcounter);
        }

        public void WriteToFile(QueueMessage msg)
        {
            //write order number to file
            writer.Write(orderNum++);

            //write game time to file
            writer.Write(TimeManager.GetCurrentTime());

            //write delivery method to file

            //serialize message to file
            msg.msg.Serialize(ref writer);

            myfs.Flush();
        }

    }
}
