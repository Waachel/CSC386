using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{
    public class ActiveMineList
    {
        //was a mine fired?
        public static bool mineDropped;

        //list of active mines
        public static List<Mine> activeMines = new List<Mine>();

    }
}
