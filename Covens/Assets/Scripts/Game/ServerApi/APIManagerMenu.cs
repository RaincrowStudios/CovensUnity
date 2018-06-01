#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// API manager's menu
/// </summary>
public class APIManagerMenu
{
    private const string SetServerRelease = "Raincrow/Server/Set Server Release";
    private const string SetServerLocal = "Raincrow/Server/Set Server Local";
    private const string SetServerFake = "Raincrow/Server/Set Server Fake";



    public static void RemoveDefine(string sDef, UnityEditor.BuildTargetGroup eBuildGroup)
    {
        string sDefs = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(eBuildGroup);
        sDefs = sDefs.Replace(";" + sDef, "");
        sDefs = sDefs.Replace(sDef + ";", "");
        sDefs = sDefs.Replace(sDef, "");
        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(eBuildGroup, sDefs);
    }
    public static void AddDefine(string sDef, UnityEditor.BuildTargetGroup eBuildGroup)
    {
        string sDefs = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(eBuildGroup);
        sDefs = sDefs + ";" + sDef;
        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(eBuildGroup, sDefs);
    }
    public static void AddDefine(string sDef)
    {
        AddDefine(sDef, UnityEditor.BuildTargetGroup.Android);
        AddDefine(sDef, UnityEditor.BuildTargetGroup.iOS);
        AddDefine(sDef, UnityEditor.BuildTargetGroup.Standalone);
    }
    public static void RemoveAll()
    {
        RemoveDefine("SERVER_RELEASE", UnityEditor.BuildTargetGroup.Android);
        RemoveDefine("SERVER_RELEASE", UnityEditor.BuildTargetGroup.iOS);
        RemoveDefine("SERVER_RELEASE", UnityEditor.BuildTargetGroup.Standalone);
        RemoveDefine("SERVER_LOCAL", UnityEditor.BuildTargetGroup.Android);
        RemoveDefine("SERVER_LOCAL", UnityEditor.BuildTargetGroup.iOS);
        RemoveDefine("SERVER_LOCAL", UnityEditor.BuildTargetGroup.Standalone);
        RemoveDefine("SERVER_FAKE", UnityEditor.BuildTargetGroup.Android);
        RemoveDefine("SERVER_FAKE", UnityEditor.BuildTargetGroup.iOS);
        RemoveDefine("SERVER_FAKE", UnityEditor.BuildTargetGroup.Standalone);
    }

    #region SERVER_RELEASE

    [UnityEditor.MenuItem(SetServerRelease, false, 0)]
    public static void SetFake()
    {
        RemoveAll();
        AddDefine("SERVER_RELEASE");
    }

    [UnityEditor.MenuItem(SetServerRelease, true, 0)]
    public static bool CheckToggleFake()
    {
#if SERVER_RELEASE
        UnityEditor.Menu.SetChecked(SetServerRelease, true);
#else
        UnityEditor.Menu.SetChecked(SetServerRelease, false);
#endif
        return true;
    }

    #endregion


    #region SERVER_LOCAL

    [UnityEditor.MenuItem(SetServerLocal, false, 0)]
    public static void ServerLocal()
    {
        RemoveAll();
        AddDefine("SERVER_LOCAL");
    }
    [UnityEditor.MenuItem(SetServerLocal, true, 0)]
    public static bool CheckServerLocal()
    {
#if SERVER_LOCAL
        UnityEditor.Menu.SetChecked(SetServerLocal, true);
#else
        UnityEditor.Menu.SetChecked(SetServerLocal, false);
#endif
        return true;
    }

    #endregion


    #region SERVER_FAKE

    [UnityEditor.MenuItem(SetServerFake, false, 0)]
    public static void ServerFake()
    {
        RemoveAll();
        AddDefine("SERVER_FAKE");
    }
    [UnityEditor.MenuItem(SetServerFake, true, 0)]
    public static bool CheckServerFake()
    {
#if SERVER_FAKE
        UnityEditor.Menu.SetChecked(SetServerFake, true);
#else
        UnityEditor.Menu.SetChecked(SetServerFake, false);
#endif
        return true;
    }

    #endregion
}


#endif