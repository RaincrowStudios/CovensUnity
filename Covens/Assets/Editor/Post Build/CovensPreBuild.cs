#if UNITY_CLOUD_BUILD
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class CovensPreBuild : MonoBehaviour
{
		public static void PreBuildUCB(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            int iBuildNumber = 0;
            int.TryParse(manifest.GetValue<string>("buildNumber"), out iBuildNumber);
            iBuildNumber += 1050;
            Debug.Log("[CovensPreBuild] buildNumber: " + iBuildNumber);
#if UNITY_ANDROID
            PlayerSettings.Android.bundleVersionCode = iBuildNumber;
#elif UNITY_IOS
            PlayerSettings.iOS.buildNumber = iBuildNumber.ToString();
#endif
        }
}
#endif
