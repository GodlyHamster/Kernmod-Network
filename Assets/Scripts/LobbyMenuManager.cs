using TMPro;
using UnityEngine;

public class LobbyMenuManager : MonoBehaviour
{
    public static LobbyMenuManager instance;
    private void Awake()
    {
        instance = this;
    }

    [Header("Menus")]
    [SerializeField]
    private GameObject HostJoinMenu;
    [SerializeField]
    private GameObject LobbyMenu;

    [Header("Networking")]
    [SerializeField]
    private ServerBehaviour server;
    [SerializeField]
    private ClientBehaviour client;

    [SerializeField]
    private TMP_InputField ipInput;

    private void Start()
    {
        SetMenuState(HostingMenuState.HostJoin);
    }

    public void HostLocalGame()
    {
        server.Init(9000);
        client.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Lobby);
    }

    public void JoinLocalGame()
    {
        client.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Lobby);
    }

    public void HostOnlineGame()
    {
        server.Init(9000);
        client.Init("127.0.0.1", 9000);
        SetMenuState(HostingMenuState.Lobby);
    }

    public void JoinOnlineGame()
    {
        client.Init(ipInput.text, 9000);
        SetMenuState(HostingMenuState.Lobby);
    }

    public void CancelConnection()
    {
        server.ShutDown();
        client.ShutDown();
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
                HostJoinMenu.SetActive(true);
                LobbyMenu.SetActive(false);
                break;
            case HostingMenuState.Lobby:
                HostJoinMenu.SetActive(false);
                LobbyMenu.SetActive(true);
                break;
            default:
                HostJoinMenu.SetActive(true);
                LobbyMenu.SetActive(false);
                break;
        }
    }
}

public enum HostingMenuState
{
    HostJoin,
    Lobby
}
