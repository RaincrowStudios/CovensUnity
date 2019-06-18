using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILocationClaimed : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private Button m_ContinueButton;

    private static UILocationClaimed m_Instance;
    public static UILocationClaimed Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UILocationClaimed>("LocationClaim"));
            return m_Instance;
        }
    }


    private int m_TweenId;

    private void Awake()
    {
        gameObject.SetActive(false);
        m_InputRaycaster.enabled = false;

        m_ContinueButton.onClick.AddListener(OnClickContinue);

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        Destroy(this.gameObject);
    }

    public void Show(string name)
    {
        LeanTween.cancel(m_TweenId);

        m_CanvasGroup.alpha = 0;
        m_InputRaycaster.enabled = true;
        m_TitleText.text = name;

        gameObject.SetActive(true);
    }

    private void OnClickContinue()
    {
        LeanTween.cancel(m_TweenId);
        //m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 1f)
        //    .setOnComplete(() =>
        //    {
        //        gameObject.SetActive(false);
        //    })
        //    .setEaseOutCubic()
        //    .uniqueId;

        gameObject.SetActive(false);
        m_InputRaycaster.enabled = false;
    }
}
