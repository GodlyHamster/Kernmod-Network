using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public static ClientBehaviour instance;

    public NetworkDriver networkDriver;
    public NetworkConnection connection;

    public int playerID;

    private DataStreamWriter writer;
    public delegate void Packet(ref DataStreamReader reader);
    private Dictionary<MessageTypes, Packet> messages = new Dictionary<MessageTypes, Packet>()
    {
        {MessageTypes.PlayerJoined, PacketHandler.ReceiveJoinedMessasge },
        {MessageTypes.BoardMove, PacketHandler.ReceiveBoardMove },
    };

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        networkDriver = NetworkDriver.Create();
        connection = default(NetworkConnection);

        var endPoint = NetworkEndpoint.LoopbackIpv4;
        endPoint.Port = 9000;
        connection = networkDriver.Connect(endPoint);
    }

    void Update()
    {
        networkDriver.ScheduleUpdate().Complete();

        if (!connection.IsCreated)
        {
            return;
        }

        DataStreamReader streamReader;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(networkDriver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Client is connected to the server");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var receivedMessage = streamReader.ReadMessageID();
                messages[receivedMessage]?.Invoke(ref streamReader);

                Debug.Log($"Client{playerID} received {receivedMessage} message from server");
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from the server");
                connection = default(NetworkConnection);
            }
        }
    }

    public DataStreamWriter StartSend()
    {
        networkDriver.BeginSend(NetworkPipeline.Null, connection, out writer);
        return writer;
    }
    public void EndSend()
    {
        networkDriver.EndSend(writer);
    }

    private void OnDestroy()
    {
        networkDriver.Dispose();
    }
}
