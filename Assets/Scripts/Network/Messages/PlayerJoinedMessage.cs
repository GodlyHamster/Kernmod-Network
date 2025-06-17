using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class PlayerJoinedMessage : NetworkMessage
{
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
    }

    public override void Deserialize(DataStreamReader reader)
    {
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
