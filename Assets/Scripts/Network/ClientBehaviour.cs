using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public static ClientBehaviour instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public NetworkDriver networkDriver;
    public NetworkConnection connection;

    private bool isActive = false;

    public Action OnConnectionDropped;


    public void Init(string ip, ushort port)
    {
        networkDriver = NetworkDriver.Create();

        var endPoint = NetworkEndpoint.Parse(ip, port);

        connection = networkDriver.Connect(endPoint);

        isActive = true;

        RegisterEvent();
    }

    void Update()
    {
        if (!isActive) return;

        networkDriver.ScheduleUpdate().Complete();

        CheckAlive();
        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            Debug.Log("Lost connection to the server");
            ShutDown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;
        NetworkEvent.Type cmd;

        while ((cmd = connection.PopEvent(networkDriver, out streamReader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Connected to the server!");
                SendToServer(new PlayerJoinedMessage());
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                NetworkUtility.OnDataReceived(streamReader, connection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Disconnected from the server");
                connection = default(NetworkConnection);
                ShutDown();
            }
        }
    }

    public void SendToServer(NetworkMessage message)
    {
        networkDriver.BeginSend(connection, out DataStreamWriter writer);
        message.Serialize(ref writer);
        networkDriver.EndSend(writer);
    }

    private void RegisterEvent()
    {
        NetworkUtility.C_KeepAlive += KeepAlive;
    }
    private void UnregisterEvent()
    {
        NetworkUtility.C_KeepAlive -= KeepAlive;
    }
    private void KeepAlive(NetworkMessage message)
    {
        SendToServer(message);
    }

    public void ShutDown()
    {
        if (isActive)
        {
            Debug.Log("Client disconnected from network");
            OnConnectionDropped?.Invoke();
            UnregisterEvent();
            networkDriver.Dispose();
            connection = default(NetworkConnection);
            isActive = false;
        }
    }

    private void OnDestroy()
    {
        ShutDown();
    }
}
