using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay Instance;
    private static Canvas Canvas;
    private static UnityEngine.UI.GraphicRaycaster InputRaycaster;

    private void Awake()
    {        
        Canvas = GetComponent<Canvas>();
        InputRaycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void _Show()
    {
        if (Canvas.enabled && InputRaycaster.enabled)
            return;

        Debug.Log("Show overlay load");
        Canvas.enabled = true;
        InputRaycaster.enabled = true;
    }

    private void _Hide()
    {
        if (Canvas.enabled == false && InputRaycaster.enabled == false)
            return;

        Debug.Log("Hide overlay load");
        Canvas.enabled = false;
        InputRaycaster.enabled = false;
    }

    public static void Show()
    {
        if (Instance == null)
        {
            return;
        }

        Instance._Show();
    }

    public static void Hide()
    {
        if (Instance == null)
        {
            return;
        }

        Instance._Hide();
    }
}
