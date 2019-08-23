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

    private void Awake()
    {
        if (FTFManager.InFTF)
        {
            m_Canvas.enabled = false;
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            m_CloseButton.gameObject.SetActive(false);
            m_CloseButton.onClick.AddListener(Close);

            m_InputRaycaster.enabled = true;
            m_Canvas.enabled = true;
            m_CanvasGroup.alpha = 1;
        }
    }

    public void Show(System.Action onClose)
    {
        m_CloseButton.gameObject.SetActive(true);
        m_OnClose = onClose;
        m_Dominion.text = m_TopWitch.text = m_TopCoven.text = "";
        m_Dominion.alpha = m_TopWitch.alpha = m_TopCoven.alpha = 0;
        float time = 2.5f;
        m_Dominion.text = LocalizeLookUp.GetText("dominion_location") + " " + GameStartup.Dominion;
        LeanTween.value(0, 1f, 2f).setOnUpdate((float a) => m_Dominion.alpha = a).setDelay(0.25f).setEaseOutCubic();
        //Debug.Log("Top Player" + GameStartup.TopPlayer);
        //Debug.Log("Top Coven" + GameStartup.TopCoven);
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
        LeanTween.value(0f, 1f, time).setOnComplete(() =>
          {
              Close();
          });
    }

    private void Close()
    {
        m_OnClose?.Invoke();
        m_OnClose = null;
        LeanTween.cancelAll();
        m_InputRaycaster.enabled = false;
        LeanTween.alphaCanvas(m_CanvasGroup, 0, 1f).setEaseOutCubic().setOnComplete(() =>
        {
            m_Canvas.enabled = false;
            Destroy(this.gameObject, 10);
        });
    }
}
