using Unity.Collections;
using UnityEngine;

public class ChatMessage : NetworkMessage
{
    public string message;

    public override void Decode(ref DataStreamReader reader)
    {
        var fixedmsg = reader.ReadFixedString64();
        message = fixedmsg.ConvertToString();
    }

    public override void Encode(ref DataStreamWriter writer)
    {
        writer.WriteFixedString64(message);
    }
}
