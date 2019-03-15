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

    private void Awake()
    {
        m_Button.onClick.AddListener(Finish);
    }

    public void Show(string text, Sprite icon, System.Action onClose)
    {
        m_Text.text = text;
        m_Icon.sprite = icon;
        m_OnClose = onClose;

        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        LeanTween.value(0, 0, 0).setDelay(4.55f).setOnStart(Finish);
    }

    public void Finish()
    {
        gameObject.SetActive(false);
        m_OnClose?.Invoke();
        m_OnClose = null;
    }
}
