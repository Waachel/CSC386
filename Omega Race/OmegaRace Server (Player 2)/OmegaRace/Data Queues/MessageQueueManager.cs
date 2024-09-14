using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Collections;
using static OmegaRace.MessageQueueManager;
using System.Windows.Interop;

namespace OmegaRace
{
    public class MessageQueueManager 
    {
        public struct QueueMessage
        {
            public Data_Queues.Message msg;
        }
        protected Queue<QueueMessage> pInputQueue;
        protected Queue<QueueMessage> pOutputQueue;

        public MessageQueueManager()
        {
            pInputQueue = new Queue<QueueMessage>();
            pOutputQueue = new Queue<QueueMessage>();
        }

        public void AddToInputQueue(QueueMessage msg)
        {
            pInputQueue.Enqueue(msg);
        }

        public void AddToOutputQueue(QueueMessage msg)
        {
            pOutputQueue.Enqueue(msg);
        }

        void ProcessOut()
        {
            while (pOutputQueue.Count > 0)
            {
                QueueMessage msg = pOutputQueue.Dequeue();

                AddToInputQueue(msg);
            }

            //ScreenLog.Add("Net msg count: " + msgcounter);
        }

        void ProcessIn()
        {
            while (pInputQueue.Count > 0)
            {
                QueueMessage qmsg = pInputQueue.Dequeue();

                qmsg.msg.dataMsg.Execute();
            }
        }

        public void Process()
        {
            ProcessOut();
            ProcessIn();
        }
    }
}
