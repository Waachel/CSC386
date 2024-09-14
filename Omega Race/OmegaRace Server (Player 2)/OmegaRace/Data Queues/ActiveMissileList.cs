using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{
    public class ActiveMissileList
    {
        //was a missile fired?
        public static bool missileFired;

        //list of active missiles
        public static List<Missile> activeMissiles = new List<Missile>();

    }
}
