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
        UnityEditor.EditorPrefs.SetString("Server", "Release");

    }

    [UnityEditor.MenuItem(SetServerRelease, true, 0)]
    public static bool CheckToggleFake()
    {
        UnityEditor.Menu.SetChecked(SetServerRelease, UnityEditor.EditorPrefs.GetString("Server") == "Release");
        return true;
    }




    [UnityEditor.MenuItem(SetServerLocal, false, 0)]
    public static void ServerLocal()
    {
        UnityEditor.EditorPrefs.SetString("Server", "Local");
    }
    [UnityEditor.MenuItem(SetServerLocal, true, 0)]
    public static bool CheckServerLocal()
    {
        UnityEditor.Menu.SetChecked(SetServerLocal, UnityEditor.EditorPrefs.GetString("Server") == "Local");
        return true;
    }




    [UnityEditor.MenuItem(SetServerStaging, false, 0)]
    public static void ServerStaging()
    {
        UnityEditor.EditorPrefs.SetString("Server", "Staging");
    }
    [UnityEditor.MenuItem(SetServerStaging, true, 0)]
    public static bool CheckServerStaging()
    {
        UnityEditor.Menu.SetChecked(SetServerStaging, UnityEditor.EditorPrefs.GetString("Server") == "Staging");

        return true;
    }

}

#endif