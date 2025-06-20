using TMPro;
using UnityEngine;

public class LoginTextRenderer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI userText;

    private void Start()
    {
        userText.text =  "Logged in as: " + APIManager.instance?.LoginData.UserData.nickname;
    }
}
