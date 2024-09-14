using Box2DX.Common;
using OmegaRace.Managers.Player;
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
    public struct MissileData
    {
        public int missileID;
        public Vec2 pos;
        public float angle;
    }

    [Serializable]

    public class UpdateMissileMessage : DataMessage
    {
        public List<MissileData> missileData;

        public UpdateMissileMessage()
        {
            missileData = new List<MissileData>();
        }

        public void Set(GameObject gObj)
        {
            MissileData mData;
            mData.missileID = gObj.getID();
            mData.pos = gObj.GetPixelPosition();
            mData.angle = gObj.GetAngle_Deg();

            missileData.Add(mData);
        }
        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(missileData.Count);

            foreach (MissileData item in missileData)
            {
                writer.Write(item.missileID);
                writer.Write(item.pos.X);
                writer.Write(item.pos.Y);
                writer.Write(item.angle);
            }
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            // get list count.
            missileData.Capacity = reader.ReadInt32();

            MissileData mData;
            int i = 0;
            while (i++ < missileData.Capacity)
            {
                mData.missileID = reader.ReadInt32();
                mData.pos.X = reader.ReadSingle();
                mData.pos.Y = reader.ReadSingle();
                mData.angle = reader.ReadSingle();

                missileData.Add(mData);
            }
            

        }

        public override void Execute()
        {
            PlayerPredictionControl prediction = new PlayerPredictionControl();

            // search active missile list
            foreach (MissileData item in missileData)
            {
                foreach (Missile missile in ActiveMissileList.activeMissiles)
                {
                    if (item.missileID == missile.getID())
                    {
                        missile.SetPosAndAngle(item.pos.X, item.pos.Y, item.angle);
                        prediction.Update(missile);
                    }
                }
            }
        }

        public override void Reset()
        {
            missileData.Clear();
        }
    }
}
