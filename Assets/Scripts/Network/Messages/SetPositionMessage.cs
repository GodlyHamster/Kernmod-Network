using Unity.Collections;
using UnityEngine;

public class SetPositionMessage : NetworkMessage
{
    public int objectID;
    public Vector3 position;

    public override void Decode(ref DataStreamReader reader)
    {
        objectID = reader.ReadInt();
        position = new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
    }

    public override void Encode(ref DataStreamWriter writer)
    {
        writer.WriteInt(objectID);
        writer.WriteFloat(position.x);
        writer.WriteFloat(position.y);
        writer.WriteFloat(position.z);
    }
}
