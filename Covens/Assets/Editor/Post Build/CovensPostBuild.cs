#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class CovensPostBuild : MonoBehaviour
{
    [PostProcessBuild(9999)]
    static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            var plistPath = System.IO.Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Update value
            PlistElementDict rootDict = plist.root;
            rootDict.SetString("NSLocationAlwaysUsageDescription", "Covens uses your real location data in order to play the game.");

            if (rootDict.values.ContainsKey("UIApplicationExitsOnSuspend"))
            {
                Log("removing \"UIApplicationExitsOnSuspend\" from \"" + plistPath + "\"");
                rootDict.values.Remove("UIApplicationExitsOnSuspend");
            }

            // Write plist
            System.IO.File.WriteAllText(plistPath, plist.WriteToString());
        }        
    }

    private static void Log(string msg)
    {
        Debug.Log("[CovensPostBuild] " + msg);
    }
}
#endif
