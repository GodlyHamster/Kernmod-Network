using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class EndGameMessage : NetworkMessage
{
    public Shape winningShape = Shape.EMPTY;

    public EndGameMessage()
    {
        MessageType = MessageTypes.GameEnd;
    }
    public EndGameMessage(DataStreamReader reader)
    {
        MessageType = MessageTypes.GameEnd;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteMessageType(MessageType);
        writer.WriteByte((byte)winningShape);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        winningShape = (Shape)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        NetworkUtility.C_EndGame?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection client)
    {
        NetworkUtility.S_EndGame?.Invoke(this, client);
    }
}
