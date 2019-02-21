using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private Texture2D m_Screenshot;
    
    public void TakeScreenshot(int width, int height, System.Action<Texture2D> callback)
    {
        if(m_Camera == null)
        {
            Debug.LogError("[CameraScreenshot] camera not set.");
            return;
        }

        StartCoroutine(TakeScreenshotCoroutine(width, height, callback));
    }

    private IEnumerator TakeScreenshotCoroutine(int width, int height, System.Action<Texture2D> callback)
    {
        yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(width, height, 24);
        m_Camera.targetTexture = rt;
        m_Screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        m_Camera.Render();
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        m_Screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);//new Rect(Screen.width/2f - width/2f, Screen.height/2f - height/2f, width, height), 0, 0);
        m_Screenshot.Apply();
        RenderTexture.active = prev;
        m_Camera.targetTexture = null;

        callback?.Invoke(m_Screenshot);
    }
}
