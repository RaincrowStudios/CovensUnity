using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDominionSplash : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_Dominion;
    [SerializeField] private TextMeshProUGUI m_TopWitch;
    [SerializeField] private TextMeshProUGUI m_TopCoven;
    [SerializeField] private Button m_CloseButton;

    public static UIDominionSplash Instance { get; private set; }

    private System.Action m_OnClose;
    private int m_TimerTweenId;

    private void Awake()
    {
        Instance = this;
        m_CloseButton.gameObject.SetActive(false);
        m_CloseButton.onClick.AddListener(Close);
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
    }

    private IEnumerator Start()
    {
        while (PlayerDataManager.playerData == null)
            yield return 0;

        //just hide so it can be shown on the end of the tutorial
        if (FTFManager.InFTF || PlayerDataManager.playerData.insidePlaceOfPower)
        {
            m_Canvas.enabled = false;
            m_InputRaycaster.enabled = false;
            m_CanvasGroup.alpha = 0;
        }
        else
        {
            m_InputRaycaster.enabled = false;
            m_Canvas.enabled = true;
            m_CanvasGroup.alpha = 1;
        }
    }

    public void Show(System.Action onClose)
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        LeanTween.alphaCanvas(m_CanvasGroup, 1, 1f).setEaseOutCubic();

        m_CloseButton.gameObject.SetActive(true);
        m_OnClose = onClose;
        m_Dominion.text = m_TopWitch.text = m_TopCoven.text = "";
        m_Dominion.alpha = m_TopWitch.alpha = m_TopCoven.alpha = 0;
        float time = 2.5f;
        m_Dominion.text = LocalizeLookUp.GetText("dominion_location") + " " + GameStartup.Dominion;
        LeanTween.value(0, 1f, 2f).setOnUpdate((float a) => m_Dominion.alpha = a).setDelay(0.25f).setEaseOutCubic();

        if (string.IsNullOrEmpty(GameStartup.TopPlayer) == false)
        {
            m_TopWitch.gameObject.SetActive(true);
            m_TopWitch.text = LocalizeLookUp.GetText("strongest_witch_dominion") + " " + GameStartup.TopPlayer;
            LeanTween.value(0, 1f, 2f).setOnUpdate((float a) => m_TopWitch.alpha = a).setDelay(1.25f).setEaseOutCubic();
            time++;
        }
        else
        {
            m_TopWitch.gameObject.SetActive(false);
        }

        if (string.IsNullOrEmpty(GameStartup.TopCoven) == false)
        {
            m_TopCoven.gameObject.SetActive(true);
            m_TopCoven.text = LocalizeLookUp.GetText("strongest_coven_dominion") + " " + GameStartup.TopCoven;
            LeanTween.value(0, 1f, 2f).setOnUpdate((float a) => m_TopCoven.alpha = a).setDelay(2.25f).setEaseOutCubic();
            time++;
        }
        else
        {
            m_TopCoven.gameObject.SetActive(false);
        }
        m_TimerTweenId = LeanTween.value(0f, 0f, 0).setDelay(time).setOnStart(Close).uniqueId;
    }

    private void Close()
    {
        LeanTween.cancel(m_TimerTweenId);
        m_InputRaycaster.enabled = false;

        m_OnClose?.Invoke();
        m_OnClose = null;

        LeanTween.alphaCanvas(m_CanvasGroup, 0, 1f).setEaseOutCubic().setOnComplete(() =>
        {
            m_Canvas.enabled = false;
            Destroy(this.gameObject, 10);
        });
    }
}
