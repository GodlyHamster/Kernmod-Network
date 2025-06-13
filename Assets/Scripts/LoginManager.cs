using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public static LoginManager instance;

    [SerializeField]
    private TMP_InputField mailText;
    [SerializeField]
    private TMP_InputField passwordText;

    [SerializeField]
    private string mainMenuScene;

    private string serverSessionID;
    private UserLoginData loginData = new UserLoginData();
    public UserLoginData LoginData { get { return loginData; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void TryLogin()
    {
        StartCoroutine(LoginRequest());
    }

    private IEnumerator LoginRequest()
    {
        //handle server login
        UnityWebRequest serverLoginRequest = UnityWebRequest.Get("https://studenthome.hku.nl/~wouter.vandompselaar/server_login.php?id=1&pw=eu-01");
        yield return serverLoginRequest.SendWebRequest();

        if (serverLoginRequest.result == UnityWebRequest.Result.Success)
        {
            string serverData = serverLoginRequest.downloadHandler.text;
            if (serverData == "0")
            {
                Debug.Log("Failed to login to server, please try again");
                yield return null;
            }
            serverSessionID = serverData;
        }

        //handle user login
        string mail = mailText.text;
        string password = passwordText.text;

        UnityWebRequest loginRequest = UnityWebRequest.Get($"https://studenthome.hku.nl/~wouter.vandompselaar/user_login.php?mail={mail}&pw={password}");
        
        yield return loginRequest.SendWebRequest();

        if (loginRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonData = loginRequest.downloadHandler.text;
            if (jsonData == "0")
            {
                Debug.Log("Failed to login, please try again");
                yield return null;
            }
            else
            {
                loginData.SetDataFromJsonString(jsonData);
                LoadMainMenu();
            }
        }
        yield return null;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }
}
