using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Extensions
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
                if (target != null)
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
                if (target != null)
                {
                    return target;
                }
            }
        }
        return default;
    }

    public static void AddRange<T>(this IList<T> source, IEnumerable<T> newList)
    {
        if (source == null)
        {
            throw new System.ArgumentNullException(nameof(source));
        }

        if (newList == null)
        {
            throw new System.ArgumentNullException(nameof(newList));
        }

        if (source is List<T> concreteList)
        {
            concreteList.AddRange(newList);
            return;
        }

        foreach (var element in newList)
        {
            source.Add(element);
        }
    }
}
