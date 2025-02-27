﻿#if UNITY_IOS
namespace Mapbox.Editor.Build
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Globalization;

	public class Mapbox_iOS_build : MonoBehaviour
	{
		[PostProcessBuild]
		public static void AppendBuildProperty(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				PBXProject proj = new PBXProject();
				// path to pbxproj file
				string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

				var file = File.ReadAllText(projPath);
				proj.ReadFromString(file);
				string target = proj.TargetGuidByName("Unity-iPhone");

				var defaultIncludePath = "Mapbox/Core/Plugins/iOS/MapboxMobileEvents/include";

                string debug = "[Mapbox_iOS_build] includePaths:\n";
				var includePaths = Directory.GetDirectories(Application.dataPath, "include", SearchOption.AllDirectories);
                foreach (string _path in includePaths)
                    debug += "\t" + _path + "\n";
                Debug.Log(debug);

				var includePath = includePaths
					.Select(path => Regex.Replace(path, Application.dataPath + "/", ""))
					.Where(path => path.EndsWith(defaultIncludePath, true, CultureInfo.InvariantCulture))
					.DefaultIfEmpty(defaultIncludePath)
					.First();

                Debug.Log("[Mapbox_iOS_build] include final path: " + includePath);

				proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/" + includePath);
				proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC -lz");

				File.WriteAllText(projPath, proj.WriteToString());
			}
		}
	}
}
#endif