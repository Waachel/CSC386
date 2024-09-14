using Box2DX.Common;
using OmegaRace.Managers.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace OmegaRace.Data_Queues
{


    [Serializable]

    public class UpdatePlayerMovementMessage : DataMessage
    {
        public Vec2 pos;
        public Vec2 vel;
        public float angle;
        public int playerNum;

        public void Set(GameObject gObj)
        {
            //id = gObj.getID();
            pos = gObj.GetPixelPosition();
            angle = gObj.GetAngle_Deg();
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
            PredictionData predictionData = new PredictionData();
            Vec2 predictedPosition;

            if (playerNum == 1)
            {
                predictionData.UpdateData(plMgr.P1Data.ship, pos);
                plMgr.P1Data.ship.pBody.SetAngle(angle);
                predictedPosition = predictionData.PredictPos(plMgr.P1Data.ship);
                plMgr.P1Data.ship.SetPosAndAngle(predictedPosition.X, predictedPosition.Y, plMgr.P1Data.ship.GetAngle_Deg());
            }

            else //(playerNum == 2)
            {
                predictionData.UpdateData(plMgr.P2Data.ship, pos);
                plMgr.P2Data.ship.pBody.SetAngle(angle);
                predictedPosition = predictionData.PredictPos(plMgr.P2Data.ship);
                plMgr.P2Data.ship.SetPosAndAngle(predictedPosition.X, predictedPosition.Y, plMgr.P2Data.ship.GetAngle_Deg());
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
