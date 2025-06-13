using System;
using Unity.Collections;
using UnityEngine;

public static class PacketHandler
{
    public static void ReceiveJoinedMessasge(ref DataStreamReader reader)
    {
        int playerID = reader.ReadInt();
        ClientBehaviour.instance.playerID = playerID;
    }

    public static void SendBoardMove(ref DataStreamWriter writer, Vector2Int position, Shape shape)
    {
        writer.WriteMessageID(MessageTypes.BoardMove);

        writer.WriteVector2Int(position);
        writer.WriteUInt((uint)shape);
    }

    public static void ReceiveBoardMove(ref DataStreamReader reader)
    {
        TicTacToe.instance?.PlacePiece(reader.ReadVector2Int(), (Shape)reader.ReadInt());
    }

    public static class Server
    {
        public static void SendJoinedMessage(ref DataStreamWriter writer, int playerID)
        {
            writer.WriteMessageID(MessageTypes.PlayerJoined);

            writer.WriteInt(playerID);
        }
    }
}
