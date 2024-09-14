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
    public class PlayerPredictionControl
    {
        public void Update (GameObject gameObject)
        {
            //arrival time
            float arrivalTime = TimeManager.GetCurrentTime();

            if (arrivalTime != gameObject.lastTime)
            {
                //update velocity
                gameObject.lastVelocity = (gameObject.GetPixelPosition() - gameObject.lastPos) * (1 / (arrivalTime - gameObject.lastTime));

                //update pos
                gameObject.lastPos = gameObject.GetPixelPosition();

                //update time
                gameObject.lastTime = arrivalTime;
            }
        }

        public void Set (GameObject gameObject)
        {
            gameObject.lastPos = gameObject.GetPixelPosition();
            gameObject.lastVelocity = new Vec2 (0,0);
            gameObject.lastTime = TimeManager.GetCurrentTime();

        }

        public void PredictPos(GameObject gameObject)
        {
            float timeDelta = TimeManager.GetCurrentTime() - gameObject.lastTime;

            Vec2 predictedPos = gameObject.lastPos + timeDelta * gameObject.lastVelocity;

            gameObject.SetPosAndAngle(predictedPos.X, predictedPos.Y, gameObject.GetAngle_Deg());
        }
    }
}
