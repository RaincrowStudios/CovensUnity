using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class ApparelView : MonoBehaviour
{
    [SerializeField] private Image baseBody;
    [SerializeField] private Image baseHand;
    [SerializeField] private Image head;
    [SerializeField] private Image hair_front;
    [SerializeField] private Image hair_back;
    [SerializeField] private Image neck;
    [SerializeField] private Image chest_front;
    [SerializeField] private Image chest_back;
    [SerializeField] private Image wristLeft;
    [SerializeField] private Image wristRight;
    [SerializeField] private Image hands;
    [SerializeField] private Image fingerLeft;
    [SerializeField] private Image fingerRight;
    [SerializeField] private Image waist;
    [SerializeField] private Image legs;
    [SerializeField] private Image feet;
    [SerializeField] private Image carryOnLeft;
    [SerializeField] private Image carryOnRight;
    [SerializeField] private Image skinFace;
    [SerializeField] private Image skinShoulder;
    [SerializeField] private Image skinChest;
    [SerializeField] private Image skinArm;
    [SerializeField] private Image style;
    [SerializeField] private Image petFeet;

    private static List<string> validStyleEquips = new List<string> { "style", "baseHand", "baseBody", "carryOnLeft", "carryOnRight" };

    private Dictionary<string, List<Image>> ApparelDict;
    public Dictionary<string, EquippedApparel> equippedApparel = new Dictionary<string, EquippedApparel>();

    private bool isCensor = false;
    private List<EquippedApparel> previousEquips;

    private void Awake()
    {
        InitApparelDict();
    }

    void InitApparelDict()
    {
        ApparelDict = new Dictionary<string, List<Image>>();
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
        equippedApparel.Clear();
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
        if (ApparelDict == null)
            InitApparelDict();

        previousEquips = new List<EquippedApparel>(data);
        previousEquips.RemoveAll(eqp => eqp.position == "style");

        ResetApparel();

        //find out if the player has a style equiped
        bool isStyle = false;
        foreach(var item in data)
        {
            if (item.position == "style")
            {
                isStyle = true;
                break;
            }
        }

        foreach (var item in data)
        {
            //only show the style allowed equips
            if (isStyle)
            {
                if (validStyleEquips.Contains(item.position))
                    InitApparel(item);
            }
            //show everyhing equiped
            else
            {
                InitApparel(item);
            }
        }
    }

    string GetStyleID(string id)
    {
        string[] races = new string[] { "A_", "E_", "O_", "A_", "E_", "O_" };
        string race = races[PlayerDataManager.playerData.bodyType - 1];
        id = id.Replace("cosmetic_", "");
        id = id.Replace("_S_", "_S_" + race);
        return id + "_Relaxed";
    }


    void InitApparel(EquippedApparel data)
    {
        if (data == null)
            return;

        if (string.IsNullOrEmpty(data.id))
            return;

        if (data.assets.Count == 0) //currently only happens for styles
        {
            //use the id as the base asset
            SetPositionSprite(data.position, data.id);
        }
        else if (data.assets.Count == 1)
        {
            SetPositionSprite(data.position, data.assets[0]);
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
                SetPositionSprite(data.position, data.assets[0]);
                SetPositionSprite(data.position, data.assets[1], 1);
            }
            else if (data.assets[0].Contains("Relaxed"))
            {
                if (isCensor) SetPositionSprite(data.position, data.assets[1]);
                else SetPositionSprite(data.position, data.assets[0]);
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

        equippedApparel[data.position] = data;
    }

    void SetPositionSprite(string position, string spirteID, int pos = 0)
    {
        try
        {
            if (position == "style") spirteID = GetStyleID(spirteID);
            ApparelDict[position][pos].gameObject.SetActive(true);
            DownloadedAssets.GetSprite(spirteID, ApparelDict[position][pos]);
        }
        catch
        {
            ApparelDict[position][0].gameObject.SetActive(true);
            DownloadedAssets.GetSprite(spirteID, ApparelDict[position][0]);
        }
    }

    public void EquipApparel(CosmeticData data)
    {
        if (data.position == "style")
        {
            previousEquips = equippedApparel.Values.ToList();
            previousEquips.RemoveAll(eqp => eqp.position == "style");
        }

        RemoveConflicts(data);

        EquippedApparel eqApparel = new EquippedApparel();
        eqApparel.id = data.id;

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
        InitApparel(eqApparel);
    }

    private void RemoveConflicts(CosmeticData item)
    {
        List<string> toRemove = new List<string>();
        bool isStyle = item.position == "style";

        if (isStyle)
        {
            //remove if conflict or if style
            foreach (var pair in equippedApparel)
            {
                if (!validStyleEquips.Contains(pair.Key) || pair.Key == "style")
                    toRemove.Add(pair.Key);
            }
        }
        else
        {
            //remove only the same slot
            if (equippedApparel.ContainsKey(item.position))
                toRemove.Add(item.position);

            //remove style if any equipd
            if (equippedApparel.ContainsKey("style") && !validStyleEquips.Contains(item.position))
                toRemove.Add("style");
        }
          
        foreach (string position in toRemove)
            UnequipApparel(position);
    }

    public void UnequipApparel(string position)
    {
        equippedApparel.Remove(position);

        //remove sprites and disable objects
        foreach (var item in ApparelDict[position])
        {
            item.overrideSprite = null;
            item.gameObject.SetActive(false);
        }

        if (position.Contains("carryOn"))
        {
            isCensor = false;
            CensorEquipped(false);
        }

        //if removing an style, reset to the starting equips
        if (position == "style")
        {
            InitializeChar(previousEquips);
        }
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
                        SetPositionSprite(apparel.Value.position, id);
                    }
                    else
                    {
                        SetPositionSprite(apparel.Value.position, item);
                    }
                }
            }
        }
    }
}
