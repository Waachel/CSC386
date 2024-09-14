using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{
    [Serializable]
    public class PlayerMovementMessage : DataMessage
    {
        public int horzInput;
        public int vertInput;
        public int playerNum;
        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(playerNum);
            writer.Write(horzInput);
            writer.Write(vertInput);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            playerNum = reader.ReadInt32();
            horzInput = reader.ReadInt32();
            vertInput = reader.ReadInt32();
        }

        public override void Execute()
        {
            // Locate player manager
            PlayerManager plMgr = GameSceneCollection.ScenePlay.PlayerMgr;

            if (playerNum == 1)
            {
                plMgr.P1Data.ship.Move(vertInput);
                plMgr.P1Data.ship.Rotate(horzInput);
            }

            else if (playerNum == 2)
            {
                plMgr.P2Data.ship.Move(vertInput);
                plMgr.P2Data.ship.Rotate(horzInput);

            }

            else
            {
                //do nothing
            }
        }

        public override void Reset()
        {
            playerNum = 0; 
            horzInput = 0; 
            vertInput = 0;
        }
    }
}
