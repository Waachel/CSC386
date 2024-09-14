using Box2DX.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{


    [Serializable]

    public class UpdatePlayerMovementMessage : DataMessage
    {
        public Vec2 pos;
        public float angle;
        public int playerNum;

        public void Set(GameObject gObj)
        {
            //id = gObj.getID();
            pos = gObj.GetPixelPosition();
            angle = gObj.GetAngle_Deg();
            //velocity = gObj.GetWorldVelocity();
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(playerNum);
            writer.Write(pos.X);
            writer.Write(pos.Y);
            writer.Write(angle);
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            playerNum = reader.ReadInt32();
            pos.X = reader.ReadSingle();
            pos.Y = reader.ReadSingle();
            angle = reader.ReadSingle();
        }

        public override void Execute()
        {
            // Locate player manager
            PlayerManager plMgr = GameSceneCollection.ScenePlay.PlayerMgr;

            if (playerNum == 1)
            {
                plMgr.P1Data.ship.SetPosAndAngle(pos.X, pos.Y, angle);
        }

            else if (playerNum == 2)
            {
                plMgr.P2Data.ship.SetPosAndAngle(pos.X, pos.Y, angle);
            }

            else
            {
                //do nothing
            }
        }
        public override void Reset()
        {
            pos = default(Vec2);
            angle = 0.0f;
            playerNum = 0;
        }

    }
}
