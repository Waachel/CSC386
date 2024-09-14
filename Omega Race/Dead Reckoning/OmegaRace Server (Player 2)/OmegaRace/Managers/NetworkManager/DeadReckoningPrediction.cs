using Box2DX.Common;
using OmegaRace.Data_Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Managers.Player
{
    public class DeadReckoningPrediction
    {
        public void Set(GameObject gameObject)
        {
            gameObject.lastPos = gameObject.currPos;
            gameObject.lastVelocity = new Vec2(0,0);
            gameObject.lastTime = gameObject.currTime;

        }

        public Vec2 PredictPos(GameObject gameObject)
        {
            float delta = TimeManager.GetCurrentTime() - gameObject.lastTime;

            return gameObject.lastPos + delta * gameObject.lastVelocity;
        }

        public void UpdatePredictionData(GameObject gameObject, Vec2 pos, Vec2 vel, float time)
        {
            gameObject.predictedPos = pos;
            gameObject.predictedVelocity = vel;
            gameObject.predictedTime = time;
        }

        public Vec2 ClientPredictPos(GameObject gameObject)
        {
            float delta = TimeManager.GetCurrentTime() - gameObject.predictedTime;

            return gameObject.predictedPos + delta * gameObject.predictedVelocity;
        }
    }
}
