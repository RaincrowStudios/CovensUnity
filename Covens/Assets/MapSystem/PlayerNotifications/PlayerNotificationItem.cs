using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class PlayerNotificationItem : MonoBehaviour
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Button m_Button;

    private System.Action m_OnClose;

    private int m_TweenId;
    private int m_ScaleTweenId;

    public bool isShowing { get; private set; }

    private void Awake()
    {
        m_Button.onClick.AddListener(Finish);
    }

    public void Show(string text, Sprite icon, System.Action onClose)
    {
        m_Text.text = text;
        m_Icon.sprite = icon;
        m_OnClose = onClose;

        m_Icon.gameObject.SetActive(icon != null);

        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        transform.localScale = Vector3.one;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 0, 4.55f).setOnComplete(Finish).uniqueId;

        isShowing = true;
        gameObject.SetActive(true);
    }

    public void Finish()
    {
        isShowing = false;

        LeanTween.cancel(m_TweenId);
        gameObject.SetActive(false);
        m_OnClose?.Invoke();
        m_OnClose = null;
    }

    public void Pop()
    {
        LeanTween.cancel(m_ScaleTweenId);
        transform.GetChild(0).localScale = Vector3.one * 1.2f;
        m_ScaleTweenId = LeanTween.scale(transform.GetChild(0).gameObject, Vector3.one, 1f).setEaseOutCubic().uniqueId;
    }
}
