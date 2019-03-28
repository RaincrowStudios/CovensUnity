using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class CovensPostBuild : MonoBehaviour
{
    static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        
        var plistPath = System.IO.Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Update value
        PlistElementDict rootDict = plist.root;
        rootDict.SetString("NSLocationAlwaysUsageDescription", "Covens uses your real location data in order to play the game.");

        // Write plist
        System.IO.File.WriteAllText(plistPath, plist.WriteToString());
    }
}
