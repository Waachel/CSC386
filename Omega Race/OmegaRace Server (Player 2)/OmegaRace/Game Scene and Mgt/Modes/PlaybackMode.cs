using OmegaRace.Data_Queues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using static OmegaRace.MessageQueueManager;

namespace OmegaRace.Game_Scene_and_Mgt.Modes
{
    public class PlaybackMode : GameMode
    {
        private BinaryReader reader;
        public bool playbackFinished = false;
        private bool getNextMsg = true;
        private float playbackTime = 0.0f;
        private QueueMessage playbackMsg;
        public override void SetupMode()
        {
            type = modeType.PLAYBACK;

            try
            {
                reader = new BinaryReader(new FileStream("../bin/Debug/" + fileName, FileMode.Open, FileAccess.Read));
            }
            catch (Exception e)
            {
                //file doesnt exist
                Debug.WriteLine("{0}", e.ToString());
            }
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

                //send to server
                //GameSceneCollection.ScenePlay.MessageToServer(msg.msg);

                msgQueueMgr.AddToInputQueue(msg);
            }

            //ScreenLog.Add("Net msg count: " + msgcounter);
        }

        public void ReadFromFile()
        {
            MessageQueueManager msgQueueMgr = GameSceneCollection.ScenePlay.MsgQueueMgr;
            
            while (!playbackFinished)
            {
                if (getNextMsg)
                {
                    if (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        //get order number
                        Debug.WriteLine("\nSeq:#{0}", reader.ReadInt32());

                        //get game time from playback
                        playbackTime = reader.ReadSingle();
                        Debug.WriteLine("Time:{0}", playbackTime);

                        //create new message
                        playbackMsg = new QueueMessage();
                        playbackMsg.msg = new Message();


                        //deserialize message data from file
                        playbackMsg.msg.Deserialize(ref reader);

                        //get message type
                        Debug.WriteLine("{0}", playbackMsg.msg.msgType);

                        getNextMsg = false;
                    }
                    else
                    {
                        playbackFinished = true;
                        break;
                    }
                }

                // if current game time is greater than message time.
                if ((TimeManager.GetCurrentTime() >= playbackTime) && (getNextMsg == false))
                {

                    QueueMessage newMsg = new QueueMessage();
                    newMsg.msg = playbackMsg.msg;

                    //add to output queue
                    msgQueueMgr.AddToOutputQueue(newMsg);

                    // get next data.
                    getNextMsg = true;
                }
                else
                {
                    break;
                }
            }
        }

    }
}
