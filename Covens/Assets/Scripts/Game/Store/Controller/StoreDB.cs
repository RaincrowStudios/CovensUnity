using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreDB : Patterns.SingletonComponent<StoreDB>
{
    const string ItemDBPath = "GameSettings/StoreDB.json";

    public StoreItemDB m_pItemDB;

    public StoreItemModel[] Itens
    {
        get
        {
            if (m_pItemDB == null || m_pItemDB.list.Length <= 0)
                LoadDB();
            return m_pItemDB.list;
        }
    }



    private void Start()
    {
        Debug.Log("------ ItemDB should be loaded from a different way in release");
        LoadDB();
    }

    public void LoadDB()
    {
        TextAsset pText = Resources.Load<TextAsset>(ItemDBPath);
        StoreItemDB pDB = JsonUtility.FromJson<StoreItemDB>(pText.text);
        foreach (var pItem in pDB.list)
            pItem.Cache();
        m_pItemDB = pDB;
    }

    /// <summary>
    /// gets one item from StoreDB
    /// </summary>
    /// <param name="sID"></param>
    /// <returns></returns>
    public StoreItemModel GetItem(string sID)
    {
        List<StoreItemModel> vItemList = new List<StoreItemModel>();
        StoreItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            if (vWList[i].ID == sID)
                return vWList[i];
        }
        return null;
    }

    /// <summary>
    /// gets a list of item of same store type
    /// </summary>
    /// <param name="eStores"></param>
    /// <returns></returns>
    public List<StoreItemModel> GetItens(params EnumStoreType[] eStores)
    {
        List<StoreItemModel> vItemList = new List<StoreItemModel>();
        StoreItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            foreach(EnumStoreType eStore in eStores)
            {
                if(vWList[i].StoreTypeEnum == eStore)
                {
                    vItemList.Add(vWList[i]);
                    break;
                }
            }
        }
        return vItemList;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Raincrow/Store DB/ReLoad")]
    public static void ReLoad()
    {
        Instance.LoadDB();
    }
    [UnityEditor.MenuItem("Raincrow/Store DB/Test Load")]
    public static void Load()
    {
        TextAsset pText = Resources.Load<TextAsset>(ItemDBPath);
        Debug.Log("Load: " + pText.text);
        StoreItemDB pDB = JsonUtility.FromJson<StoreItemDB>(pText.text);
        // cache the variables
        foreach (var pItem in pDB.list)
            pItem.Cache();
        Debug.Log("success!");
        Debug.Log(pDB.ToString());
    }
#endif
}
