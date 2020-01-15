using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.GameEventResponses;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup), typeof(Canvas))]
public class UIRewardsPopup : MonoBehaviour
{
    [Header("UIRewardsPopup")]
    [SerializeField] private Button m_Continue;
    [Space]
    [SerializeField] private GameObject m_LeftArrow;
    [SerializeField] private GameObject m_RightArrow;
    [Space]
    [SerializeField] private ScrollRect m_Scroll;
    [SerializeField] private HorizontalLayoutGroup m_ItemContainer;
    [SerializeField] private UIRewardsPopupItem m_ItemPrefab;

    private Canvas m_Canvas;
    private CanvasGroup m_CanvasGroup;
    private GraphicRaycaster m_InputRaycaster;
    private RectTransform m_RectTransform;
    private RectTransform m_ContainerRectTransform;
        
    private void Awake()
    {
        m_CanvasGroup = this.GetComponent<CanvasGroup>();
        m_Canvas = this.GetComponent<Canvas>();
        m_InputRaycaster = this.GetComponent<GraphicRaycaster>();
        m_RectTransform = this.GetComponent<RectTransform>();
        m_ContainerRectTransform = m_ItemContainer.GetComponent<RectTransform>();

        m_CanvasGroup.alpha = 0;
        m_LeftArrow.SetActive(false);
        m_RightArrow.SetActive(false);
        m_ItemPrefab.gameObject.SetActive(false);
        m_ItemPrefab.transform.SetParent(this.transform);
        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;

        m_Continue.onClick.AddListener(OnClickContinue);
        m_Scroll.onValueChanged.AddListener(OnScroll);

        m_Scroll.horizontalNormalizedPosition = 0.5f;
    }

    public virtual void Show(CollectibleData[] ingredients, string[] cosmetics, int gold, int silver, ulong xp, int power, int resilience)
    {
        m_Continue.interactable = false;
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.value(0, 0, 0).setDelay(1f).setOnComplete(() => m_Continue.interactable = true);
        LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f).setEaseOutCubic();
        
        StartCoroutine(ShowRewards(ingredients, cosmetics, gold, silver, xp, power, resilience));
    }

    private IEnumerator ShowRewards(CollectibleData[] ingredients, string[] cosmetics, int gold, int silver, ulong xp, int power, int resilience)
    {
        float delay = 0.5f;
        if (xp > 0)
        {
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupExp(xp);
            yield return WaitOrTap(delay);
        }

        if (silver > 0)
        {
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupSilver(silver);
            yield return WaitOrTap(delay);
        }

        if (gold > 0)
        {
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupGold(gold);
            yield return WaitOrTap(delay);
        }

        if (ingredients?.Length > 0)
        {
            int total = 0;
            foreach (var ingredient in ingredients)
                total += ingredient.amount;
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupIngredients(total);
            yield return WaitOrTap(delay);
        }

        char gender = PlayerDataManager.playerData.male ? 'm' : 'f';
        if (cosmetics != null)
        {
            foreach (string id in cosmetics)
            {
                if (DownloadedAssets.GetCosmetic(id).gender[0] != gender)
                    continue;

                Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupCosmetic(id);
                yield return WaitOrTap(delay);
            }
        }
    }

    private IEnumerator WaitOrTap(float time)
    {
        float counter = 0;
        float hold = 0;
        bool tap = false;

        while (counter < time && !tap)
        {
            counter += Time.unscaledDeltaTime;

            if (Input.GetMouseButton(0))
            {
                hold += Time.unscaledDeltaTime;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (hold > 0.1f)
                    hold = 0;
                else
                    tap = true;
            }

            yield return null;
        }
    }

    public void Close()
    {
        StopAllCoroutines();

        m_InputRaycaster.enabled = false;

        LeanTween.alphaCanvas(m_CanvasGroup, 0, .5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                OnClose();
                Destroy(this.gameObject);
            });
    }

    private void OnClickContinue()
    {
        Close();
    }

    private void OnScroll(Vector2 data)
    {
        float width = m_ContainerRectTransform.sizeDelta.x;
        float pos = -m_Scroll.content.anchoredPosition.x + width/2f;
        float left = m_ItemContainer.padding.left + 345;
        float right = width - m_ItemContainer.padding.right - 345;//m_ItemContainer.padding.left + (345 + m_ItemContainer.spacing) * (m_ItemContainer.transform.childCount - 1) - m_ItemContainer.padding.right - left;

        m_LeftArrow.SetActive(pos > left);
        m_RightArrow.SetActive(pos < right);
    }

    protected virtual void OnClose(){ }
}
