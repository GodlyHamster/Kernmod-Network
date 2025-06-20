using System;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbyMenuManager : MonoBehaviour
{
    public static LobbyMenuManager instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField]
    private string GameScene;

    [Header("Menus")]
    [SerializeField]
    private GameObject HostJoinScreen;
    [SerializeField]
    private GameObject LobbyScreen;
    [SerializeField]
    private GameObject ConnectingScreen;

    [SerializeField]
    private TMP_InputField ipInput;

    private int currentPlayers = 0;

    private void OnEnable()
    {
        NetworkUtility.S_PlayerJoined += OnConnectedServer;
        NetworkUtility.C_PlayerJoined += OnConnectedClient;
        NetworkUtility.C_StartGame += OnStartGameClient;
        ClientBehaviour.instance.OnConnectionDropped += QuitLobby;
    }

    private void OnDisable()
    {
        NetworkUtility.S_PlayerJoined -= OnConnectedServer;
        NetworkUtility.C_PlayerJoined -= OnConnectedClient;
        ClientBehaviour.instance.OnConnectionDropped -= QuitLobby;
    }

    private void Start()
    {
        SetMenuState(HostingMenuState.HostJoin);
    }

    private void OnConnectedServer(NetworkMessage message, NetworkConnection connection)
    {

        PlayerJoinedMessage newMessage = message as PlayerJoinedMessage;

        currentPlayers++;
        newMessage.assignedShape = (Shape)currentPlayers;

        ServerBehaviour.instance.SendToClient(connection, message);

        //start when lobby is full
        if (currentPlayers == 2)
        {
            ServerBehaviour.instance.Broadcast(new StartGameMessage());
        }
    }

    private void OnConnectedClient(NetworkMessage message)
    {
        SetMenuState(HostingMenuState.Lobby);
    }

    private void OnStartGameClient(NetworkMessage message)
    {
        SceneManager.LoadScene(GameScene);
    }

    public void HostLocalGame()
    {
        ServerBehaviour.instance.Init(9000);
        ClientBehaviour.instance.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void JoinLocalGame()
    {
        ClientBehaviour.instance.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void HostOnlineGame()
    {
        ServerBehaviour.instance.Init(9000);
        ClientBehaviour.instance.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void JoinOnlineGame()
    {
        ClientBehaviour.instance.Init(ipInput.text, 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void CancelConnection()
    {
        ClientBehaviour.instance.ShutDown();
        ServerBehaviour.instance.ShutDown();
    }

    public void QuitLobby()
    {
        CancelConnection();
        SetMenuState(HostingMenuState.HostJoin);
    }

    private void SetMenuState(HostingMenuState menuState)
    {
        switch (menuState)
        {
            case HostingMenuState.HostJoin:
                HostJoinScreen.SetActive(true);
                LobbyScreen.SetActive(false);
                ConnectingScreen.SetActive(false);
                break;
            case HostingMenuState.Lobby:
                HostJoinScreen.SetActive(false);
                LobbyScreen.SetActive(true);
                ConnectingScreen.SetActive(false);
                break;
            case HostingMenuState.Connecting:
                HostJoinScreen.SetActive(false);
                LobbyScreen.SetActive(false);
                ConnectingScreen.SetActive(true);
                break;
            default:
                HostJoinScreen.SetActive(true);
                LobbyScreen.SetActive(false);
                ConnectingScreen.SetActive(false);
                break;
        }
    }
}

public enum HostingMenuState
{
    HostJoin,
    Lobby,
    Connecting
}
