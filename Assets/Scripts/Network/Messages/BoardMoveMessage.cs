using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class BoardMoveMessage : NetworkMessage
{
    public Vector2Int movePos;
    public Shape shape;

    public BoardMoveMessage()
    {
        MessageType = MessageTypes.BoardMove;
    }
    public BoardMoveMessage(DataStreamReader reader)
    {
        MessageType = MessageTypes.BoardMove;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteMessageType(MessageType);
        writer.WriteVector2Int(movePos);
        writer.WriteByte((byte)shape);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        movePos = reader.ReadVector2Int();
        shape = (Shape)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        NetworkUtility.C_BoardMove?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection client)
    {
        NetworkUtility.S_BoardMove?.Invoke(this, client);
    }
}
