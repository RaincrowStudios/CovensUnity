using UnityEngine;
using System.Collections;
using UnityEditor;

public class Scenes : MonoBehaviour {

[MenuItem("Scenes/Main Scene")]
	static void MainScene()
	{
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/MainScene.unity");
	}


	[MenuItem("Scenes/Start Scene")]
	static void StartScene()
	{
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/StartScene.unity");
	}
}
