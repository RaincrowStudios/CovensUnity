using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CovensPreBuild
{	
    [MenuItem("Raincrow/Pre Build/Run")]
    public static void Start()
    {
        CleanupWorktree();
        UpdateVersionAndBuildNumber();
        GitCommitChanges();
    }    

    private static void CleanupWorktree()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "git.exe",
            Arguments = "reset --hard",
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using (Process gitProcess = Process.Start(startInfo))
        {
            using (System.IO.StreamReader reader = gitProcess.StandardOutput)
            {
                gitProcess.WaitForExit();
                Debug.LogFormat("{0} {1}", gitProcess.StartInfo.FileName, gitProcess.StartInfo.Arguments);
            }
        }

        startInfo = new ProcessStartInfo
        {
            FileName = "git.exe",
            Arguments = "clean -df",
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using (Process gitProcess = Process.Start(startInfo))
        {
            using (System.IO.StreamReader reader = gitProcess.StandardOutput)
            {
                gitProcess.WaitForExit();
                Debug.LogFormat("{0} {1}", gitProcess.StartInfo.FileName, gitProcess.StartInfo.Arguments);
            }
        }
    }

    private static void UpdateVersionAndBuildNumber()
    {
        try
        {
            // Get branch name
            string branchName = string.Empty;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = "rev-parse --abbrev-ref HEAD",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (Process gitProcess = Process.Start(startInfo))
            {
                using (System.IO.StreamReader reader = gitProcess.StandardOutput)
                {
                    gitProcess.WaitForExit();
                    branchName = reader.ReadToEnd().Trim();
                }
            }

            if (string.IsNullOrEmpty(branchName))
            {
                string exceptionMessage = string.Format("[{0}]: Could not get branch name!", nameof(CovensPreBuild));
                throw new System.ArgumentNullException(nameof(branchName), exceptionMessage);
            }

            // Get number from branch name
            string branchNumberString = string.Join(string.Empty, Regex.Split(branchName, @"\D+"));

            // Get commit count
            int commitCount = 0;
            startInfo = new ProcessStartInfo
            {
                FileName = "git.exe",
                Arguments = string.Concat("rev-list --count ", branchName),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (Process gitProcess = Process.Start(startInfo))
            {
                using (System.IO.StreamReader reader = gitProcess.StandardOutput)
                {
                    gitProcess.WaitForExit();
                    commitCount = int.Parse(reader.ReadToEnd().Trim());
                }
            }

            PlayerSettings.bundleVersion = branchNumberString;
            Debug.LogFormat("App Version: {0}", PlayerSettings.bundleVersion);

            PlayerSettings.Android.bundleVersionCode = commitCount;
            Debug.LogFormat("Android Bundle Version: {0}", PlayerSettings.Android.bundleVersionCode);

            PlayerSettings.iOS.buildNumber = commitCount.ToString();
            Debug.LogFormat("iOS Bundle Version: {0}", PlayerSettings.iOS.buildNumber);

            // This will never be a problem anymore
            PlayerSettings.SplashScreen.show = false;
            Debug.LogFormat("Show Splash Screen: {0}", PlayerSettings.SplashScreen.show ? "Yes" : "No");

            AssetDatabase.SaveAssets();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private static void GitCommitChanges()
    {
        string projectAssetPath = Application.dataPath.Replace("/Assets", "/ProjectSettings/ProjectSettings.asset");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "git.exe",
            Arguments = string.Concat("add ", projectAssetPath),
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using (Process gitProcess = Process.Start(startInfo))
        {
            using (System.IO.StreamReader reader = gitProcess.StandardOutput)
            {
                gitProcess.WaitForExit();
            }
        }

        string commitMessage = string.Format("[New] Update Version {0} - Bundle Version {1} - Build Number {2}", PlayerSettings.bundleVersion, PlayerSettings.Android.bundleVersionCode, PlayerSettings.iOS.buildNumber);

        startInfo = new ProcessStartInfo
        {
            FileName = "git.exe",
            Arguments = string.Format("commit -m \"{0}\"", commitMessage),
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using (Process gitProcess = Process.Start(startInfo))
        {
            using (System.IO.StreamReader reader = gitProcess.StandardOutput)
            {
                gitProcess.WaitForExit();
            }
        }
    }
}
