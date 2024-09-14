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
        MESSAGE_MINE
    }

    [Serializable]
    public class Message
    {
        public NetDeliveryMethod deliveryMethod;
        public MessageType msgType;
        public DataMessage dataMsg;


        //Message type is player movement
        public void populateMessage(PlayerMovementMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MESSAGE_PLAYER_MOVEMENT;
            dataMsg = newMsg;
        }

        //Message type is fire
        public void populateMessage(FireMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MESSAGE_FIRE;
            dataMsg = newMsg;
        }

        //Message type is mine
        public void populateMessage(MineMessage newMsg)
        {
            deliveryMethod = NetDeliveryMethod.ReliableOrdered;
            msgType = MessageType.MESSAGE_MINE;
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
                    dataMsg = new FireMessage();
                    break;
                case MessageType.MESSAGE_MINE:
                    dataMsg = new MineMessage();
                    break;
            }
            dataMsg.Deserialize(ref reader);

        }
    }
}
