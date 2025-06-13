using Unity.Collections;

public class SetPlayerIDMessage : NetworkMessage
{
    public int playerID;

    public override void Decode(ref DataStreamReader reader)
    {
        playerID = reader.ReadInt();
    }

    public override void Encode(ref DataStreamWriter writer)
    {
        writer.WriteInt(playerID);
    }
}
