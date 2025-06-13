using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public abstract class NetworkMessage
{
    public abstract void Encode(ref DataStreamWriter writer);
    public abstract void Decode(ref DataStreamReader reader);
}
