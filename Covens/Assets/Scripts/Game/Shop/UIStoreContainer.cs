using Raincrow.Store;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIStoreContainer : MonoBehaviour
{
    public enum Filter
    {
        NONE = 0,
        clothing,
        accessories,
        skinart,
        hairstyles,
    }

    //[SerializeField] private ScrollRect m_ScrollView;
    [SerializeField] private Transform m_Container;
    [SerializeField] private UIStoreItemGroup m_ItemPrefab;
    [SerializeField] private RectTransform m_BottomBar;

    private SimplePool<UIStoreItemGroup> m_ItemPool;
    private Filter m_CurrentFilter;
    private UIStore.Screen m_Category;

    public CanvasGroup canvasGroup => this.GetComponent<CanvasGroup>();

    public RectTransform rectTransform => this.GetComponent<RectTransform>();

    public float alpha
    {
        get => canvasGroup.alpha;
        set => canvasGroup.alpha = value;
    }

    private void Awake()
    {
        alpha = 0;
        m_ItemPool = new SimplePool<UIStoreItemGroup>(m_ItemPrefab, 50);
    }

    private void SetBottomText(params string[] title)
    {
        for (int i = 0; i < m_BottomBar.childCount; i++)
        {
            if (i < title.Length)
            {
                m_BottomBar.GetChild(i).GetComponent<TextMeshProUGUI>().text = title[i];
                m_BottomBar.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                m_BottomBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void SetBottomButtons(params UnityEngine.Events.UnityAction[] onClick)
    {
        for (int i = 0; i < m_BottomBar.childCount; i++)
        {
            int auxI = i;

            if (i < onClick.Length)
            {
                Button button = m_BottomBar.GetChild(i).GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    for (int j = 0; j < m_BottomBar.childCount; j++)
                    {
                        TextMeshProUGUI t = m_BottomBar.GetChild(j).GetComponent<TextMeshProUGUI>();
                        if (j == auxI)
                        {
                            t.fontStyle = FontStyles.Underline;
                            t.color = Color.white;
                        }
                        else
                        {
                            t.fontStyle = FontStyles.Normal;
                            t.color = Color.white * 0.64f;
                        }
                    }
                    onClick[auxI]?.Invoke();
                });
                m_BottomBar.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                m_BottomBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void SetupCosmetics(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.COSMETICS)
            return;

        m_Category = UIStore.Screen.COSMETICS;

        SetBottomText(
            LocalizeLookUp.GetText("store_gear_clothing"),
            LocalizeLookUp.GetText("store_gear_accessories"),
            LocalizeLookUp.GetText("store_gear_skin_art"),
            LocalizeLookUp.GetText("store_gear_hairstyle"));

        List<CosmeticData> cosmetics = new List<CosmeticData>();
        foreach (StoreItem item in items)
            cosmetics.Add(DownloadedAssets.GetCosmetic(item.id));

        SetBottomButtons(
            () => OnClickFilter(Filter.clothing, items, cosmetics),
            () => OnClickFilter(Filter.accessories, items, cosmetics),
            () => OnClickFilter(Filter.skinart, items, cosmetics),
            () => OnClickFilter(Filter.hairstyles, items, cosmetics));

        if (m_CurrentFilter == Filter.NONE)
            SetFilter(Filter.clothing, items, cosmetics);
        else
            SetFilter(m_CurrentFilter, items, cosmetics);
    }

    private void OnClickFilter(Filter filter, List<StoreItem> items, List<CosmeticData> cosmetics)
    {
        if (m_CurrentFilter == filter)
            return;
        SetFilter(filter, items, cosmetics);
    }

    private void SetFilter(Filter filter, List<StoreItem> items, List<CosmeticData> cosmetics)
    {
        m_CurrentFilter = filter;
        StopAllCoroutines();
        StartCoroutine(SpawnCosmeticsCoroutine(filter, items, cosmetics));
    }

    private IEnumerator SpawnCosmeticsCoroutine(Filter filter, List<StoreItem> items, List<CosmeticData> cosmetics)
    {
        //despawn items previously on ui
        m_ItemPool.DespawnAll();

        yield return 0;

        int count = 0;
        List<(UIStoreItem, string)> iconList = new List<(UIStoreItem, string)>();
        UIStoreItemGroup group = m_ItemPool.Spawn(m_Container);
        group.OnSpawn();

        char gender = PlayerDataManager.playerData.male ? 'm' : 'f';
        
        //spawn and setup the items
        for (int i = 0; i < items.Count; i++)
        {
            if (cosmetics[i].type[0] != gender)
                continue;

            if (cosmetics[i].storeCatagory != filter.ToString())
                continue;

            UIStoreItem item = group.GetItem();
            if (item == null)
            {
                group = m_ItemPool.Spawn(m_Container);
                group.OnSpawn();
                item = group.GetItem();
            }

            item.Setup(items[i], cosmetics[i]);
            iconList.Add((item, cosmetics[i].iconId));

            count++;
            if (count >= 5)
            {
                count = 0;
                yield return 0;
            }
        }

        //load the art assets
        for (int i = 0; i < iconList.Count; i++)
        {
            iconList[i].Item1.LoadIcon(iconList[i].Item2);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetupCurrency(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.CURRENCY)
            return;
        m_Category = UIStore.Screen.CURRENCY;

        List<CurrencyBundleData> bundles = new List<CurrencyBundleData>();
        foreach (StoreItem item in items)
            bundles.Add(StoreManagerAPI.GetSilverBundle(item.id));

        SetBottomButtons();
        StopAllCoroutines();
        StartCoroutine(SpawnCurrenciesCoroutine(items, bundles));
    }

    private IEnumerator SpawnCurrenciesCoroutine(List<StoreItem> items, List<CurrencyBundleData> bundles)
    {
        m_ItemPool.DespawnAll();
        yield return 0;

        int count = 0;
        List<(UIStoreItem, string)> iconList = new List<(UIStoreItem, string)>();
        UIStoreItemGroup group = m_ItemPool.Spawn(m_Container);
        group.OnSpawn();

        for (int i = 0; i < items.Count; i++)
        {
            UIStoreItem item = group.GetItem();
            if (item == null)
            {
                group = m_ItemPool.Spawn(m_Container);
                group.OnSpawn();
                item = group.GetItem();
            }

            item.Setup(items[i], bundles[i]);
            //iconList.Add((item, bundles[i].iconId));

            count++;
            if (count >= 5)
            {
                count = 0;
                yield return 0;
            }
        }
    }

    public void SetupIngredients(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.INGREDIENTS)
            return;
        m_Category = UIStore.Screen.INGREDIENTS;

        SetBottomButtons();
        StopAllCoroutines();
        StartCoroutine(SpawnIngredientsCoroutine(items));
    }

    private IEnumerator SpawnIngredientsCoroutine(List<StoreItem> items)
    {
        m_ItemPool.DespawnAll();
        yield return 0;
    }

    public void SetupCharms(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.CHARMS)
            return;
        m_Category = UIStore.Screen.CHARMS;

        SetBottomButtons();
        StopAllCoroutines();
        StartCoroutine(SpawnCharmsCoroutine(items));
    }

    private IEnumerator SpawnCharmsCoroutine(List<StoreItem> items)
    {
        m_ItemPool.DespawnAll();
        yield return 0;
    }
}
