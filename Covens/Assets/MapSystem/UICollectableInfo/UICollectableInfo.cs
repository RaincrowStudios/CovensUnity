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
    [SerializeField] private Button m_CollectButton;
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
            if (m_Instance == false)
                return false;
            else
                return m_Instance.m_Canvas.enabled;
        }
    }

    private int m_TweenId;
    private IMarker m_Marker;
    private Token m_MarkerData;
    private MarkerDataDetail m_Details;
    private Dictionary<string, Sprite> m_IconDict;

    public Token token { get { return m_MarkerData; } }

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CollectButton.onClick.AddListener(() => { OnClickCollect(m_MarkerData); });
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_IconDict = new Dictionary<string, Sprite>()
        {
            { "herb", m_Icons[0] },
            { "tool", m_Icons[1] },
            { "gem", m_Icons[2] }
        };
    }

    public void Show(IMarker marker, Token data)
    {
        m_Marker = marker;
        m_MarkerData = data;



        m_Icon.sprite = m_IconDict[data.type];
        m_Title.text = "Loading...";
        m_Description.text = "";
        m_Rarity.text = "";

        //Debug.Log ("maybe..?");
        OnClickCollect(m_MarkerData);

        //*m_CollectButton.interactable = false;
        //*m_CollectButton.gameObject.SetActive(true);

        //*m_Panel.anchorMin = m_Panel.anchorMax = new Vector2(0.5f, 0.5f);
        //*m_Panel.anchoredPosition = Vector2.zero;
        //the close button only occupies the whole screen
        //*RectTransform closeRect = m_CloseButton.GetComponent<RectTransform>();
        //*closeRect.anchorMin = new Vector2(0f, 0);
        //*closeRect.anchoredPosition = Vector2.zero;

        //*AnimateIn();
    }

    public void Show(IngredientDict data)
    {
        m_Icon.sprite = m_IconDict[data.type];
        m_Title.text = data.name;
        m_Rarity.text = "Rarity (" + data.rarity + ")";
        m_Description.text = data.description;

        m_CollectButton.interactable = false;
        m_CollectButton.gameObject.SetActive(false);


        //move the card to the ride side of the screen
        m_Panel.anchorMin = m_Panel.anchorMax = new Vector2(0.735f, 0.5f);
        m_Panel.anchoredPosition = Vector2.zero;

        //the close button only occupies the right half of the screen
        RectTransform closeRect = m_CloseButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.5f, 0);
        closeRect.anchoredPosition = Vector2.zero;
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

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        IngredientDict collectable = DownloadedAssets.GetCollectable(details.id);
        if (collectable == null)
        {
            Close();
            return;
        }

        m_Title.text = collectable.name;
        m_Rarity.text = "Rarity (" + collectable.rarity + ")";
        m_Description.text = collectable.description;
        m_CollectButton.interactable = true;
    }

    private void Close()
    {
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

    private void OnClickCollect(Token token)
    {
        m_CollectButton.interactable = false;
        PickUpCollectibleAPI.pickUp(token.instance, res =>
        {
            if (res == null)
            {
                string msg = "Failed to collect the item.";
                PlayerNotificationManager.Instance.ShowNotification(msg, m_IconDict[token.type]);
            }
            else
            {
                IngredientDict ingr = DownloadedAssets.GetIngredient(res.id);
                string msg = "Added " + res.count.ToString() + " " + (ingr == null ? "ingredient" : ingr.name) + " to the inventory";
                PlayerNotificationManager.Instance.ShowNotification(msg, m_IconDict[token.type]);
                SoundManagerOneShot.Instance.PlayItemAdded();
            }
            //Close();
        });
    }

    private void OnClickClose()
    {
        Close();
    }
}