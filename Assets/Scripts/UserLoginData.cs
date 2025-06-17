using System;
using UnityEngine;

public class UserLoginData
{
    private UserData userData;
    public UserData UserData { get { return userData; } }

    public void SetDataFromJsonString(string json)
    {
        userData = JsonUtility.FromJson<UserData>(json);
    }
}

[Serializable]
public class UserData
{
    public string id;
    public string email;
    public string nickname = "UnnamedPlayer";
}
