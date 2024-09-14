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
            // Locate player manager
            PlayerManager plMgr = GameSceneCollection.ScenePlay.PlayerMgr;

            if (playerNum == 1)
            {
                plMgr.P1Data.LayMine();
            }

            else if (playerNum == 2)
            {
                plMgr.P2Data.LayMine();
            }

            ActiveMineList.mineDropped = true;
        }

        public override void Reset()
        {
            playerNum = 0;
        }
    }
}
