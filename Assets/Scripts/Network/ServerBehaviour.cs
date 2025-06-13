using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System;

public class ServerBehaviour : MonoBehaviour
{
    public NetworkDriver networkDriver;
    private NativeList<NetworkConnection> connections;

    public int playersInSession
    {
        get
        {
            return connections.Length;
        }
    }

    void Start()
    {
        networkDriver = NetworkDriver.Create();
        var endPoint = NetworkEndpoint.AnyIpv4;
        endPoint.Port = 9000;
        if (networkDriver.Bind(endPoint) != 0)
        {
            Debug.Log($"Failed to bind to port {endPoint.Port}");
        }
        else
        {
            networkDriver.Listen();
        }

        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    void Update()
    {
        networkDriver.ScheduleUpdate().Complete();

        //Clean connections
        for (int i = connections.Length - 1; i >= 0; i--)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
            }
        }

        //new connections
        NetworkConnection c;
        while ((c = networkDriver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a new connection");

            //Sends player their id
            networkDriver.BeginSend(NetworkPipeline.Null, c, out DataStreamWriter writer);
            int playerID = connections.IndexOf(c);
            PacketHandler.Server.SendJoinedMessage(ref writer, playerID);
            networkDriver.EndSend(writer);
        }

        DataStreamReader streamReader;
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated) continue;

            NetworkEvent.Type cmd;
            while ((cmd = networkDriver.PopEventForConnection(connections[i], out streamReader)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var receivedMessage = streamReader.ReadMessageID();
                    Debug.Log($"Server received {receivedMessage} message from client {i}");
                    SendMessageToAll(receivedMessage);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public void SendMessageToOne(MessageTypes message, int playerId)
    {
        networkDriver.BeginSend(NetworkPipeline.Null, connections[playerId], out DataStreamWriter writer);
        MessageHandler.HandleSendMessage(message, ref writer);
        networkDriver.EndSend(writer);
    }

    public void SendMessageToAll(MessageTypes message)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            networkDriver.BeginSend(NetworkPipeline.Null, connections[i], out DataStreamWriter writer);
            MessageHandler.HandleSendMessage(message, ref writer);
            networkDriver.EndSend(writer);
        }
    }

    private void OnDestroy()
    {
        if (networkDriver.IsCreated)
        {
            networkDriver.Dispose();
            connections.Dispose();
        }
    }
}
