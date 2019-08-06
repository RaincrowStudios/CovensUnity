using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.Maps;

public class UICollectableInfo : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;

    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Rarity;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private Button m_CloseButton;

    [SerializeField] private Sprite[] m_Icons;

    private static UICollectableInfo m_Instance;
    public static UICollectableInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UICollectableInfo>("UICollectableInfo"));
            return m_Instance;
        }
    }

    public static bool IsOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            return m_Instance.m_InputRaycaster.enabled;
        }
    }

    private int m_TweenId;
    public Dictionary<string, Sprite> m_IconDict;
    public string CollectableId { get; private set; }

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_IconDict = new Dictionary<string, Sprite>()
        {
            { "herb", m_Icons[0] },
            { "tool", m_Icons[1] },
            { "gem", m_Icons[2] }
        };
    }

    public void Show(string id)
    {
        CollectableId = id;

        IngredientData data = DownloadedAssets.GetCollectable(id);
        m_Icon.sprite = m_IconDict[data.type];
        m_Title.text = LocalizeLookUp.GetCollectableName(id);
        m_Rarity.text = "Rarity (" + data.rarity + ")";
        m_Description.text = LocalizeLookUp.GetCollectableDesc(id);


        //move the card to the ride side of the screen
        m_Panel.anchorMin = m_Panel.anchorMax = new Vector2(0.735f, 0.5f);
        m_Panel.anchoredPosition = Vector2.zero;

        //the close button only occupies the right half of the screen
        RectTransform closeRect = m_CloseButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.5f, 0);
        closeRect.anchoredPosition = Vector2.zero;
		if (data.forbidden)
            m_Panel.GetChild(3).gameObject.SetActive(true);
        else
            m_Panel.GetChild(3).gameObject.SetActive(false);

        AnimateIn();
    }

    private void AnimateIn()
    {
        m_CanvasGroup.alpha = 0;
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.75f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .uniqueId;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }
    
    public void Close()
    {
        CollectableId = null;

        m_InputRaycaster.enabled = false;
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 0.5f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .uniqueId;
    }   

    private void OnClickClose()
    {
        Close();
    }
}
