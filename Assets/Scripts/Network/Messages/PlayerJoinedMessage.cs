using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class PlayerJoinedMessage : NetworkMessage
{
    public Shape assignedShape = Shape.EMPTY;

    public PlayerJoinedMessage()
    {
        MessageType = MessageTypes.PlayerJoined;
    }
    public PlayerJoinedMessage(DataStreamReader reader)
    {
        MessageType = MessageTypes.PlayerJoined;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteMessageType(MessageType);
        writer.WriteByte((byte)assignedShape);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        assignedShape = (Shape)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        NetworkUtility.C_PlayerJoined?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection client)
    {
        NetworkUtility.S_PlayerJoined?.Invoke(this, client);
    }
}
