using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class KeepAliveMessage : NetworkMessage
{
    public KeepAliveMessage()
    {
        MessageType = MessageTypes.KeepAlive;
    }
    public KeepAliveMessage(DataStreamReader reader)
    {
        MessageType = MessageTypes.KeepAlive;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteMessageType(MessageType);
    }

    public override void Deserialize(DataStreamReader reader)
    {
    }

    public override void ReceivedOnClient()
    {
        NetworkUtility.C_KeepAlive?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection client)
    {
        NetworkUtility.S_KeepAlive?.Invoke(this, client);
    }
}
