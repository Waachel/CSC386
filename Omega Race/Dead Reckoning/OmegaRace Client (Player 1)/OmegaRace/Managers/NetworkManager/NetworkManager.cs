using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Lidgren.Network;
using System.Net;
using System.IO;
using OmegaRace.Data_Queues;

namespace OmegaRace.Managers.NetworkManager
{
    public class NetworkManager
    {
        NetClient client;
        public NetworkManager(string ipOrHost, int serverPort)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Connection Test");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);


            //simulating packet loss and latency
            //config.SimulatedLoss = 0.25f;
            //config.SimulatedMinimumLatency = 0.1f;
            //config.SimulatedRandomLatency = 0.01f;

            client = new NetClient(config);
            client.Start();

            

            // a bit rough but ok for this demo
            IPEndPoint ep = NetUtility.Resolve(ipOrHost, serverPort);
            client.Connect(ep);
        }

        public void ProcessIncoming(GameScenePlay game)
        {
            NetIncomingMessage im;

            while ((im = client.ReadMessage()) != null)
            {
                // First, the LIdgren-type of message we received
                switch (im.MessageType)
                {
                    //**********************************
                    // A server replied to out discovery request
                    case NetIncomingMessageType.DiscoveryResponse:
                        Debug.WriteLine("Found server at " + im.SenderEndPoint + " name: " + im.ReadString());
                        client.Connect(im.SenderEndPoint);
                        break;

                    // Connection status to serverhas changed
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        string reason = im.ReadString();
                        Debug.WriteLine("Connection status changed: " + status.ToString() + ": " + reason);
                        
                        //Sync time
                        if (status == NetConnectionStatus.Connected)
                        {
                            TimeRequestMessage timeMsg = GameSceneCollection.ScenePlay.MsgMgr.GetTimeRequestMessage();
                            timeMsg.ClientTime = TimeManager.GetCurrentTime();
                            Message tMsg = new Message();
                            tMsg.PopulateMessage(timeMsg);
                            GameSceneCollection.ScenePlay.MessageToServer(tMsg);
                            GameSceneCollection.ScenePlay.MsgMgr.ReleaseTimeRequestMessage(timeMsg);
                        }
                        
                        break;

                    // A client is sending application-related data
                    case NetIncomingMessageType.Data:
                        game.MessageFromServer(im);
                        break;

                    //*****************************************

                    // These are other Lidgren status messages that we likely shouldn't have to deal with
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.UnconnectedData:
                        Debug.WriteLine("Received from " + im.SenderEndPoint + ": " + im.ReadString());
                        break;
                }

                client.Recycle(im);
            }
        }

        public void SendMessage(byte[] msgarray, NetDeliveryMethod deliveryMethod, int sequenceNum)
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write(msgarray);

            if(deliveryMethod == NetDeliveryMethod.Unknown)
            {
                deliveryMethod = NetDeliveryMethod.ReliableSequenced;
            }

            client.SendMessage(om, client.Connections[0], deliveryMethod, sequenceNum);
        }
    }
}
