using OmegaRace.Data_Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace
{
    public class CollisionEvent
    {
        public static List<Message> collisions = new List<Message>();
        public static void Action(Fence f, Ship s)
        {
            CollisionsFenceShip msg = new CollisionsFenceShip();
            msg.fenceID = f.getID();

            Message fsMsg = new Message();
            fsMsg.PopulateMessage(msg);

            //add to collisions list
            collisions.Add(fsMsg);
        }
        public static void Action(Missile m, Ship s)
        {
            if (m.GetOwner() != s.GetOwner())
            {
                CollisionsMissileShip msg = new CollisionsMissileShip();
                if (s.GetOwner().player == Player.Player1)
                {
                    msg.playerNum = 1;
                }
                //Player 2
                else
                {
                    msg.playerNum = 2;
                }

                msg.missileID = m.getID();
                Message msMsg = new Message();
                msMsg.PopulateMessage(msg);

                // add to collisions list
                collisions.Add(msMsg);
            }
        }

        public static void Action(Fence f, Missile m)
        {
            CollisionsFenceMissile msg = new CollisionsFenceMissile();
            msg.missileID = m.getID();
            msg.fenceID = f.getID();

            Message fmMsg = new Message();
            fmMsg.PopulateMessage(msg);

            //add to collisions list
            collisions.Add(fmMsg);
        }

        public static void Action(Missile m, FencePost p)
        {
            CollisionsMissileFencePost msg = new CollisionsMissileFencePost();
            msg.missileID = m.getID();

            Message mpMsg = new Message();
            mpMsg.PopulateMessage(msg);

            //add to collisions list
            collisions.Add(mpMsg);
        }

        public static void Action(Ship s, Mine m)
        {
            if (m.GetOwner() != s.GetOwner() && m.state == MINE_STATE.ARMED)
            {
                CollisionsShipMine msg = new CollisionsShipMine();
                if (s.GetOwner().player == Player.Player1)
                {
                    msg.playerNum = 1;
                }
                //Player 2
                else
                {
                    msg.playerNum = 2;
                }

                msg.mineID = m.getID();
                Message smMsg = new Message();
                smMsg.PopulateMessage(msg);

                // add to collisions list
                collisions.Add(smMsg);
            }
        }
    }
}
