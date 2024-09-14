using Box2DX.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static OmegaRace.MessageQueueManager;

namespace OmegaRace.Data_Queues
{
    public struct MineData
    {
        public int mineID;
        public Vec2 pos;
    }

    [Serializable]

    public class UpdateMineMessage : DataMessage
    {
        public List<MineData> mineData;

        public UpdateMineMessage()
        {
            mineData = new List<MineData>();
        }

        public void Set(GameObject gObj)
        {
            MineData mData;
            mData.mineID = gObj.getID();
            mData.pos = gObj.GetPixelPosition();

            mineData.Add(mData);
        }
        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(mineData.Count);

            foreach (MineData item in mineData)
            {
                writer.Write(item.mineID);
                writer.Write(item.pos.X);
                writer.Write(item.pos.Y);
            }
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            // get list count.
            mineData.Capacity = reader.ReadInt32();

            MineData mData;
            int i = 0;
            while (i++ < mineData.Capacity)
            {
                mData.mineID = reader.ReadInt32();
                mData.pos.X = reader.ReadSingle();
                mData.pos.Y = reader.ReadSingle();

                mineData.Add(mData);
            }


        }

        public override void Execute()
        {
            // search active missile list
            foreach (MineData item in mineData)
            {
                foreach (Mine mine in ActiveMineList.activeMines)
                {
                    if (item.mineID == mine.getID())
                    {
                        mine.SetPosAndAngle(item.pos.X, item.pos.Y, 0);
                    }
                }
            }
        }

        public override void Reset()
        {
            mineData.Clear();
        }
    }
}
