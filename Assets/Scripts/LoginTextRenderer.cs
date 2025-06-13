using TMPro;
using UnityEngine;

public class LoginTextRenderer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI userText;

    private void Start()
    {
        userText.text =  "Logged in as: " + LoginManager.instance?.LoginData.UserData.nickname;
    }
}
