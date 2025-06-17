using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System;

public class ServerBehaviour : MonoBehaviour
{
    public static ServerBehaviour instance;
    private void Awake()
    {
        instance = this;
    }

    public NetworkDriver networkDriver;
    private NativeList<NetworkConnection> connections;
    public int PlayerCount { get { return connections.Length; } }

    private bool isActive = false;
    private const float keepAliveTickrate = 20f;
    private float lastKeepAlive;

    public Action OnConnectionDropped;

    public void Init(ushort port)
    {
        networkDriver = NetworkDriver.Create();
        var endPoint = NetworkEndpoint.AnyIpv4;
        endPoint.Port = port;
        if (networkDriver.Bind(endPoint) != 0)
        {
            Debug.Log($"Failed to bind to port {endPoint.Port}");
        }
        else
        {
            networkDriver.Listen();
        }

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        KeepAlive();

        networkDriver.ScheduleUpdate().Complete();

        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > keepAliveTickrate)
        {
            lastKeepAlive = Time.time;
            Broadcast(new KeepAliveMessage());
        }
    }

    private void CleanupConnections()
    {
        for (int i = connections.Length - 1; i >= 0; i--)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = networkDriver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Accepted a new connection");
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;
        for (int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = networkDriver.PopEventForConnection(connections[i], out streamReader)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    NetworkUtility.OnDataReceived(streamReader, connections[i], this);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    OnConnectionDropped?.Invoke();
                    ShutDown();
                }
            }
        }
    }

    public void SendToClient(NetworkConnection connection, NetworkMessage message)
    {
        networkDriver.BeginSend(connection, out DataStreamWriter writer);
        message.Serialize(ref writer);
        networkDriver.EndSend(writer);
    }

    public void Broadcast(NetworkMessage message)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                SendToClient(connections[i], message);
            }
        }
    }

    public void ShutDown()
    {
        if (isActive)
        {
            Debug.Log("Server disconnected from network");
            networkDriver.Dispose();
            connections.Dispose();
            isActive = false;
        }
    }
    private void OnDestroy()
    {
        ShutDown();
    }
}
