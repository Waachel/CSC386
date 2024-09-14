using OmegaRace.Game_Scene_and_Mgt.Modes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace OmegaRace.Data_Queues
{
    [Serializable]
    public class TimeRequestMessage : DataMessage
    {
        public float ClientTime;

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(ClientTime);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            ClientTime = reader.ReadSingle();
        }

        public override void Execute()
        {
            TimeReplyMessage replyMsg = GameSceneCollection.ScenePlay.MsgMgr.GetTimeReplyMessage();
            replyMsg.ServerTime = TimeManager.GetCurrentTime();
            replyMsg.ClientTime = ClientTime;
            Message msg = new Message();
            msg.PopulateMessage(replyMsg);
            GameSceneCollection.ScenePlay.MessageToClient(msg);
            Debug.WriteLine("Server Time: " + replyMsg.ServerTime);
            GameSceneCollection.ScenePlay.MsgMgr.ReleaseTimeReplyMessage(replyMsg);
        }

        public override void Reset()
        {
            ClientTime = 0.0f;
        }
    }
}
