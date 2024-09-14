using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{
    [Serializable]
    public class TimeReplyMessage : DataMessage
    {
        public float ServerTime;
        public float ClientTime;
        public float ActualServerTime;

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(ServerTime);
            writer.Write(ClientTime);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            ServerTime = reader.ReadSingle();
            ClientTime = reader.ReadSingle();
        }

        public override void Execute()
        {
            float delay = TimeManager.GetCurrentTime() - ClientTime;
            ActualServerTime = ServerTime + (delay / 2);

            //Set time based on actual server time
            TimeManager.SetTime(ActualServerTime);
            Debug.WriteLine("Client Time: " + ActualServerTime);
        }

        public override void Reset()
        {
            ServerTime = 0.0f;
            ClientTime = 0.0f;
        }
    }
}
