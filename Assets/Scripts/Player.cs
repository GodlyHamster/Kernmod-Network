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
            //send random move
        }
    }
}
