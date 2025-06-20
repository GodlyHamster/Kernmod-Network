using System;
using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TicTacToe : MonoBehaviour
{
    public static TicTacToe instance;

    [Header("Game")]
    [SerializeField]
    private int boardSize = 3;
    public Sprite crossSprite;
    public Sprite circleSprite;

    [Header("Endscreen")]
    [SerializeField]
    private GameObject endScreenContainer;
    [SerializeField]
    private TextMeshProUGUI endText;

    private Shape[,] board;

    private Shape currentTurn = Shape.CROSS;

    private bool gameEnded = false;

    private void OnEnable()
    {
        NetworkUtility.S_BoardMove += OnBoardMoveServer;
        NetworkUtility.S_EndGame += OnGameEndedServer;

        NetworkUtility.C_BoardMove += OnBoardMoveClient;
        NetworkUtility.C_EndGame += OnGameEndedClient;
    }


    private void OnDisable()
    {
        NetworkUtility.S_BoardMove -= OnBoardMoveServer;
        NetworkUtility.S_EndGame -= OnGameEndedServer;

        NetworkUtility.C_BoardMove += OnBoardMoveClient;
        NetworkUtility.C_EndGame -= OnGameEndedClient;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        endScreenContainer.SetActive(false);
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        board = new Shape[boardSize, boardSize];
    }

    private void OnBoardMoveServer(NetworkMessage message, NetworkConnection connection)
    {
        BoardMoveMessage boardMove = message as BoardMoveMessage;

        if (currentTurn != boardMove.shape) return;

        ServerBehaviour.instance.Broadcast(message);
    }
    private void OnBoardMoveClient(NetworkMessage message)
    {
        BoardMoveMessage boardMove = message as BoardMoveMessage;

        PlacePiece(boardMove.movePos, boardMove.shape);
    }

    public bool PlacePiece(Vector2Int pos, Shape shape)
    {
        if (gameEnded) return false;
        if (currentTurn != shape) return false;
        if (shape == Shape.EMPTY) return false;

        //place shape
        if (board[pos.x, pos.y] == Shape.EMPTY)
        {
            board[pos.x, pos.y] = shape;
            currentTurn = shape == Shape.CROSS ? Shape.CRICLE : Shape.CROSS;
        }

        //check row win
        for (int i = 0; i < boardSize; i++)
        {
            if (board[i, pos.y] != shape) break;
            if (i == boardSize - 1)
            {
                SendWinMessage(shape);
            }
        }
        //check col win
        for (int i = 0; i < boardSize; i++)
        {
            if (board[pos.x, i] != shape) break;
            if (i == boardSize - 1)
            {
                SendWinMessage(shape);
            }
        }
        //check diagonal
        for (int i = 0; i < boardSize; i++)
        {
            if (board[i, i] != shape) break;
            if (i == boardSize - 1)
            {
                SendWinMessage(shape);
            }
        }
        //check inverted diagonal
        for (int i = 0; i < boardSize; i++)
        {
            if (board[boardSize-1 - i, i] != shape) break;
            if (i == boardSize - 1)
            {
                SendWinMessage(shape);
            }
        }
        return true;
    }

    private void SendWinMessage(Shape winningShape)
    {
        gameEnded = true;
        ClientBehaviour.instance.SendToServer(new EndGameMessage() { winningShape = winningShape });
    }
    private void OnGameEndedServer(NetworkMessage message, NetworkConnection connection)
    {
        //broadcast game ending to all players
        ServerBehaviour.instance.Broadcast(message);
    }
    private void OnGameEndedClient(NetworkMessage message)
    {
        EndGameMessage msg = message as EndGameMessage;
        bool didWin = msg.winningShape == Player.instance.assignedShape;

        //upload score and enable end screen
        int score = didWin ? 1 : 0;
        APIManager.instance.UploadScore(score);

        endText.text = didWin ? "You Won!!" : "You lost :(";
        endScreenContainer.SetActive(true);
    }

    public void ReturnToMenu()
    {
        ClientBehaviour.instance.ShutDown();
        ServerBehaviour.instance.ShutDown();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDrawGizmos()
    {
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
