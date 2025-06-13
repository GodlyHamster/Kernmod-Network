using Unity.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private ClientBehaviour client;

    [SerializeField]
    private Shape assignedShape;

    private int objectId;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DataStreamWriter writer = client.StartSend();
            PacketHandler.SendBoardMove(ref writer, new Vector2Int(0, 0), assignedShape);
            client.EndSend();
        }
    }
}
