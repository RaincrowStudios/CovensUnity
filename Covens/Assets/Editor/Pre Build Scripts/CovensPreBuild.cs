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
            PlayerSettings.Android.bundleVersionCode = commitCount;
            PlayerSettings.iOS.buildNumber = commitCount.ToString();

            // This will never be a problem anymore
            PlayerSettings.SplashScreen.show = false;            
        }        
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }
}
