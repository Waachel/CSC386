using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OmegaRace.MessageQueueManager;

namespace OmegaRace.Game_Scene_and_Mgt.Modes
{
    public abstract class GameMode
    {
        public enum modeType
        {
            NORMAL,
            RECORD,
            PLAYBACK
        }

        //file name for recording and playback modes
        public string fileName = "GameRecording.rec";

        //default mode type
        public modeType type = modeType.NORMAL; 
        public abstract void SetupMode ();
        public abstract void ProcessInputQueue(Queue<QueueMessage> pInputQueue);
        public abstract void ProcessOutputQueue(Queue<QueueMessage> pOutputQueue);
    }
}
