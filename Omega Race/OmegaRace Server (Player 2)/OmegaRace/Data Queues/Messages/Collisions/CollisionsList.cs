using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OmegaRace.Data_Queues
{
    public class CollisionsList : DataMessage
    {
        public List<Message> collisions;

        public CollisionsList()
        {

        }
        public CollisionsList(List<Message> l)
        {
            // copy collision list
            collisions = new List<Message>(l);
        }

        public override void Serialize(ref BinaryWriter writer)
        {
            writer.Write(collisions.Count);

            foreach (Message item in collisions)
            {
                if (item.msgType == MessageType.COLLISIONS_FENCE_MISSILE)
                {
                    writer.Write((int)MessageType.COLLISIONS_FENCE_MISSILE);

                    // down cast
                    CollisionsFenceMissile fenceMissileCollision = (CollisionsFenceMissile)item.dataMsg;
                    writer.Write(fenceMissileCollision.missileID);
                    writer.Write(fenceMissileCollision.fenceID);
                }
                else if (item.msgType == MessageType.COLLISIONS_FENCE_SHIP)
                {
                    writer.Write((int)MessageType.COLLISIONS_FENCE_SHIP);

                    // down cast
                    CollisionsFenceShip fenceShipCollision = (CollisionsFenceShip)item.dataMsg;
                    writer.Write(fenceShipCollision.fenceID);
                }
                else if (item.msgType == MessageType.COLLISIONS_MISSILE_FENCEPOST)
                {
                    writer.Write((int)MessageType.COLLISIONS_MISSILE_FENCEPOST);

                    // down cast
                    CollisionsMissileFencePost missileFencePostCollision = (CollisionsMissileFencePost)item.dataMsg;
                    writer.Write(missileFencePostCollision.missileID);
                }
                else if (item.msgType == MessageType.COLLISIONS_MISSILE_SHIP)
                {
                    writer.Write((int)MessageType.COLLISIONS_MISSILE_SHIP);

                    // down cast
                    CollisionsMissileShip missileShipCollision = (CollisionsMissileShip)item.dataMsg;
                    writer.Write(missileShipCollision.playerNum);
                    writer.Write(missileShipCollision.missileID);
                }
                //Ship Mine
                else
                {
                    writer.Write((int)MessageType.COLLISIONS_SHIP_MINE);

                    // down cast
                    CollisionsShipMine shipMineCollision = (CollisionsShipMine)item.dataMsg;
                    writer.Write(shipMineCollision.playerNum);
                    writer.Write(shipMineCollision.mineID);
                }
            }
        }

        public override void Deserialize(ref BinaryReader reader)
        {
            collisions = new List<Message>(reader.ReadInt32());

            int i = 0;
            while (i++ < collisions.Capacity)
            {
                MessageType newType = (MessageType)reader.ReadInt32();
                switch (newType)
                {
                    case MessageType.COLLISIONS_FENCE_MISSILE:
                        CollisionsFenceMissile fenceMissileCollision = new CollisionsFenceMissile();
                        fenceMissileCollision.missileID = reader.ReadInt32();
                        fenceMissileCollision.fenceID = reader.ReadInt32();
                        Message fenceMissileMsg = new Message();
                        fenceMissileMsg.PopulateMessage(fenceMissileCollision);
                        collisions.Add(fenceMissileMsg);
                        break;

                    case MessageType.COLLISIONS_FENCE_SHIP:
                        CollisionsFenceShip fenceShipCollision = new CollisionsFenceShip();
                        fenceShipCollision.fenceID = reader.ReadInt32();
                        Message fenceShipMsg = new Message();
                        fenceShipMsg.PopulateMessage(fenceShipCollision);
                        collisions.Add(fenceShipMsg);
                        break;

                    case MessageType.COLLISIONS_MISSILE_FENCEPOST:
                        CollisionsMissileFencePost missileFencePostCollision = new CollisionsMissileFencePost();
                        missileFencePostCollision.missileID = reader.ReadInt32();
                        Message missileFencePostMsg = new Message();
                        missileFencePostMsg.PopulateMessage(missileFencePostCollision);
                        collisions.Add(missileFencePostMsg);
                        break;

                    case MessageType.COLLISIONS_MISSILE_SHIP:
                        CollisionsMissileShip missileShipCollision = new CollisionsMissileShip();
                        missileShipCollision.playerNum = reader.ReadInt32();
                        missileShipCollision.missileID = reader.ReadInt32();
                        Message missileShipMsg = new Message();
                        missileShipMsg.PopulateMessage(missileShipCollision);
                        collisions.Add(missileShipMsg);
                        break;

                    case MessageType.COLLISIONS_SHIP_MINE:
                        CollisionsShipMine shipMineCollision = new CollisionsShipMine();
                        shipMineCollision.playerNum = reader.ReadInt32();
                        shipMineCollision.mineID = reader.ReadInt32();
                        Message shipMineMsg = new Message();
                        shipMineMsg.PopulateMessage(shipMineCollision);
                        collisions.Add(shipMineMsg);
                        break;
                }
            }
        }

        public override void Execute()
        {
            foreach (Message item in collisions)
            {
                item.dataMsg.Execute();
            }
        }

        public override void Reset()
        {
            collisions.Clear();
        }
    }
}
