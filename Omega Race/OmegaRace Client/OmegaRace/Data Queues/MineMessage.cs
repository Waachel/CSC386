using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace OmegaRace.Data_Queues
{
    [Serializable]
    public class MineMessage : DataMessage
    {
        public int playerNum;
        public bool dropMine = false;
        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(playerNum);
            writer.Write(dropMine);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            playerNum = reader.ReadInt32();
            dropMine = reader.ReadBoolean();
        }

        public override void Execute()
        {
            // Locate player manager
            PlayerManager plMgr = GameSceneCollection.ScenePlay.PlayerMgr;

            if (playerNum == 1 && dropMine)
            {
                plMgr.P1Data.LayMine();
            }

            else if (playerNum == 2 && dropMine)
            {
                plMgr.P2Data.LayMine();
            }

            else
            {
                //do nothing
            }
        }


    }
}
