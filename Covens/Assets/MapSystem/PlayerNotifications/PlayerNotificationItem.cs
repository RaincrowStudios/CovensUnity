using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class PlayerNotificationItem : MonoBehaviour
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Button m_Button;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    private System.Action m_OnClose;

    //private int m_TweenId;
    private int m_ScaleTweenId;
    private int m_FadeTweenId;

    private void Awake()
    {
        m_Button.onClick.AddListener(() => Finish(0.2f));
        m_CanvasGroup.alpha = 0;
    }

    public void Show(string text, Sprite icon, System.Action onClose, System.Action onUpdate)
    {
        m_Text.text = text;
        m_Icon.sprite = icon;
        m_OnClose = onClose;

        m_Icon.gameObject.SetActive(icon != null);

        LeanTween.cancel(m_FadeTweenId);

        m_FadeTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.2f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_FadeTweenId = LeanTween.value(0, 0, 3f)
                    .setOnComplete(() => Finish(1f, onUpdate))
                    .setOnUpdate((float t) => onUpdate?.Invoke())
                    .uniqueId;
            })
            .setOnUpdate((float t) => onUpdate?.Invoke())
            .uniqueId;

        Pop();

        gameObject.SetActive(true);
    }

    public void Finish(float time)
    {
        Finish(time, null);
    }

    public void Finish(float time, System.Action onUpdate)
    {
        LeanTween.cancel(m_FadeTweenId);
        LeanTween.cancel(m_ScaleTweenId);

        m_FadeTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, time)
            .setEaseOutCubic()
            .setOnUpdate((float t) => onUpdate?.Invoke())
            .setOnComplete(() => 
            {
                m_CanvasGroup.transform.localScale = Vector3.one;
                gameObject.SetActive(false);
                m_OnClose?.Invoke();
                m_OnClose = null;
            })
            .uniqueId;                
    }

    public void Pop()
    {
        LeanTween.cancel(m_ScaleTweenId);
        m_CanvasGroup.transform.localScale = Vector3.one * 1.1f;
        //m_ScaleTweenId = LeanTween.scale(m_CanvasGroup.gameObject, Vector3.one * 1.1f, 0.1f).setOnComplete(() =>
        //{
            m_ScaleTweenId = LeanTween.scale(m_CanvasGroup.gameObject, Vector3.one, 0.55f).setEaseOutCubic().uniqueId;
        //}).uniqueId;
    }
}
