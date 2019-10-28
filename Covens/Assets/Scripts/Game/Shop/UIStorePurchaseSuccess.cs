using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;

public class UIStorePurchaseSuccess : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Animator m_Animator;

    [Space()]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Button m_ContinueButton;

    private static UIStorePurchaseSuccess m_Instance;

    private System.Action m_OnClose;
    private int m_AlphaTweenId;

    public static void Show(string title, string subtitle, Sprite icon, System.Action onClose = null)
    {
        if (m_Instance == null)
        {
            Debug.LogError("purchase success not instantiated");
            onClose?.Invoke();
            return;
        }
        m_Instance._Show(title, subtitle, icon, onClose);
    }

    public static void Close()
    {
        if (m_Instance == null)
            return;

        m_Instance._Close();
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;

        m_ContinueButton.onClick.AddListener(_Close);
        m_Animator.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_AlphaTweenId);
    }

    private void _Show(string title, string subtitle, Sprite icon, System.Action onClose)
    {
        m_Title.text = title;
        m_Subtitle.text = subtitle;
        m_Icon.overrideSprite = icon;
        m_OnClose = onClose;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_AlphaTweenId);
        m_Animator.gameObject.SetActive(true);
        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.2f).setEaseOutCubic().uniqueId;
    }

    private void _Close()
    {
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_AlphaTweenId);
        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f).setEaseOutCubic().setOnComplete(() =>
        {
            m_Canvas.enabled = false;
            m_Animator.gameObject.SetActive(false);
        }).uniqueId;
        m_OnClose?.Invoke();
    }

    public static void Show(List<PackItemData> items)
    {
        if (items == null || items.Count == 0)
            return;

        System.Action<int, float> show = (i, delay) => { };
        char gender = PlayerDataManager.playerData.male ? 'm' : 'f';

        show = (i, delay) =>
        {
            if (i >= items.Count)
                return;

            if (delay > 0)
            {
                LeanTween.value(0, 0, delay).setOnComplete(() => show(i, 0));
                return;
            }

            string iconId = items[i].id;
            string title = LocalizeLookUp.GetStoreTitle(items[i].id);
            string subtitle = LocalizeLookUp.GetStoreSubtitle(items[i].id);

            if (items[i].type == StoreManagerAPI.TYPE_COSMETIC)
            {
                CosmeticData data = DownloadedAssets.GetCosmetic(items[i].id);

                if (data.gender[0] != gender)
                {
                    show(i + 1, 0);
                    return;
                }

                iconId = data == null ? items[i].id : data.iconId;
            }
            else if (items[i].type == StoreManagerAPI.TYPE_CURRENCY)
            {
                iconId = items[i].id + "2"; //gold2 and silver2
                subtitle = items[i].amount.ToString();
            }

            LoadingOverlay.Show();
            DownloadedAssets.GetSprite(
                iconId,
                spr =>
                {
                    LoadingOverlay.Hide();
                    Show(title, subtitle, spr, () => show(i + 1, 0.2f));
                },
                true);
        };

        show(0, 0);
    }
}
