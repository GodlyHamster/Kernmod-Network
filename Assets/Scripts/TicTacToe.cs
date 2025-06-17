using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class TicTacToe : MonoBehaviour
{
    public static TicTacToe instance;

    [SerializeField]
    private int boardSize = 3;

    private Shape[,] board;

    [Header("Debugging")]
    public bool doDebug = false;
    public Vector2Int movePos;
    public Shape shape;

    private Shape assignedShape;

    [ContextMenu("Do Move")]
    public void DebugMove()
    {
        PlacePiece(movePos, shape);
    }

    private void OnEnable()
    {
        NetworkUtility.S_PlayerJoined += OnPlayerJoinServer;
        NetworkUtility.C_PlayerJoined += OnPlayerJoinClient;
    }

    private void OnDisable()
    {
        NetworkUtility.S_PlayerJoined -= OnPlayerJoinServer;
        NetworkUtility.C_PlayerJoined -= OnPlayerJoinClient;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        board = new Shape[boardSize, boardSize];
    }

    public void PlacePiece(Vector2Int pos, Shape shape)
    {
        if (shape == Shape.EMPTY) return;

        //place shape
        if (board[pos.x, pos.y] == Shape.EMPTY)
        {
            board[pos.x, pos.y] = shape;
        }

        //check row win
        for (int i = 0; i < boardSize; i++)
        {
            if (board[i, pos.y] != shape) break;
            if (i == boardSize - 1)
            {
                Debug.Log($"{shape} won!!");
            }
        }
        //check col win
        for (int i = 0; i < boardSize; i++)
        {
            if (board[pos.x, i] != shape) break;
            if (i == boardSize - 1)
            {
                Debug.Log($"{shape} won!!");
            }
        }
        //check diagonal
        for (int i = 0; i < boardSize; i++)
        {
            if (board[i, i] != shape) break;
            if (i == boardSize - 1)
            {
                Debug.Log($"{shape} won!!");
            }
        }
        //check inverted diagonal
        for (int i = 0; i < boardSize; i++)
        {
            if (board[boardSize-1 - i, i] != shape) break;
            if (i == boardSize - 1)
            {
                Debug.Log($"{shape} won!!");
            }
        }
    }

    private void OnPlayerJoinServer(NetworkMessage message, NetworkConnection connection)
    {
        //PlayerJoinedMessage newMessage = message as PlayerJoinedMessage;

        //newMessage.assignedShape = (Shape)ServerBehaviour.instance.PlayerCount;

        //ServerBehaviour.instance.SendToClient(connection, newMessage);
    }
    private void OnPlayerJoinClient(NetworkMessage message)
    {
        //PlayerJoinedMessage newMessage = message as PlayerJoinedMessage;

        //assignedShape = newMessage.assignedShape;
    }

    private void OnDrawGizmos()
    {
        if (!doDebug) return;
        if (board == null) return;

        Color[] colors = new Color[] { Color.black, Color.red, Color.blue };
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                Gizmos.color = colors[(int)board[i, j]];
                Gizmos.DrawCube(new Vector2(i, j), new Vector3(0.9f, 0.9f, 0.0f));
            }
        }
    }
}

public enum Shape
{
    EMPTY = 0,
    CROSS = 1,
    CRICLE = 2
}
