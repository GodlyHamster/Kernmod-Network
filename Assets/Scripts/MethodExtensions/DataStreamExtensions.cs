using Unity.Collections;
using UnityEngine;

public static class DataStreamExtensions
{
    public static void WriteMessageID(this ref DataStreamWriter writer, MessageTypes message)
    {
        writer.WriteUInt((uint)message);
    }
    public static MessageTypes ReadMessageID(this ref DataStreamReader reader)
    {
        return (MessageTypes)reader.ReadUInt();
    }

    public static void WriteVector2Int(this ref DataStreamWriter writer, Vector2Int vec)
    {
        writer.WriteInt(vec.x);
        writer.WriteInt(vec.y);
    }
    public static Vector2Int ReadVector2Int(this ref DataStreamReader reader)
    {
        return new Vector2Int(reader.ReadInt(), reader.ReadInt());
    }

    public static void WriteVector3(this ref DataStreamWriter writer, Vector3 vec)
    {
        writer.WriteFloat(vec.x);
        writer.WriteFloat(vec.y);
        writer.WriteFloat(vec.z);
    }

    public static Vector3 ReadVector3(this ref DataStreamReader reader)
    {
        return new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
    }
}
