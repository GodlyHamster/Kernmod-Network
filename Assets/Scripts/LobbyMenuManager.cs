using System;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using System.Collections.Generic;

public class LobbyMenuManager : MonoBehaviour
{
    public static LobbyMenuManager instance;
    private void Awake()
    {
        instance = this;
    }

    [Header("Menus")]
    [SerializeField]
    private GameObject HostJoinScreen;
    [SerializeField]
    private GameObject LobbyScreen;
    [SerializeField]
    private GameObject ConnectingScreen;

    [Header("Networking")]
    [SerializeField]
    private ServerBehaviour server;
    [SerializeField]
    private ClientBehaviour client;

    [SerializeField]
    private TMP_InputField ipInput;

    private void OnEnable()
    {
        NetworkUtility.S_PlayerJoined += OnConnectedServer;
        NetworkUtility.C_PlayerJoined += OnConnectedClient;
        client.OnConnectionDropped += QuitLobby;
    }

    private void OnDisable()
    {
        NetworkUtility.S_PlayerJoined -= OnConnectedServer;
        NetworkUtility.C_PlayerJoined -= OnConnectedClient;
        client.OnConnectionDropped -= QuitLobby;
    }

    private void Start()
    {
        SetMenuState(HostingMenuState.HostJoin);
    }

    private void OnConnectedServer(NetworkMessage message, NetworkConnection connection)
    {
        server.SendToClient(connection, message);
    }

    private void OnConnectedClient(NetworkMessage message)
    {
        SetMenuState(HostingMenuState.Lobby);
    }

    public void HostLocalGame()
    {
        server.Init(9000);
        client.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void JoinLocalGame()
    {
        client.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void HostOnlineGame()
    {
        server.Init(9000);
        client.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void JoinOnlineGame()
    {
        client.Init(ipInput.text, 9000);
        SetMenuState(HostingMenuState.Connecting);
    }

    public void CancelConnection()
    {
        client.ShutDown();
        server.ShutDown();
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
