using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

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

    public async void TryLogin()
    {
        if (await ServerLogin() == false) return;
        if (await UserLogin() == false) return;
        LoadMainMenu();
    }
    public async void UploadScore(int score)
    {
        if (await ServerLogin() == false) return;
        if (await UploadScoreRequest(score) == false) return;
    }

    private async Task<bool> ServerLogin()
    {
        //handle server login
        UnityWebRequest serverLoginRequest = UnityWebRequest.Get("https://studenthome.hku.nl/~wouter.vandompselaar/server_login.php?id=1&pw=eu-01");
        await serverLoginRequest.SendWebRequest();

        if (serverLoginRequest.result == UnityWebRequest.Result.Success)
        {
            string serverData = serverLoginRequest.downloadHandler.text;
            if (serverData == "0")
            {
                Debug.Log("Failed to login to server, please try again");
                return false;
            }
            serverSessionID = serverData;
        }
        return true;
    }

    private async Task<bool> UserLogin()
    {
        //handle user login
        string mail = mailText.text;
        string password = passwordText.text;

        UnityWebRequest loginRequest = UnityWebRequest.Get($"https://studenthome.hku.nl/~wouter.vandompselaar/user_login.php?mail={mail}&pw={password}");
        
        await loginRequest.SendWebRequest();

        if (loginRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonData = loginRequest.downloadHandler.text;
            if (jsonData == "0")
            {
                Debug.Log("Failed to login, please try again");
                return false;
            }
            else
            {
                loginData.SetDataFromJsonString(jsonData);
            }
        }
        return true;
    }

    private async Task<bool> UploadScoreRequest(int score)
    {
        //upload score to database
        string playerID = loginData.UserData.id;
        UnityWebRequest scoreUploadRequest = UnityWebRequest.Get($"https://studenthome.hku.nl/~wouter.vandompselaar/insert_score.php?sessid={serverSessionID}&score={score}&user={playerID}");
        await scoreUploadRequest.SendWebRequest();

        if (scoreUploadRequest.result == UnityWebRequest.Result.Success)
        {
            string returnText = scoreUploadRequest.downloadHandler.text;
            Debug.Log(returnText);
            if (returnText == "1")
            {
                Debug.Log("Succesfully uploaded score to database");
                return true;
            }
            else
            {
                Debug.Log("Failed to upload score to database");
            }
        }
        return false;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }
}
