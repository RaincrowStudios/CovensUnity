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
        Canvas.enabled = true;
        InputRaycaster.enabled = true;
    }

    private void _Hide()
    {
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
