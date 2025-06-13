using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class MessageHandler
{

    private static Dictionary<MessageTypes, NetworkMessage> messages = new Dictionary<MessageTypes, NetworkMessage>()
    {
        {MessageTypes.PlayerJoined, new SetPlayerIDMessage() },
    };

    public static void HandleReceiveMessage(MessageTypes message, ref DataStreamReader streamreader)
    {
        messages[message].Decode(ref streamreader);
    }

    public static void HandleSendMessage(MessageTypes messageType, ref DataStreamWriter writer)
    {
        writer.WriteUInt((uint)messageType);
        messages[messageType].Encode(ref writer);
    }

    public static void ReceiveJoinedData(ref DataStreamReader reader)
    {
        messages[MessageTypes.PlayerJoined].Decode(ref reader);
    }
}
