using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.GameEventResponses;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup), typeof(GraphicRaycaster))]
public class UILootRewards : MonoBehaviour
{
    [SerializeField] private Image m_Portrait;
    [SerializeField] private TextMeshProUGUI m_BossName;
    [SerializeField] private TextMeshProUGUI m_ChestTier;
    [SerializeField] private Button m_Continue;
    [Space]
    [SerializeField] private GameObject m_LeftArrow;
    [SerializeField] private GameObject m_RightArrow;
    [Space]
    [SerializeField] private ScrollRect m_Scroll;
    [SerializeField] private HorizontalLayoutGroup m_ItemContainer;
    [SerializeField] private UILootRewardItem m_ItemPrefab;

    private CanvasGroup m_CanvasGroup;
    private GraphicRaycaster m_InputRaycaster;
    private RectTransform m_RectTransform;
    private RectTransform m_ContainerRectTransform;

    public static UILootRewards Instantiate()
    {
        return Instantiate(Resources.Load<UILootRewards>("LootRewardsPopup"));
    }

    private void Awake()
    {
        m_CanvasGroup = this.GetComponent<CanvasGroup>();
        m_InputRaycaster = this.GetComponent<GraphicRaycaster>();
        m_RectTransform = this.GetComponent<RectTransform>();
        m_ContainerRectTransform = m_ItemContainer.GetComponent<RectTransform>();

        m_CanvasGroup.alpha = 0;
        m_LeftArrow.SetActive(false);
        m_RightArrow.SetActive(false);
        m_ItemPrefab.gameObject.SetActive(false);
        m_ItemPrefab.transform.SetParent(this.transform);
        m_InputRaycaster.enabled = true;

        m_Continue.onClick.AddListener(OnClickContinue);
        m_Scroll.onValueChanged.AddListener(OnScroll);
    }

    public void Show(CollectLootHandler.EventData data)
    {
        m_Continue.interactable = false;
        LeanTween.value(0, 0, 0).setDelay(0.5f).setOnStart(() => m_Continue.interactable = true);
        LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f).setEaseOutCubic();

        //m_Portrait.
        DownloadedAssets.GetSprite(data.bossId + "_portrait", m_Portrait);
        m_BossName.text = LocalizeLookUp.GetSpiritName(data.bossId);
        m_ChestTier.text = LocalizeLookUp.GetText(data.type);

        StartCoroutine(ShowRewards(data));
    }

    private IEnumerator ShowRewards(CollectLootHandler.EventData data)
    {
        float delay = 1f;
        if (data.xp > 0)
        {
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupExp(data.xp);
        }

        if (data.silver > 0)
        {
            yield return WaitOrTap(delay);
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupSilver(data.silver);
        }

        if (data.gold > 0)
        {
            yield return WaitOrTap(delay);
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupGold(data.gold);
        }

        if (data.collectibles?.Length > 0)
        {
            yield return WaitOrTap(delay);
            int total = 0;
            foreach (var ingredient in data.collectibles)
                total += ingredient.amount;
            Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupIngredients(total);
        }

        if (data.cosmetics != null)
        {
            foreach (string id in data.cosmetics)
            {
                yield return WaitOrTap(delay);
                Instantiate(m_ItemPrefab, m_ItemContainer.transform).SetupCosmetic(id);
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
        LeanTween.alphaCanvas(m_CanvasGroup, 0, 1f).setEaseOutCubic().setOnComplete(() => Destroy(this.gameObject));
    }

    private void OnClickContinue()
    {
        Close();
    }

    private void OnScroll(Vector2 data)
    {
        float pos = -m_Scroll.content.anchoredPosition.x;
        float left = m_ItemContainer.padding.left + 345;
        float right = m_ItemContainer.padding.left + (345 + m_ItemContainer.spacing) * (m_ItemContainer.transform.childCount - 1) - m_ItemContainer.padding.right - left;
        m_LeftArrow.SetActive(pos > left);
        m_RightArrow.SetActive(pos < right);
    }
}
