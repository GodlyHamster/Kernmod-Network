using System;
using Unity.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Shape assignedShape = Shape.EMPTY;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        NetworkUtility.C_PlayerJoined += OnJoined;
    }

    private void OnDisable()
    {
        NetworkUtility.C_PlayerJoined -= OnJoined;
    }

    private void OnJoined(NetworkMessage message)
    {
        PlayerJoinedMessage newMessage = message as PlayerJoinedMessage;

        assignedShape = newMessage.assignedShape;
    }
}
