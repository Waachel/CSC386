using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OmegaRace.Data_Queues
{
    //message for fence and ship collision
    [Serializable]
    public class CollisionsFenceShip : DataMessage
    {
        public int fenceID;

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(fenceID);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            fenceID = reader.ReadInt32();
        }

        public override void Execute()
        {
            //find fence
            Fence newFence = (Fence)GameManager.Find(fenceID);

            //what happens on fence hit
            newFence.OnHit();
        }

        public override void Reset()
        {
            fenceID = 0;
        }
    }
}
