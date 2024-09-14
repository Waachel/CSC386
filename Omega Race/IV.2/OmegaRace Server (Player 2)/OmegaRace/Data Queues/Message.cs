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
        MINE_EVENT,
        TIME_REPLY,
        TIME_REQUEST
    }

    [Serializable]
    public class Message
    {
        public NetDeliveryMethod deliveryMethod;
        public MessageType msgType;
        public DataMessage dataMsg;
        public int sequenceChannel;


        //Message type is player movement
        public void PopulateMessage(PlayerMovementMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.UnreliableSequenced; //can lose small updates without the movements seeming rough as long as they are in the right sequence
            sequenceChannel = 5;
            msgType = MessageType.MESSAGE_PLAYER_MOVEMENT;
            dataMsg = newMsg;
        }

        //Message type is fire missile event from client
        public void PopulateMessage(MissileEvent newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableUnordered; //want to make sure the fire event is reliably sent but the order isn't quite as important since it doesn't actually fire 
            sequenceChannel = 0;
            msgType = MessageType.MISSILE_EVENT;
            dataMsg = newMsg;
        }

        //Message type is drop mine event from client
        public void PopulateMessage(MineEvent newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableUnordered; //same as missile
            sequenceChannel = 0;
            msgType = MessageType.MINE_EVENT;
            dataMsg = newMsg;
        }

        //Message type is fire
        public void PopulateMessage(MissileMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //need to make sure that both players get the fire message at the same time since it actually fires the missile
            sequenceChannel = 2;
            msgType = MessageType.MESSAGE_FIRE;
            dataMsg = newMsg;
        }

        //Message type is mine
        public void PopulateMessage(MineMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //same as missile
            sequenceChannel = 3;
            msgType = MessageType.MESSAGE_MINE;
            dataMsg = newMsg;
        }

        //Message type is update player
        public void PopulateMessage(UpdatePlayerMovementMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.UnreliableSequenced; //can lose small updates without the movements seeming rough as long as they are in the right sequence
            sequenceChannel = 5;
            msgType = MessageType.UPDATE_PLAYER_MOVEMENT;
            dataMsg = newMsg;
        }

        //Message type is update missile
        public void PopulateMessage(UpdateMissileMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.UnreliableSequenced; //can lose small updates as the direction doesn't change but needs to be received in the right order
            sequenceChannel = 5;
            msgType = MessageType.UPDATE_FIRE;
            dataMsg = newMsg;
        }

        //Message type is update mine
        public void PopulateMessage(UpdateMineMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //mine needs to explode at the same time for both players
            sequenceChannel = 5;
            msgType = MessageType.UPDATE_MINE;
            dataMsg = newMsg;
        }

        //Message type is list of collisions
        public void PopulateMessage(CollisionsList newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //need to make sure both players get all collisions in the correct order
            sequenceChannel = 4;
            msgType = MessageType.COLLISIONS_LIST;
            dataMsg = newMsg;
        }

        //Message type is collisions of fence and missile
        public void PopulateMessage(CollisionsFenceMissile newMsg)
        {
            deliveryMethod = NetDeliveryMethod.UnreliableSequenced; //want to maintain collision order but doesn't impact game play
            sequenceChannel = 6;
            msgType = MessageType.COLLISIONS_FENCE_MISSILE;
            dataMsg = newMsg;
        }

        //Message type is collisions of fence and ship
        public void PopulateMessage(CollisionsFenceShip newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableSequenced; //need to make sure we get them all and in the correct order as ships bounce off fences
            sequenceChannel = 4;
            msgType = MessageType.COLLISIONS_FENCE_SHIP;
            dataMsg = newMsg;
        }

        //Message type is collisions of missile and ship
        public void PopulateMessage(CollisionsMissileShip newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //impacts the game greatly so we need it to be delivered reliably and in order
            sequenceChannel = 1;
            msgType = MessageType.COLLISIONS_MISSILE_SHIP;
            dataMsg = newMsg;
        }

        //Message type is collisions of missile and fence post
        public void PopulateMessage(CollisionsMissileFencePost newMsg)
        {
            deliveryMethod = NetDeliveryMethod.Unreliable;//uncommon and does't really impact the game
            sequenceChannel = 0;
            msgType = MessageType.COLLISIONS_MISSILE_FENCEPOST;
            dataMsg = newMsg;
        }

        //Message type is collisions of ship and mine
        public void PopulateMessage(CollisionsShipMine newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //same as missiles
            sequenceChannel = 1;
            msgType = MessageType.COLLISIONS_SHIP_MINE;
            dataMsg = newMsg;
        }

        //Message type is time reply
        public void PopulateMessage(TimeReplyMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //same as missiles
            sequenceChannel = 4;
            msgType = MessageType.TIME_REPLY;
            dataMsg = newMsg;
        }

        //Message type is time request
        public void PopulateMessage(TimeRequestMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered; //same as missiles
            sequenceChannel = 4;
            msgType = MessageType.TIME_REQUEST;
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
                case MessageType.TIME_REPLY:
                    dataMsg = new TimeReplyMessage();
                    break;
                case MessageType.TIME_REQUEST:
                    dataMsg = new TimeRequestMessage();
                    break;
            }
            dataMsg.Deserialize(ref reader);

        }
    }
}
