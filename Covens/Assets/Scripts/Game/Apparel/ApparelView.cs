using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class ApparelView : MonoBehaviour
{
    public Dictionary<string, List<Image>> ApparelDict = new Dictionary<string, List<Image>>();

    public Dictionary<string, EquippedApparel> equippedApparel = new Dictionary<string, EquippedApparel>();

    #region ImagePositions

    public Image baseBody;
    public Image baseHand;
    public Image head;
    public Image hair_front;
    public Image hair_back;
    public Image neck;
    public Image chest_front;
    public Image chest_back;
    public Image wristLeft;
    public Image wristRight;
    public Image hands;
    public Image fingerLeft;
    public Image fingerRight;
    public Image waist;
    public Image legs;
    public Image feet;
    public Image carryOnLeft;
    public Image carryOnRight;
    public Image skinFace;
    public Image skinShoulder;
    public Image skinChest;
    public Image skinArm;
    public Image style;
    public Image petFeet;

    #endregion

    bool isCensor = false;

    void InitDict()
    {
        ApparelDict.Clear();
        ApparelDict.Add("baseBody", new List<Image>() { baseBody });
        ApparelDict.Add("baseHand", new List<Image>() { baseHand });
        ApparelDict.Add("head", new List<Image>() { head });
        ApparelDict.Add("hair", new List<Image>() { hair_front, hair_back });
        ApparelDict.Add("neck", new List<Image>() { neck });
        ApparelDict.Add("chest", new List<Image>() { chest_front, chest_back });
        ApparelDict.Add("wristLeft", new List<Image>() { wristLeft });
        ApparelDict.Add("wristRight", new List<Image>() { wristRight });
        ApparelDict.Add("hands", new List<Image>() { hands });
        ApparelDict.Add("fingerLeft", new List<Image>() { fingerLeft });
        ApparelDict.Add("fingerRight", new List<Image>() { fingerRight });
        ApparelDict.Add("waist", new List<Image>() { waist });
        ApparelDict.Add("legs", new List<Image>() { legs });
        ApparelDict.Add("feet", new List<Image>() { feet });
        ApparelDict.Add("carryOnLeft", new List<Image>() { carryOnLeft });
        ApparelDict.Add("carryOnRight", new List<Image>() { carryOnRight });
        ApparelDict.Add("skinFace", new List<Image>() { skinFace });
        ApparelDict.Add("skinShoulder", new List<Image>() { skinShoulder });
        ApparelDict.Add("skinArm", new List<Image>() { skinArm });
        ApparelDict.Add("skinChest", new List<Image>() { skinChest });
        ApparelDict.Add("style", new List<Image>() { style });
        ApparelDict.Add("petFeet", new List<Image>() { petFeet });
    }

    public void ResetApparel()
    {
        foreach (var items in ApparelDict)
        {
            foreach (var item in items.Value)
            {
                item.gameObject.SetActive(false);
                item.overrideSprite = null;
                item.sprite = null;
            }
        }
    }

    public void InitializeChar(List<EquippedApparel> data)
    {
        InitDict();
        equippedApparel.Clear();
        ResetApparel();

        foreach (var item in data)
        {
            if (item.position == "style")
            {
                ResetApparel();
                ApparelDict["style"][0].gameObject.SetActive(true);
                DownloadedAssets.GetSprite(GetStyleID(item.id), ApparelDict["style"][0]);
                return;
            }
            initApparel(item);
        }
    }

    string GetStyleID(string id)
    {
        string race = PlayerDataManager.playerData.race.ElementAt(2) + "_";
        id = id.Replace("cosmetic_", "");
        id = id.Replace("_S_", "_S_" + race);
        return id + "_Relaxed";
    }


    void initApparel(EquippedApparel data)
    {
        if (data == null)
            return;

        if (string.IsNullOrEmpty(data.id))
            return;
        if (data.assets.Count == 0)
        {
            ResetApparel();
            setPositionApparel(data.position, data.id);
        }
        else if (data.assets.Count == 1)
        {
            //			ApparelDict [data.position] [0].gameObject.SetActive (true);
            setPositionApparel(data.position, data.assets[0]);
            if (ApparelDict[data.position].Count == 2)
            {
                ApparelDict[data.position][1].gameObject.SetActive(false);
            }
            if (!ApparelDict[data.position][0].gameObject.activeInHierarchy)
            {
                ApparelDict[data.position][0].gameObject.SetActive(true);
            }
        }
        else if (data.assets.Count == 2)
        {
            if (data.assets[0].Contains("Front"))
            {
                setPositionApparel(data.position, data.assets[0]);
                setPositionApparel(data.position, data.assets[1], 1);
            }
            else
            {
                if (data.assets[0].Contains("Relaxed"))
                {
                    if (isCensor) setPositionApparel(data.position, data.assets[1]);
                    else setPositionApparel(data.position, data.assets[0]);
                }
            }
        }
        if (ApparelDict["carryOnLeft"][0].gameObject.activeInHierarchy || ApparelDict["carryOnRight"][0].gameObject.activeInHierarchy)
        {
            if (!isCensor)
            {
                CensorEquipped(true);
                isCensor = true;
            }
        }
        equippedApparel[data.id] = data;

    }

    void setPositionApparel(string key, string spirteID, int pos = 0)
    {
        try
        {
            if (key == "style") spirteID = GetStyleID(spirteID);
            ApparelDict[key][pos].gameObject.SetActive(true);
            DownloadedAssets.GetSprite(spirteID, ApparelDict[key][pos]);
        }
        catch
        {
            ApparelDict[key][0].gameObject.SetActive(true);
            DownloadedAssets.GetSprite(spirteID, ApparelDict[key][0]);
        }
    }

    public void EquipApparel(CosmeticData data)
    {
        //foreach (var item in data.conflicts)
        //{
        //    if (equippedApparel.ContainsKey(item))
        //    {
        //        foreach (var apparelItem in ApparelDict[equippedApparel[item].position])
        //        {
        //            apparelItem.overrideSprite = null;
        //            apparelItem.gameObject.SetActive(false);
        //            equippedApparel.Remove(item);
        //        }
        //    }
        //}

        //remove other equips occupying the same position
        List<string> toRemove = new List<string>();
        foreach (var pair in equippedApparel)
        {
            if (pair.Value.position == data.position)
                toRemove.Add(pair.Key);
        }
        foreach (string key in toRemove)
        {
            equippedApparel.Remove(key);
        }

        EquippedApparel eqApparel = new EquippedApparel();
        eqApparel.id = data.id;
        eqApparel.position = data.position;

        List<Sprite> apparelSprite = new List<Sprite>();
        if (data.apparelType == ApparelType.Base)
        {
            eqApparel.assets = data.assets.baseAsset;
        }
        else if (data.apparelType == ApparelType.Grey)
        {
            eqApparel.assets = data.assets.grey;
        }
        else if (data.apparelType == ApparelType.Shadow)
        {
            eqApparel.assets = data.assets.shadow;
        }
        else if (data.apparelType == ApparelType.White)
        {
            eqApparel.assets = data.assets.white;
        }
        initApparel(eqApparel);
    }

    public void UnequipApparel(CosmeticData data)
    {
        if (equippedApparel.ContainsKey(data.id))
            equippedApparel.Remove(data.id);

        foreach (var item in ApparelDict[data.position])
        {
            item.overrideSprite = null;
            item.gameObject.SetActive(false);
        }
        if (data.position.Contains("carryOn"))
        {
            isCensor = false;
            CensorEquipped(false);
        }

        if (data.position == "style")
        {
            Debug.Log("resetting style");
            InitializeChar(equippedApparel.Values.ToList());
        }
        //foreach (var item in equippedApparel) {
        //	Debug.Log (item.Key);
        //}
    }

    void CensorEquipped(bool isActive)
    {
        foreach (var apparel in equippedApparel)
        {
            foreach (var item in apparel.Value.assets)
            {
                if (item.Contains("Relaxed"))
                {
                    if (isActive)
                    {
                        string id = String.Copy(item);
                        id = id.Replace("Relaxed", "Censer");
                        setPositionApparel(apparel.Value.position, id);
                    }
                    else
                    {
                        setPositionApparel(apparel.Value.position, item);
                    }
                }
            }
        }
    }
}
