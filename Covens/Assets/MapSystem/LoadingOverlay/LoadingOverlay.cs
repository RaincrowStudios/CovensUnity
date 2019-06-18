using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay Instance;
    private static Canvas Canvas;
    private static UnityEngine.UI.GraphicRaycaster InputRaycaster;

    public static void Show()
    {
        if (Instance == null)
        {
            Instantiate();
        }

        Canvas.enabled = true;
        InputRaycaster.enabled = true;
    }

    public static void Hide()
    {
        if (Instance == null)
        {
            Instantiate();
        }

        Canvas.enabled = false;
        InputRaycaster.enabled = false;
    }

    private static void Instantiate()
    {
        LoadingOverlay prefab = Resources.Load<LoadingOverlay>("LoadingOverlay");
        Instance = Instantiate(prefab);
        Canvas = Instance.GetComponent<Canvas>();
        InputRaycaster = Instance.GetComponent<UnityEngine.UI.GraphicRaycaster>();
    }

    protected virtual void OnApplicationQuit()
    {
        if (Instance != null)
        {
            DestroyImmediate(Instance);
        }        
    }
}
