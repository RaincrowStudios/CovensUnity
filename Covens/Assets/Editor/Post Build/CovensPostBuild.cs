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
            PlistElementArray rootArray = rootDict.CreateArray("NSLocationAlwaysUsageDescription");
            rootArray.AddString("Covens uses your real location data in order to play the game.");

            // Write plist
            System.IO.File.WriteAllText(plistPath, plist.WriteToString());
        }        
    }
}
#endif
