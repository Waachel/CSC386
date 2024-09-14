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
using OmegaRace.Game_Scene_and_Mgt.Modes;

namespace OmegaRace
{
    public class MessageQueueManager 
    {
        public struct QueueMessage
        {
            public Data_Queues.Message msg;
        }
        public Queue<QueueMessage> pInputQueue;
        public Queue<QueueMessage> pOutputQueue;
        public GameMode mode;
        public MessageQueueManager(GameMode gm)
        {
            pInputQueue = new Queue<QueueMessage>();
            pOutputQueue = new Queue<QueueMessage>();
            mode = gm;
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
            mode.ProcessOutputQueue(pOutputQueue);
        }

        void ProcessIn()
        {
            mode.ProcessInputQueue(pInputQueue);
        }

        public void Process()
        {
            ProcessOut();
            ProcessIn();
        }
    }
}
