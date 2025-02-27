﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a class to manage the DB and assets of itens
/// </summary>
public class ItemDB : Patterns.SingletonComponent<ItemDB>
{
    const string ItemDBPath = "GameSettings/ItemDB.json";
    const string FilePreviewPath = "Inventory/{0}Preview/{1}";
    const string FilePath = "Inventory/{0}/{1}";
    

    public WardrobeItemDB m_pWardrobeItemDB;

    public WardrobeItemModel[] Itens
    {
        get
        {
            if (m_pWardrobeItemDB == null || m_pWardrobeItemDB.list.Length <= 0)
                LoadDB();
            return m_pWardrobeItemDB.list;
        }
    }
    public bool IsItemLoaded
    {
        get { return m_pWardrobeItemDB != null; }
    }


    private void Start()
    {
//        Debug.Log("------ ItemDB should be loaded from a different way in release");
        LoadDB();
    }

    public void LoadDB()
    {
        TextAsset pText = Resources.Load<TextAsset>(ItemDBPath);
        WardrobeItemDB pDB = JsonUtility.FromJson<WardrobeItemDB>(pText.text);
        foreach (var pItem in pDB.list)
            pItem.Cache();
        m_pWardrobeItemDB = pDB;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Raincrow/Item DB/ReLoad")]
    public static void ReLoad()
    {
        Instance.LoadDB();
    }
    [UnityEditor.MenuItem("Raincrow/Item DB/Test Load")]
    public static void Load()
    {
        TextAsset pText = Resources.Load<TextAsset>(ItemDBPath);
        Debug.Log("Load: " + pText.text);
        WardrobeItemDB pDB = JsonUtility.FromJson<WardrobeItemDB>(pText.text);
        // cache the variables
        foreach (var pItem in pDB.list)
            pItem.Cache();
        Debug.Log("success!");
        Debug.Log(pDB.ToString());
    }
#endif

    #region get itens
    /// <summary>
    /// gets all items
    /// </summary>
    /// <param name="eGender"></param>
    /// <returns></returns>
    public List<WardrobeItemModel> GetAllItems(EnumGender eGender)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        WardrobeItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            if (vWList[i].GenderEnum == eGender)
            {
                vItemList.Add(vWList[i]);
            }
        }
        return vItemList;
    }

    /// <summary>
    /// gets the default body
    /// </summary>
    /// <param name="eGender"></param>
    /// <returns></returns>
    public List<WardrobeItemModel> GetDefaultItens(EnumGender eGender)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        WardrobeItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            if (vWList[i].GenderEnum == eGender && vWList[i].IsDefaultB)
            {
                vItemList.Add(vWList[i]);
            }
        }
        return vItemList;
    }
    /// <summary>
    /// gets the item based on its category
    /// </summary>
    /// <param name="eCategories"></param>
    /// <returns></returns>
    public List<WardrobeItemModel> GetItens(EnumWardrobeCategory eCategories, EnumGender eGender)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        WardrobeItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            if(vWList[i].GenderEnum == eGender && (vWList[i].WardrobeCategory & eCategories) != 0)
            {
                vItemList.Add(vWList[i]);
            }
        }
        return vItemList;
    }
    /// <summary>
    /// get itens from the slot
    /// </summary>
    /// <param name="eSlot"></param>
    /// <param name="eGender"></param>
    /// <returns></returns>
    public List<WardrobeItemModel> GetItens(EnumEquipmentSlot eSlot, EnumGender eGender)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        WardrobeItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            if (vWList[i].GenderEnum == eGender && vWList[i].EquipmentSlotEnum == eSlot)
            {
                vItemList.Add(vWList[i]);
            }
        }
        return vItemList;
    }
    /// <summary>
    /// get item from its gender
    /// </summary>
    /// <param name="eGender"></param>
    /// <returns></returns>
    public List<WardrobeItemModel> GetItens(EnumGender eGender, EnumEquipmentSlot[] vIgnore = null)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        WardrobeItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            if (vWList[i].GenderEnum == eGender)
            {
                if (vIgnore != null)
                {
                    bool bIgnore = false;
                    for (int j = 0; j < vIgnore.Length; j++)
                        if (vIgnore[j] == vWList[i].EquipmentSlotEnum)
                            bIgnore = true;
                    if (bIgnore)
                        continue;
                }
                vItemList.Add(vWList[i]);
            }
        }
        return vItemList;
    }
    /// <summary>
    /// gets the item from its ID
    /// </summary>
    /// <param name="sID"></param>
    /// <returns></returns>
    public WardrobeItemModel GetItem(string sID)
    {
        WardrobeItemModel[] vWList = Itens;
        for (int i = 0; i < vWList.Length; i++)
        {
            if (vWList[i].ID == sID)
            {
                return vWList[i];
            }
        }
        Debug.LogError(sID + ". ID File Not Found");
        return null;
    }


    /// <summary>
    /// gets a list of textures related to this item
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public List<Sprite> GetTextures(WardrobeItemModel pItem, HandMode eHandMode)
    {
        // Resources/Inventory/Female/f_C_S_DWO
        List<Sprite> vTexture = new List<Sprite>();
        string[] vTextures = pItem.GetTexturesName(eHandMode);
        for (int i = 0; i < vTextures.Length; i++)
        {
            string sPathName = GetTexturePath(pItem.Gender, vTextures[i]);
            string sSpriteName = string.Format("Item-{0}", sPathName);
            Sprite pSprite = SpriteResources.GetSprite(sSpriteName);
            if(pSprite != null)
            {
                vTexture.Add(pSprite);
                continue;
            }
            pSprite = LoadFile(pItem.Name);
            //Texture2D pTexture = LoadPreviewFile(sPathName);
            if(pSprite == null)
            {
//                Debug.LogError("couldn't load file: " + pItem.Name);
                vTexture.Add(null);
                continue;
            }
            //pSprite = Sprite.Create(pTexture, new Rect(0, 0, pTexture.width, pTexture.height), new Vector2(.5f, .5f));
            // cache it
            SpriteResources.AddSprite(sSpriteName, pSprite);
            vTexture.Add(pSprite);
        }
        return vTexture;
    }

    /// <summary>
    /// gets the preview icon of this element
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public Sprite GetTexturePreview(WardrobeItemModel pItem)
    {
        // Resources/Inventory/FemalePreview/f_C_S_DWO
        string sPathName = GetTexturePreviewPath(pItem.Gender, pItem.Name);
        string sSpriteName = string.Format("Item-{0}", sPathName);
        Sprite pSprite = SpriteResources.GetSprite(sSpriteName);
        if (pSprite != null)
        {
            return pSprite;
        }
        pSprite = LoadPreviewFile(pItem.Name);
        if (pSprite == null)
        {
            Debug.LogError("couldn't load preview: " + pItem.Name);
            return null;
        }
        //pSprite = Sprite.Create(pTexture, new Rect(0, 0, pTexture.width, pTexture.height), new Vector2(.5f, .5f));
        // cache it
        SpriteResources.AddSprite(sSpriteName, pSprite);

        return pSprite;
    }

    #endregion


    #region utilities


    private string GetTexturePath(string sGender, string sName)
    {
        return string.Format(FilePath, sGender, sName);
    }
    private string GetTexturePreviewPath(string sGender, string sName)
    {
        return string.Format(FilePreviewPath, sGender, sName);
    }


    private Sprite LoadPreviewFile(string sName)
    {
//        if (DownloadedAssets.wardobePreviewArt.ContainsKey(sName))
//        {
//            return DownloadedAssets.wardobePreviewArt[sName];
//        }
        return null;
        // TODO: change here how the textures are loaded
        //Texture2D pText = Resources.Load<Texture2D>(sPath);
        //if (pText == null)
        //{
        //    Debug.LogError("LoadFile FAILED: " + sPath);
        //}
        //return pText;
    }
    private Sprite LoadFile(string sName)
    {
//        if (DownloadedAssets.wardobeArt.ContainsKey(sName))
//        {
//            return DownloadedAssets.wardobeArt[sName];
//        }
        return null;

    }

    #endregion
}