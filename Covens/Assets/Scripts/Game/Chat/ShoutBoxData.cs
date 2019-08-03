using UnityEngine;
using TMPro;
using Raincrow.Maps;

public class ShoutBoxData : MonoBehaviour
{
    // public TextMeshPro Title;
    public TextMeshPro Content;

    private System.Action m_OnClose;
    private float m_ClickTime;
    private bool m_Closing;

    public void Setup(IMarker marker, string title, string content, System.Action onClose)
    {
        m_Closing = false;
        m_OnClose = onClose;

        // Title.text = title;
        Content.text = content;

        transform.localScale = Vector3.zero;
        transform.rotation = marker.AvatarTransform.rotation;

        LeanTween.scale(gameObject, Vector3.one, 0.55f).setEaseOutCubic();
        LeanTween.value(0, 1, 5).setOnComplete(() => Close());
    }

    private void OnMouseDown()
    {
        m_ClickTime = Time.time;
    }

    private void OnMouseUp()
    {
        if (Time.time - m_ClickTime < 0.06f)
        {
            Close();
        }
    }

    private void Close()
    {
        if (m_Closing)
            return;

        m_Closing = true;

        LeanTween.scale(gameObject, Vector3.zero, 0.3f)
            .setEaseOutCubic()
            .setOnComplete(m_OnClose);
    }
}

