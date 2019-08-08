using System.Collections.Generic;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay Instance;
    private Canvas m_Canvas;
    private CanvasGroup m_CanvasGroup;
    private UnityEngine.UI.GraphicRaycaster m_InputRaycaster;
    private int m_TweenId;

    private void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_CanvasGroup = m_Canvas.GetComponent<CanvasGroup>();
        m_InputRaycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void _Show()
    {
        if (m_Canvas.enabled && m_InputRaycaster.enabled)
            return;

        Debug.Log("Show overlay load");

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.5f)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void _Hide()
    {
        if (m_Canvas.enabled == false && m_InputRaycaster.enabled == false)
            return;

        Debug.Log("Hide overlay load");

        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 1.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .uniqueId;
    }

    public static void Show()
    {
        if (Instance == null)
        {
            return;
        }

        Instance._Show();
    }

    public static void Hide()
    {
        if (Instance == null)
        {
            return;
        }

        Instance._Hide();
    }

    public struct Silver
    {
        public string productId;
        public string id;
        public int amount;
        public string bonus;
        public float cost;
    }

    public struct Consumable
    {
        public string id;
        public int silver;
        public int gold;
        public float duration;
    }

    public struct StoreCosmetic
    {
        public string id;
        public string type;
        public int silver;
        public int gold;
        public string iconId;
        public string position;

    }

    [TextArea(5, 5)]
    public string json;

    [TextArea(5, 5)]
    [SerializeField] public string cosmetics;

    [ContextMenu("Parse json")]
    public void ParseJson()
    {
        List<CosmeticData> allCosmetics = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CosmeticData>>(cosmetics);
        List<StoreCosmetic> storeCosmetics = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StoreCosmetic>>(json);

        string res = "";
        foreach (CosmeticData cosmetic in allCosmetics)
        {
            StoreCosmetic store = storeCosmetics.Find(item => item.id == cosmetic.id);

            if (string.IsNullOrEmpty(store.id))
                res += $"{cosmetic.id}\n";
            else
                res += $"{store.id}\t{store.silver}\t{store.gold}\n";
        }
        

        //List<StoreCosmetic> missingCosmetics = new List<StoreCosmetic>();
        //foreach(var item in storeCosmetics)
        //{
        //    if (allCosmetics.Exists(cosm => cosm.id == item.id))
        //        continue;

        //    res += $"{item.id}\t{item.type}\t{item.position}\t";
        //    if(item.assets.baseAsset != null)
        //    {
        //        foreach (string asset in item.assets.baseAsset)
        //            res += asset + ",";
        //        res += "*";
        //    }
        //    res += "\t";
        //    if (item.assets.shadow != null)
        //    {
        //        foreach (string asset in item.assets.shadow)
        //            res += asset + ",";
        //        res += "*";
        //    }
        //    res += "\t";
        //    if (item.assets.grey != null)
        //    {
        //        foreach (string asset in item.assets.grey)
        //            res += asset + ",";
        //        res += "*";
        //    }
        //    res += "\t";
        //    if (item.assets.white != null)
        //    {
        //        foreach (string asset in item.assets.white)
        //            res += asset + ",";
        //        res += "*";
        //    }
        //    res += $"\t{}\t{item.silver}\t{item.gold}\n";
        //}

        Debug.Log(res);

        //Consumable[] silver = Newtonsoft.Json.JsonConvert.DeserializeObject<Consumable[]>(json);
        //string res = "";
        //foreach(var item in silver)
        //{
        //    res += $"{item.id}\t{item.silver}\t{item.gold}\t{item.duration}\n";
        //}
        //Debug.Log(res);


    }
}
