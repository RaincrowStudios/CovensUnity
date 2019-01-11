using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKytelerButton : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Text m_RingName;
    [SerializeField] private Button m_Button;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private GameObject m_Selected;

    [Header("Grayscale")]
    private Material m_Material;
    private int m_GrayscaleProperty;

    private KytelerData m_Data;
    private KytelerItem m_KnownInfo;

    private System.Action m_OnClick;
    private System.Action m_OnClickClose;

    private void Awake()
    {
        m_Selected.SetActive(false);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(KytelerData data, KytelerItem info, System.Action onClick, System.Action onClickClose)
    {
        m_Data = data;
        m_KnownInfo = info;
        m_OnClick = onClick;
        m_OnClickClose = onClickClose;

        m_RingName.text = m_Data.id;

        if (info != null)
        {
            SetOwned(info.ownerName == PlayerDataManager.playerData.displayName);
        }
        
        try
        {
            DownloadedAssets.GetSprite(data.iconId, m_Icon, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError(data.iconId + "\n" + e.Message + "\n" + e.StackTrace);
        }
    }

    private void OnClick()
    {
        if (m_OnClick != null)
            m_OnClick.Invoke();
    }

    private void OnClickClose()
    {
        if (m_OnClickClose != null)
            m_OnClickClose.Invoke();
    }

    public void SetSelected(bool selected)
    {
        m_Selected.SetActive(selected);
    }

    public void SetOwned(bool owned)
    {
        SetGrayscale(owned ? 0 : 1);
    }

    public void SetEquiped(bool equiped)
    {
        m_CloseButton.gameObject.SetActive(equiped);
    }

    public void SetGrayscale(float value)//, float duration, LeanTweenType easeType = LeanTweenType.easeOutCubic)
    {
        if(m_Material == null)
        {
            m_Material = Instantiate(m_Icon.material);
            m_Icon.material = m_Material;
            m_GrayscaleProperty = Shader.PropertyToID("_EffectAmount");
        }
        m_Material.SetFloat(m_GrayscaleProperty, value);
    }
}
