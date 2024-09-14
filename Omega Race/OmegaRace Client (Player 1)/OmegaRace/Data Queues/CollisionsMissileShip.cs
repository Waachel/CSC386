using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OmegaRace.Data_Queues
{
    //message for fence missile collision
    [Serializable]
    public class CollisionsMissileShip : DataMessage
    {
        public int playerNum;
        public int missileID;

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(playerNum);
            writer.Write(missileID);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            playerNum = reader.ReadInt32();
            missileID = reader.ReadInt32();
        }

        public override void Execute()
        {
            //search active missile list
            foreach (Missile missile in ActiveMissileList.activeMissiles)
            {
                if (missileID == missile.getID())
                {
                    //what happens on missile hit
                    missile.OnHit();

                    //remove missile from active list
                    ActiveMissileList.activeMissiles.Remove(missile);

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
            missileID = 0;
        }
    }
}
