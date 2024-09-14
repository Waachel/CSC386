using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Box2DX.Common;
using OmegaRace.Data_Queues;
using static OmegaRace.MessageQueueManager;
using System.Windows.Interop;
using OmegaRace.Game_Scene_and_Mgt.Modes;

namespace OmegaRace.Data_Queues
{
    // Message which initiate drop mine event.
    [Serializable]
    public class MineEvent : DataMessage
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
            MineMessage mineMsg = GameSceneCollection.ScenePlay.MsgMgr.GetMineMessage();
            mineMsg.playerNum = playerNum;
            Message msg = new Message();
            msg.PopulateMessage(mineMsg);

            if (GameSceneCollection.ScenePlay.MsgQueueMgr.mode.type != GameMode.modeType.PLAYBACK)
            {
                GameSceneCollection.ScenePlay.MessageToServer(msg);
            }
            GameSceneCollection.ScenePlay.MessageToClient(msg);

            GameSceneCollection.ScenePlay.MsgMgr.ReleaseMineMessage(mineMsg);
        }

        public override void Reset()
        {
            playerNum = 0;
        }
    }
}
