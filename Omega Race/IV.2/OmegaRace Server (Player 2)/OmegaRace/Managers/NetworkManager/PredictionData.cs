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
    public class PredictionData
    {
        public void UpdateData(GameObject gameObject, Vec2 pos)
        {
            //arrival time
            float arrivalTime = TimeManager.GetCurrentTime();

            if (arrivalTime != gameObject.lastTime)
            {

                Vec2 pixel = pos;
                Vec2 last = gameObject.lastPos;
                float calc = 1 / (arrivalTime - gameObject.lastTime);
                Vec2 vel = (pixel - last) * calc;

                gameObject.lastVelocity = gameObject.lastVelocity = (pos - gameObject.lastPos) * (1 / (arrivalTime - gameObject.lastTime));

                gameObject.lastPos = pos;

                gameObject.lastTime = arrivalTime;
            }
        }

        public void Set(GameObject gameObject)
        {
            gameObject.lastPos = gameObject.GetPixelPosition();
            gameObject.lastVelocity = new Vec2(0, 0);
            gameObject.lastTime = TimeManager.GetCurrentTime();

        }

        public Vec2 PredictPos(GameObject gameObject)
        {
            float delta = TimeManager.GetCurrentTime() - gameObject.lastTime;

            return gameObject.lastPos + delta * gameObject.lastVelocity;
        }
    }
}
