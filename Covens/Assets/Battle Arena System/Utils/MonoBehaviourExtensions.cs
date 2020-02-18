using UnityEngine;
using UnityEngine.SceneManagement;

public static class MonoBehaviourExtensions
{
    /// <summary>
    /// Works with objects that do not inherit from UnityEngine.Object
    /// </summary>
    public static T FindObjectOfType<T>(this GameObject gameObject)
    {
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject[] gameObjects = scene.GetRootGameObjects();
            for (int j = 0; j < gameObjects.Length; j++)
            {
                T target = gameObjects[j].GetComponentInChildren<T>();                
                if (target != default)
                {                    
                    return target;
                }
            }
        }
        return default;
    }

    /// <summary>
    /// Works with objects that do not inherit from UnityEngine.Object
    /// </summary>
    public static T FindObjectOfType<T>(this Component component)
    {
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject[] gameObjects = scene.GetRootGameObjects();
            for (int j = 0; j < gameObjects.Length; j++)
            {
                T target = gameObjects[j].GetComponentInChildren<T>();
                if (target != default)
                {
                    return target;
                }
            }
        }
        return default;
    }
}
