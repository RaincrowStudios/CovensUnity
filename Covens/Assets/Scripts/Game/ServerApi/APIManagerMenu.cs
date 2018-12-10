#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// API manager's menu
/// </summary>
public class APIManagerMenu
{
    private const string SetServerRelease = "Server/Set Server Release";
    private const string SetServerLocal = "Server/Set Server Local";
    private const string SetServerStaging = "Server/Set Server Staging";




    [UnityEditor.MenuItem(SetServerRelease, false, 0)]
    public static void SetFake()
    {
        PlayerPrefs.SetString("Server", "Release");

    }

    [UnityEditor.MenuItem(SetServerRelease, true, 0)]
    public static bool CheckToggleFake()
    {
        UnityEditor.Menu.SetChecked(SetServerRelease, PlayerPrefs.GetString("Server") == "Release");
        return true;
    }




    [UnityEditor.MenuItem(SetServerLocal, false, 0)]
    public static void ServerLocal()
    {
        PlayerPrefs.SetString("Server", "Local");
    }
    [UnityEditor.MenuItem(SetServerLocal, true, 0)]
    public static bool CheckServerLocal()
    {
        UnityEditor.Menu.SetChecked(SetServerLocal, PlayerPrefs.GetString("Server") == "Local");
        return true;
    }




    [UnityEditor.MenuItem(SetServerStaging, false, 0)]
    public static void ServerStaging()
    {
        PlayerPrefs.SetString("Server", "Staging");
    }
    [UnityEditor.MenuItem(SetServerStaging, true, 0)]
    public static bool CheckServerStaging()
    {
        UnityEditor.Menu.SetChecked(SetServerStaging, PlayerPrefs.GetString("Server") == "Staging");

        return true;
    }

}

#endif