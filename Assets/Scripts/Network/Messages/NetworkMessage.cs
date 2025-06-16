using Unity.Collections;
using Unity.Networking.Transport;

public class NetworkMessage
{
    public MessageTypes MessageType { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteMessageType(MessageType);
    }
    public virtual void Deserialize(DataStreamReader reader)
    {

    }

    public virtual void ReceivedOnClient()
    {

    }
    public virtual void ReceivedOnServer(NetworkConnection client) 
    {

    }
}
