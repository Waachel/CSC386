using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{
    [Serializable]
    public class FireMessage : DataMessage
    {
        public int playerNum;
        public bool fire = false;
        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(playerNum);
            writer.Write(fire);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            playerNum = reader.ReadInt32();
            fire = reader.ReadBoolean();
        }

        public override void Execute()
        {
            // Locate player manager
            PlayerManager plMgr = GameSceneCollection.ScenePlay.PlayerMgr;

            if (playerNum == 1 && fire)
            {
                plMgr.P1Data.FireMissile();
            }

            else if (playerNum == 2 && fire)
            {
                plMgr.P2Data.FireMissile();
            }

            else
            {
                //do nothing
            }
        }


    }
}
