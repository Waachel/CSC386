using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Lidgren.Network;
using System.Net;
using System.IO;
using OmegaRace;

namespace AzulNetworkBase
{
    public class NetworkManager
    {
        NetServer server;
        public NetworkManager(int serverPort)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Connection Test");
            config.AcceptIncomingConnections = true;
            config.MaximumConnections = 100;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = serverPort;

            //simulating packet loss and latency
            //config.SimulatedLoss = 0.25f;
            //config.SimulatedMinimumLatency = 0.1f;
            //config.SimulatedRandomLatency = 0.01f;

            server = new NetServer(config);
            server.Start();
        }

        public void ProcessIncoming(GameScenePlay game)
        {
            NetIncomingMessage im;

            while ((im = server.ReadMessage()) != null)
            {
                // First, the LIdgren-type of message we received
                switch (im.MessageType)
                {
                    //**********************************
                    // A server replied to out discovery request
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage om = server.CreateMessage();
                        om.Write("Connecting server");
                        server.SendDiscoveryResponse(om, im.SenderEndPoint);
                        break;
                    // Connection status to server has changed
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        string reason = im.ReadString();
                        Debug.WriteLine("Connection status changed: " + status.ToString() + ": " + reason);
                        break;

                    // A client is sending application-related data
                    case NetIncomingMessageType.Data:
                        //Debug.WriteLine("Data message received");
                        game.MessageFromClient(im);
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

                server.Recycle(im);
            }
        }

        public void SendMessage(byte[] msgarray, NetDeliveryMethod deliveryMethod, int sequenceNum)
        {
            NetOutgoingMessage om = server.CreateMessage();
            om.Write(msgarray);

            if(deliveryMethod == NetDeliveryMethod.Unknown)
            {
                deliveryMethod = NetDeliveryMethod.ReliableSequenced;
            }

            if (server.ConnectionsCount > 0)
                server.SendMessage(om, server.Connections[0], deliveryMethod, sequenceNum);
        }
    }
}