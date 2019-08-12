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

    private static List<string> fixedEquips = new List<string> { "style", "baseHand", "baseBody" };
    private static List<string> validStyleEquips = new List<string> { "carryOnLeft", "carryOnRight", "petFeet", "style" };

    private Dictionary<string, List<Image>> ApparelDict;
    public Dictionary<string, EquippedApparel> equippedApparel = new Dictionary<string, EquippedApparel>();

    private bool isCenser;
    private bool m_IsStyle;
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
        m_IsStyle = false;
        isCenser = false;
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

        ResetApparel();

        //store previous equip to reenable if the player removes the Style
        previousEquips = new List<EquippedApparel>(data);
        previousEquips.RemoveAll(eqp => eqp.position == "style");
        
        //find out if the player has a style or censor equiped
        foreach(var item in data)
        {
            if (item.position == "style")
                m_IsStyle = true;
            if (item.position == "carryOnLeft" || item.position == "carryOnRight")
                isCenser = true;
        }

        foreach (var item in data)
        {
            equippedApparel[item.position] = item;
            //only show the style allowed equips
            if (m_IsStyle)
            {
                if (validStyleEquips.Contains(item.position))
                    LoadVisuals(item);
            }
            //show everyhing equiped
            else
            {
                LoadVisuals(item);
            }
        }
    }

    private void LoadVisuals(EquippedApparel data)
    {
        if (data == null)
            return;

        if (string.IsNullOrEmpty(data.id))
            return;

        if (data.position == "style")
        {
            string assetId = "";
            string[] races = new string[] { "A_", "E_", "O_", "A_", "E_", "O_" };
            string race = races[PlayerDataManager.playerData.bodyType - 1];
            assetId = data.id.Replace("cosmetic_", "").Replace("_S_", "_S_" + race);
            if (isCenser)
                assetId += "_Censer";
            else
                assetId += "_Relaxed";

            //load the sprite
            Image img = ApparelDict["style"][0];
            DownloadedAssets.GetSprite(assetId, img);
            img.gameObject.SetActive(true);
        }
        else
        {
            List<Image> slots = ApparelDict[data.position];

            //it has a front and back slot
            if (slots.Count == 2)
            {
                //only the front slot is used
                if (data.assets.Count == 1)
                {
                    string assetId = data.assets[0];

                    if (isCenser)
                        assetId = assetId.Replace("_Relaxed", "_Censer");

                    DownloadedAssets.GetSprite(assetId, slots[0]);
                    slots[0].gameObject.SetActive(true);
                    slots[1].gameObject.SetActive(false);
                }
                //both slots are used
                else
                {
                    string[] assetId = new string[] { data.assets[0], data.assets[1] };
                    if (isCenser)
                    {
                        assetId[0] = isCenser ? assetId[0].Replace("_Relaxed", "_Censer") : assetId[0];
                        assetId[1] = isCenser ? assetId[1].Replace("_Relaxed", "_Censer") : assetId[1];
                    }

                    DownloadedAssets.GetSprite(assetId[0], slots[0]);
                    DownloadedAssets.GetSprite(assetId[1], slots[1]);

                    slots[0].gameObject.SetActive(true);
                    slots[1].gameObject.SetActive(true);
                }
            }
            else
            {
                string assetId = isCenser ? data.assets[0].Replace("_Relaxed", "_Censer") : data.assets[0];
                DownloadedAssets.GetSprite(assetId, slots[0]);
                slots[0].gameObject.SetActive(true);
            }
        }
    }

    private void UnloadVisuals(string position)
    {
        if (string.IsNullOrEmpty(position))
            return;

        if (!equippedApparel.ContainsKey(position))
            return;

        List<Image> slots = ApparelDict[position];
        foreach (Image _slot in slots)
        {
            _slot.gameObject.SetActive(false);
            _slot.overrideSprite = null;
        }
    }

    public void EquipApparel(CosmeticData data)
    {
        bool isStyle = false;
        if (data.position == "style")
        {
            previousEquips = equippedApparel.Values.ToList();
            previousEquips.RemoveAll(eqp => eqp.position == "style");
            isStyle = true;
        }

        UnequipConflicts(data.position);
        
        EquippedApparel equip = new EquippedApparel();
        equip.id = data.id;
        List<Sprite> apparelSprite = new List<Sprite>();
        if (data.apparelType == ApparelType.Base)
            equip.assets = data.assets.baseAsset;
        else if (data.apparelType == ApparelType.Grey)
            equip.assets = data.assets.grey;
        else if (data.apparelType == ApparelType.Shadow)
            equip.assets = data.assets.shadow;
        else if (data.apparelType == ApparelType.White)
            equip.assets = data.assets.white;

        equippedApparel[equip.position] = equip;
        m_IsStyle = isStyle;

        LoadVisuals(equip);
        RefreshCensorEquips();
    }

    private void UnequipConflicts(string position)
    {
        List<string> toRemove = new List<string>();
        bool isStyle = position == "style";

        if (isStyle)
        {
            //remove if conflict or if style
            foreach (var pair in equippedApparel)
            {
                //dont remove base body, hand
                if (fixedEquips.Contains(pair.Key))
                {
                    UnloadVisuals(pair.Key);
                    continue;
                }

                //remove invalid equips or other style
                if (!validStyleEquips.Contains(pair.Key) || pair.Key == position)
                    toRemove.Add(pair.Key);
            }
        }
        else
        {
            //remove only the same slot
            if (equippedApparel.ContainsKey(position))
                toRemove.Add(position);

            //remove style if any equipd
            if (equippedApparel.ContainsKey("style") && !validStyleEquips.Contains(position))
                toRemove.Add("style");
        }

        foreach (string key in toRemove)
            UnequipApparel(key);
    }

    public void UnequipApparel(string position)
    {
        if (!equippedApparel.ContainsKey(position))
            return;

        //if removing an style, reset to the starting equips
        if (position == "style")
        {
            m_IsStyle = false;
            InitializeChar(previousEquips);
        }
        else
        {
            UnloadVisuals(position);
            equippedApparel.Remove(position);
            RefreshCensorEquips();
        }
    }

    void RefreshCensorEquips()
    {
        bool previousValue = isCenser;
        isCenser = equippedApparel.ContainsKey("carryOnLeft") || equippedApparel.ContainsKey("carryOnRight");

        if (isCenser == previousValue)
            return;
        
        foreach (var apparel in equippedApparel)
            LoadVisuals(apparel.Value);
    }
}
