using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Box2DX.Common;
using OmegaRace.Data_Queues;
using static OmegaRace.MessageQueueManager;
using OmegaRace.Game_Scene_and_Mgt.Modes;

namespace OmegaRace.Data_Queues
{
    // Message which initiate fire missile event.
    [Serializable]
    public class MissileEvent : DataMessage
    {
        public int playerNum;
        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(playerNum);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            playerNum = reader.ReadInt32();
        }

        public override void Execute()
        {

            MissileMessage fireMsg = GameSceneCollection.ScenePlay.MsgMgr.GetMissileMessage();
            fireMsg.playerNum = playerNum;
            Message msg = new Message();
            msg.PopulateMessage(fireMsg);
            if(GameSceneCollection.ScenePlay.MsgQueueMgr.mode.type != GameMode.modeType.PLAYBACK)
            {
                GameSceneCollection.ScenePlay.MessageToServer(msg);
            }
            
            GameSceneCollection.ScenePlay.MessageToClient(msg);

            GameSceneCollection.ScenePlay.MsgMgr.ReleaseMissileMessage(fireMsg);
        }

        public override void Reset()
        {
            playerNum = 0;
        }
    }
}
