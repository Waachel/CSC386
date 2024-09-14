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
    public class CollisionsFenceMissile : DataMessage
    {
        public int missileID;
        public int fenceID;

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(missileID);
            writer.Write(fenceID);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            missileID = reader.ReadInt32();
            fenceID = reader.ReadInt32();
        }

        public override void Execute()
        {
            //find fence
            Fence newFence = (Fence)GameManager.Find(fenceID);

            //what happens on fence hit
            newFence.OnHit();

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

        }

        public override void Reset()
        {
            missileID = 0;
            fenceID = 0;
        }
    }
}
