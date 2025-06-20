using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public static class NetworkUtility
{
    public static void OnDataReceived(DataStreamReader reader, NetworkConnection connection, ServerBehaviour server = null)
    {
        NetworkMessage message = null;
        MessageTypes messageType = reader.ReadMessageType();
        switch (messageType)
        {
            case MessageTypes.KeepAlive: message = new KeepAliveMessage(reader); break;
            case MessageTypes.PlayerJoined: message = new PlayerJoinedMessage(reader); break;
            case MessageTypes.StartGame: message = new StartGameMessage(reader); break;
            case MessageTypes.BoardMove: message = new BoardMoveMessage(reader); break;
            case MessageTypes.GameEnd: message = new EndGameMessage(reader); break;
            default:
            Debug.LogError("Message received had not type");
            break;
        }

        if (server != null)
        {
            message.ReceivedOnServer(connection);
        }
        else
        {
            message.ReceivedOnClient();
        }
    }

    public static Action<NetworkMessage> C_KeepAlive;
    public static Action<NetworkMessage> C_PlayerJoined;
    public static Action<NetworkMessage> C_StartGame;
    public static Action<NetworkMessage> C_BoardMove;
    public static Action<NetworkMessage> C_EndGame;
    public static Action<NetworkMessage, NetworkConnection> S_KeepAlive;
    public static Action<NetworkMessage, NetworkConnection> S_PlayerJoined;
    public static Action<NetworkMessage, NetworkConnection> S_StartGame;
    public static Action<NetworkMessage, NetworkConnection> S_BoardMove;
    public static Action<NetworkMessage, NetworkConnection> S_EndGame;
}
