using UnityEngine;
using System.Collections;
using UnityEditor;

public class Scenes : MonoBehaviour
{

    [MenuItem("Scenes/Main Scene")]
    static void MainScene()
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene("Assets/Scenes/MainScene.unity");
    }

    [MenuItem("Scenes/Main Scene Reduced")]
    static void MainSceneReduced()
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene("Assets/Scenes/MainScene-Reduced.unity");
    }


    [MenuItem("Scenes/Start Scene")]
    static void StartScene()
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene("Assets/Scenes/StartScene.unity");
    }

    [MenuItem("Tools/Play")]
    static void PlayTest()
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene("Assets/Scenes/StartScene.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Tools/Add Localize %#e")]
    static void AddLocalizeLookUp()
    {
        GameObject obj = Selection.activeGameObject;
        obj.AddComponent<LocalizeLookUp>();
    }

}
