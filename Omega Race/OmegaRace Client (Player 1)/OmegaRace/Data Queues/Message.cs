using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Data_Queues
{
    //Type of Message
    public enum MessageType
    {
        MESSAGE_PLAYER_MOVEMENT,
        MESSAGE_FIRE,
        MESSAGE_MINE,
        UPDATE_PLAYER_MOVEMENT,
        UPDATE_FIRE,
        UPDATE_MINE,
        COLLISIONS_LIST,
        COLLISIONS_FENCE_SHIP,
        COLLISIONS_MISSILE_SHIP,
        COLLISIONS_FENCE_MISSILE,
        COLLISIONS_MISSILE_FENCEPOST,
        COLLISIONS_SHIP_MINE,
        MISSILE_EVENT,
        MINE_EVENT
    }

    [Serializable]
    public class Message
    {
        public NetDeliveryMethod deliveryMethod;
        public MessageType msgType;
        public DataMessage dataMsg;


        //Message type is player movement
        public void PopulateMessage(PlayerMovementMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MESSAGE_PLAYER_MOVEMENT;
            dataMsg = newMsg;
        }

        //Message type is fire missile event from client
        public void PopulateMessage(MissileEvent newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MISSILE_EVENT;
            dataMsg = newMsg;
        }

        //Message type is drop mine event from client
        public void PopulateMessage(MineEvent newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MINE_EVENT;
            dataMsg = newMsg;
        }

        //Message type is fire
        public void PopulateMessage(MissileMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MESSAGE_FIRE;
            dataMsg = newMsg;
        }

        //Message type is mine
        public void PopulateMessage(MineMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MESSAGE_MINE;
            dataMsg = newMsg;
        }

        //Message type is update player
        public void PopulateMessage(UpdatePlayerMovementMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.UPDATE_PLAYER_MOVEMENT;
            dataMsg = newMsg;
        }

        //Message type is update missile
        public void PopulateMessage(UpdateMissileMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.UPDATE_FIRE;
            dataMsg = newMsg;
        }

        //Message type is update mine
        public void PopulateMessage(UpdateMineMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.UPDATE_MINE;
            dataMsg = newMsg;
        }

        //Message type is list of collisions
        public void PopulateMessage(CollisionsList newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.COLLISIONS_LIST;
            dataMsg = newMsg;
        }

        //Message type is collisions of fence and missile
        public void PopulateMessage(CollisionsFenceMissile newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.COLLISIONS_FENCE_MISSILE;
            dataMsg = newMsg;
        }

        //Message type is collisions of fence and ship
        public void PopulateMessage(CollisionsFenceShip newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.COLLISIONS_FENCE_SHIP;
            dataMsg = newMsg;
        }

        //Message type is collisions of missile and ship
        public void PopulateMessage(CollisionsMissileShip newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.COLLISIONS_MISSILE_SHIP;
            dataMsg = newMsg;
        }

        //Message type is collisions of missile and fence post
        public void PopulateMessage(CollisionsMissileFencePost newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.COLLISIONS_MISSILE_FENCEPOST;
            dataMsg = newMsg;
        }

        //Message type is collisions of ship and mine
        public void PopulateMessage(CollisionsShipMine newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.COLLISIONS_SHIP_MINE;
            dataMsg = newMsg;
        }

        public void Serialize(ref BinaryWriter writer)
        {
            writer.Write((int)msgType);
            dataMsg.Serialize(ref writer);
        }

        public void Deserialize(ref BinaryReader reader)
        {
            msgType = (MessageType)reader.ReadInt32();

            switch (msgType)
            {
                case MessageType.MESSAGE_PLAYER_MOVEMENT:
                    dataMsg = new PlayerMovementMessage();
                    break;
                case MessageType.MESSAGE_FIRE:
                    dataMsg = new MissileMessage();
                    break;
                case MessageType.MESSAGE_MINE:
                    dataMsg = new MineMessage();
                    break;
                case MessageType.UPDATE_PLAYER_MOVEMENT:
                    dataMsg = new UpdatePlayerMovementMessage();
                    break;
                case MessageType.UPDATE_FIRE:
                    dataMsg = new UpdateMissileMessage();
                    break;
                case MessageType.UPDATE_MINE:
                    dataMsg = new UpdateMineMessage();
                    break;
                case MessageType.COLLISIONS_LIST:
                    dataMsg = new CollisionsList();
                    break;
                case MessageType.COLLISIONS_FENCE_MISSILE:
                    dataMsg = new CollisionsFenceMissile();
                    break;
                case MessageType.COLLISIONS_FENCE_SHIP:
                    dataMsg = new CollisionsFenceShip();
                    break;
                case MessageType.COLLISIONS_MISSILE_FENCEPOST:
                    dataMsg = new CollisionsMissileFencePost();
                    break;
                case MessageType.COLLISIONS_MISSILE_SHIP:
                    dataMsg = new CollisionsMissileShip();
                    break;
                case MessageType.COLLISIONS_SHIP_MINE:
                    dataMsg = new CollisionsShipMine();
                    break;
                case MessageType.MISSILE_EVENT:
                    dataMsg = new MissileEvent();
                    break;
                case MessageType.MINE_EVENT:
                    dataMsg = new MineEvent();
                    break;
            }
            dataMsg.Deserialize(ref reader);

        }
    }
}
