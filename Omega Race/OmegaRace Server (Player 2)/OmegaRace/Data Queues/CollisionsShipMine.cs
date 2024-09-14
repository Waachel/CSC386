using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OmegaRace.Data_Queues
{
    //message for fence mine collision
    [Serializable]
    public class CollisionsShipMine : DataMessage
    {
        public int playerNum;
        public int mineID;

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(playerNum);
            writer.Write(mineID);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            playerNum = reader.ReadInt32();
            mineID = reader.ReadInt32();
        }

        public override void Execute()
        {
            //search active mine list
            foreach (Mine mine in ActiveMineList.activeMines)
            {
                if (mineID == mine.getID())
                {
                    //what happens on mine hit
                    mine.OnHit();

                    //remove mine from active list
                    ActiveMineList.activeMines.Remove(mine);

                    break;
                }
            }

            PlayerManager plMgr = GameSceneCollection.ScenePlay.PlayerMgr;

            //get a new ship
            if (playerNum == 1)
            {
                //what happens on ship hit
                plMgr.P1Data.ship.OnHit();
            }
            //playerNum 2
            else
            {
                //what happens on ship hit
                plMgr.P2Data.ship.OnHit();
            }

        }

        public override void Reset()
        {
            playerNum = 0;
            mineID = 0;
        }
    }
}
