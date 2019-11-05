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

#if PRODUCTION        
            Debug.Log("[CovensPreBuild] android scripting backend set to IL2CPP");
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
#endif

#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = iBuildNumber;
#endif

#if UNITY_IOS
            PlayerSettings.iOS.buildNumber = iBuildNumber.ToString();
#endif
    }
}
#endif
