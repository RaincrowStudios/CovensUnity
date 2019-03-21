
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CreateAssetBundle
{	
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {                
        string assetBundlePath = string.Concat(Application.dataPath, "/AssetBundles");
        bool isDirectoryEmpty = false;
        if (!Directory.Exists(assetBundlePath))
        {
	        Directory.CreateDirectory(assetBundlePath);
            isDirectoryEmpty = true;
        }
        else if (!IsDirectoryEmpty(assetBundlePath))
        {
            string dialog = string.Format("{0} directory is not empty. Delete all files?", assetBundlePath);
            if (EditorUtility.DisplayDialog("Warning!", dialog, "Yes", "No"))
            {
                DirectoryInfo di = new DirectoryInfo(assetBundlePath);
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }

                isDirectoryEmpty = true;
            }
        }
        else
        {
            isDirectoryEmpty = true;
        }
        
        if (isDirectoryEmpty)
        {
            BuildPipeline.BuildAssetBundles(assetBundlePath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        else
        {
            EditorUtility.DisplayDialog("Error!", "Directory is not empty!", "Ok");
        }
    }

    static bool IsDirectoryEmpty(string path)
    {
        IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
        using (IEnumerator<string> en = items.GetEnumerator())
        {
            return !en.MoveNext();
        }
    }
}