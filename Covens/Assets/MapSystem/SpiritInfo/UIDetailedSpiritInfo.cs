using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDetailedSpiritInfo : MonoBehaviour {

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_DisplayName;
    [SerializeField] private TextMeshProUGUI m_Tier;
    [SerializeField] private TextMeshProUGUI m_Location;
    [SerializeField] private TextMeshProUGUI m_Lore;
    [SerializeField] private TextMeshProUGUI m_Behavior;

    [Header("Images")]
    [SerializeField] private Image m_SpiritArt;
    [SerializeField] private Image m_TierIcon;

    [Header("Buttons")]
    [SerializeField] private Button m_CloseButton;

    private static UIDetailedSpiritInfo m_Instance;
    public static UIDetailedSpiritInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIDetailedSpiritInfo>("UIDetailedSpiritInfo"));
            return m_Instance;
        }
    }

    private int m_TweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;

        m_CloseButton.onClick.AddListener(() =>
        {
            Close();
        });
    }

    public void Show(SpiritData spirit, Token token)
    {
        m_DisplayName.text = spirit.Name;

        if (spirit.tier == 1)
            m_Tier.text = "Lesser Spirit";
        else if (spirit.tier == 2)
            m_Tier.text = "Greater Spirit";
        else if (spirit.tier == 3)
            m_Tier.text = "Superior Spirit";
        else
            m_Tier.text = "Legendary Spirit";

        m_TierIcon.sprite = MarkerSpawner.GetSpiritTierSprite(token.spiritType);
        m_Location.text = spirit.Location;
        m_Behavior.text = "<color=white>Behavior:</color> " + spirit.Behavior;
        m_Lore.text = "<color=white>Lore:</color> " + spirit.Description;
        
        m_SpiritArt.color = new Color(0, 0, 0, 0);
        DownloadedAssets.GetSprite(token.spiritId, spr =>
        {
            m_SpiritArt.overrideSprite = spr;
            LeanTween.color(m_SpiritArt.rectTransform, Color.white, 1f).setEaseOutCubic();
        });


        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f)
            .setEaseOutCubic()
            .uniqueId;
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            }).uniqueId;
    }
}
