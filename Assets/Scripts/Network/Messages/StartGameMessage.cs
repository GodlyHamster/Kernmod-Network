using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class StartGameMessage : NetworkMessage
{
    public StartGameMessage()
    {
        MessageType = MessageTypes.StartGame;
    }
    public StartGameMessage(DataStreamReader reader)
    {
        MessageType = MessageTypes.StartGame;
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
        NetworkUtility.C_StartGame?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection client)
    {
        NetworkUtility.S_StartGame?.Invoke(this, client);
    }
}
