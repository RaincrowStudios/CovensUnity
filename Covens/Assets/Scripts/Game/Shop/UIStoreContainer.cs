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

    [SerializeField] private ScrollRect m_Scroll;
    [SerializeField] private RectTransform m_Canvas;
    [SerializeField] private RectTransform m_Container;
    [SerializeField] private UIStoreItemGroup m_ItemPrefab;
    [SerializeField] private RectTransform m_BottomBar;

    private SimplePool<UIStoreItemGroup> m_ItemPool;
    public Filter m_CurrentFilter;
    private UIStore.Screen m_Category;
    private delegate bool StoreItemValidDelegate(object item);

    public CanvasGroup canvasGroup => this.GetComponent<CanvasGroup>();

    public RectTransform rectTransform => this.GetComponent<RectTransform>();

    public float alpha
    {
        get => canvasGroup.alpha;
        set => canvasGroup.alpha = value;
    }

    public static float Width { get; private set; }

    private void Awake()
    {
        alpha = 0;
        m_ItemPool = new SimplePool<UIStoreItemGroup>(m_ItemPrefab, 50);
        Width = m_Canvas.sizeDelta.x;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        m_ItemPool.DestroyAll();
    }

    public void SetupCosmetics(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.COSMETICS)
        {
            //make sure all the icons load in case the spawn coroutine was interrupted
            UIStoreItemGroup[] groups = GetComponentsInChildren<UIStoreItemGroup>();
            foreach (var group in groups)
                group.enabled = true;
            return;
        }

        m_Category = UIStore.Screen.COSMETICS;

        SetBottomText(
            LocalizeLookUp.GetText("store_gear_clothing"),
            LocalizeLookUp.GetText("store_gear_accessories"),
            LocalizeLookUp.GetText("store_gear_skin_art"),
            LocalizeLookUp.GetText("store_gear_hairstyle"));

        List<object> cosmetics = new List<object>();
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

    public void SetupCurrency(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.CURRENCY)
            return;
        m_Category = UIStore.Screen.CURRENCY;

        List<object> bundles = new List<object>();
        foreach (StoreItem item in items)
            bundles.Add(StoreManagerAPI.GetCurrencyBundle(item.id));

        SetBottomButtons();
        StopAllCoroutines();
        StartCoroutine(SpawnItemsCoroutine(Filter.NONE, items, bundles));
    }

    public void SetupIngredients(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.INGREDIENTS)
            return;
        m_Category = UIStore.Screen.INGREDIENTS;

        List<object> ingredientBundles = new List<object>();
        foreach (StoreItem item in items)
            ingredientBundles.Add(StoreManagerAPI.GetIngredientBundle(item.id));

        SetBottomButtons();
        StopAllCoroutines();
        StartCoroutine(SpawnItemsCoroutine(Filter.NONE, items, ingredientBundles));
    }

    public void SetupCharms(List<StoreItem> items)
    {
        if (m_Category == UIStore.Screen.CHARMS)
            return;
        m_Category = UIStore.Screen.CHARMS;

        List<object> elixirs = new List<object>();
        foreach (StoreItem item in items)
            elixirs.Add(StoreManagerAPI.GetConsumable(item.id));

        SetBottomButtons();
        StopAllCoroutines();
        StartCoroutine(SpawnItemsCoroutine(Filter.NONE, items, elixirs));
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

    private void SetBottomButtons(params UnityEngine.Events.UnityAction[] onClick)
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

    private void OnClickFilter(Filter filter, List<StoreItem> items, List<object> cosmetics)
    {
        if (m_CurrentFilter == filter)
            return;
        SetFilter(filter, items, cosmetics);
    }

    public void SetFilter(Filter filter, List<StoreItem> items, List<object> cosmetics)
    {
        m_CurrentFilter = filter;
        StopAllCoroutines();
        StartCoroutine(SpawnItemsCoroutine(filter, items, cosmetics));
    }

    private IEnumerator SpawnItemsCoroutine(Filter filter, List<StoreItem> items, List<object> data)
    {
        LoadingOverlay.Show();

        //despawn items previously on ui
        m_ItemPool.DespawnAll();
        m_Scroll.horizontalNormalizedPosition = 0;

        yield return 0;
        
        StoreItemValidDelegate isItemValid = (item) => true;

        //cosmetic store item validator
        if (data[0] is CosmeticData)
        {
            isItemValid = (item) =>
            {
                char gender = PlayerDataManager.playerData.male ? 'm' : 'f';
                CosmeticData cosmetic = item as CosmeticData;

                if (cosmetic.type[0] != gender)
                    return false;

                if (cosmetic.storeCatagory != filter.ToString())
                    return false;

                return true;
            };
        }

        bool singleRow = items.Count < 6;
        int count = 0;
        m_Container.pivot = new Vector2(items.Count <= 6 ? 0.5f : 0f, 0.5f);

        List<UIStoreItemGroup> spawnedGroups = new List<UIStoreItemGroup>();
        UIStoreItemGroup group = m_ItemPool.Spawn(m_Container);
        group.enabled = false;
        group.OnSpawn();
        group.SetSingleRowLayout(singleRow);
        spawnedGroups.Add(group);

        if (m_Category == UIStore.Screen.COSMETICS && filter == Filter.clothing)
        {
            Debug.Log("Hard inserting free packs");
            List<string> packs = new List<string>();
            if (StoreManagerAPI.StoreData.Packs != null)
            {
                foreach (var pack in StoreManagerAPI.StoreData.Packs)
                {
                    if (pack.Value.isFree == false)
                        continue;
                    packs.Add(pack.Key);
                }

                //spawn packs if any was passed
                for (int i = 0; i < packs?.Count; i++)
                {
                    UIStoreItem item = group.GetItem();
                    if (item == null)
                    {
                        //spawn new group
                        group = m_ItemPool.Spawn(m_Container);
                        group.enabled = false;
                        group.OnSpawn();
                        group.SetSingleRowLayout(singleRow);
                        spawnedGroups.Add(group);
                        item = group.GetItem();
                    }

                    item.Setup(packs[i], StoreManagerAPI.GetPackData(packs[i]));
                }
            }
        }

        //spawn and setup the items
        for (int i = 0; i < items.Count; i++)
        {
            if (!isItemValid(data[i]))
                continue;

            UIStoreItem item = group.GetItem();
            if (item == null)
            {
                //spawn new group
                group = m_ItemPool.Spawn(m_Container);
                group.enabled = false;
                group.OnSpawn();
                group.SetSingleRowLayout(singleRow);
                spawnedGroups.Add(group);
                item = group.GetItem();
            }

            item.Setup(items[i], data[i]);
            //iconList.Add((item, data[i].iconId));

            count++;
            if (count >= 5)
            {
                count = 0;
                yield return 0;
            }
        }

        LoadingOverlay.Hide();

        foreach (var item in spawnedGroups)
        {
            item.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
