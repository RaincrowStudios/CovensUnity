using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class ApparelView : MonoBehaviour
{
    [SerializeField] private Image mannequin;
    [Space]
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

    private static List<string> unremovableCosmetics = new List<string> { "baseHand", "baseBody" };
    private static List<string> validStyleEquips = new List<string> { "carryOnLeft", "carryOnRight", "petFeet", "style" };

    private Dictionary<string, List<Image>> m_ApparelDict;
    private Dictionary<string, List<Image>> ApparelDict
    {
        get
        {
            if (m_ApparelDict == null)
            {
                m_ApparelDict = new Dictionary<string, List<Image>>();
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
            return m_ApparelDict;
        }
    }

    public Dictionary<string, EquippedApparel> equippedApparel = new Dictionary<string, EquippedApparel>();

    private bool m_IsCenser;
    private bool m_IsStyle;
    
    public void ResetApparelView()
    {
        m_IsStyle = false;
        m_IsCenser = false;
        equippedApparel.Clear();

        foreach (var items in ApparelDict)
        {
            foreach (var item in items.Value)
            {
                item.gameObject.SetActive(false);
                item.overrideSprite = null;
            }
        }
    }

    public void InitCharacter(List<EquippedApparel> equipped, bool reset = true, System.Action onComplete = null)
    {
        if (reset)
            ResetApparelView();

        mannequin.gameObject.SetActive(true);

        //init censer/style value
        foreach (var equip in equipped)
        {
            if (equip.position == "style")
                m_IsStyle = true;
            else if (equip.position == "carryOnLeft" || equip.position == "carryOnRight")
                m_IsCenser = true;
        }

        if (equipped.Count == 0)
            onComplete?.Invoke();

        //load the sprite assets
        for (int i = 0; i < equipped.Count; i++)
        {
            equippedApparel[equipped[i].position] = equipped[i];

            if (i >= equipped.Count - 1)
                LoadVisuals(equipped[i], onComplete);
            else
                LoadVisuals(equipped[i], null);
        }
    }
    
    public void EquipApparel(CosmeticData data, bool isMannequin = false)
    {
        //unequip conflicts
        List<string> conflicts = new List<string>();
        bool isStyle = (data.position == "style");

        //gets what needs to be removed
        if (isStyle)
        {
            mannequin.gameObject.SetActive(true);

            foreach (var pair in equippedApparel)
            {
                if (unremovableCosmetics.Contains(pair.Key))
                {
                    UnloadVisuals(pair.Key);
                    continue;
                }

                if (validStyleEquips.Contains(pair.Key) == false || pair.Key == data.position)
                {
                    conflicts.Add(pair.Key);
                }
            }
        }
        else
        {
            //remove only the same slot
            if (equippedApparel.ContainsKey(data.position))
                conflicts.Add(data.position);

            //remove style if any equipd
            if (equippedApparel.ContainsKey("style") && !validStyleEquips.Contains(data.position))
                conflicts.Add("style");
        }

        //unequip them
        foreach (var position in conflicts)
        {
            if (position != data.position)
                UnequipApparel(position);
        }

        //create the apparel object
        EquippedApparel equip = new EquippedApparel();
        equip.id = data.id;

        switch(data.apparelType)
        {
            case ApparelType.Base:      equip.assets = data.assets.baseAsset;   break;
            case ApparelType.Shadow:    equip.assets = data.assets.shadow;      break;
            case ApparelType.Grey:      equip.assets = data.assets.grey;        break;
            case ApparelType.White:     equip.assets = data.assets.white;       break;
        }

        //equip it
        equippedApparel[data.position] = equip;

        //update censer and style state
        m_IsStyle = isStyle;
        if (m_IsCenser == false && (data.position == "carryOnLeft" || data.position == "carryOnRight"))
        {
            m_IsCenser = true;
            RefreshCensorEquips();
        }

        //load the new visuals
        LoadVisuals(equip, () => mannequin.gameObject.SetActive(false), isMannequin);
    }

    public void UnequipApparel(string position)
    {
        //unload assets
        UnloadVisuals(position);

        //remove from dict
        if (equippedApparel.ContainsKey(position))
            equippedApparel.Remove(position);

        //update censer state
        if (position == "carryOnLeft" || position == "carryOnRight")
        {
            if (m_IsCenser)
            {
                m_IsCenser = false;
                RefreshCensorEquips();
            }
        }

        //update style state
        if (position == "style")
        {
            m_IsStyle = false;

            //load previously hidden basebody sprites
            foreach(var pair in equippedApparel)
            {
                if (unremovableCosmetics.Contains(pair.Key))
                    LoadVisuals(pair.Value, null);
            }
        }
    }

    private void UnloadVisuals(string position)
    {
        if (string.IsNullOrEmpty(position))
            return;

        List<Image> slots = ApparelDict[position];
        foreach (Image _slot in slots)
        {
            _slot.gameObject.SetActive(false);
            _slot.overrideSprite = null;
        }
    }

    private void LoadVisuals(EquippedApparel apparel, System.Action callback, bool isMannequin = false)
    {
        if (apparel.position == "style")
        {
            string assetId = "";
           
            if (isMannequin)
            {
                assetId = apparel.id.Replace("cosmetic_", "").Replace("_S_", "_S_M_");
            }
            else
            {
                string[] races = new string[] { "A_", "E_", "O_", "A_", "E_", "O_" };
                string race = races[PlayerDataManager.playerData.bodyType];
                assetId = apparel.id.Replace("cosmetic_", "").Replace("_S_", "_S_" + race);


                if (m_IsCenser)
                    assetId += "_Censer";
                else
                    assetId += "_Relaxed";
            }

            //load the style sprite
            Image img = ApparelDict["style"][0];
            DownloadedAssets.GetSprite(assetId, spr =>
            {
                OnLoadSprite(apparel.id, apparel.position, img, spr);
                callback?.Invoke();
            });
            img.gameObject.SetActive(true);
        }
        else
        {
            List<Image> slots = ApparelDict[apparel.position];

            //it has a front and back slot
            if (slots.Count == 2)
            {
                //only the front slot is used
                if (apparel.assets.Count == 1)
                {
                    string assetId = apparel.assets[0];

                    if (m_IsCenser)
                        assetId = assetId.Replace("_Relaxed", "_Censer");

                    DownloadedAssets.GetSprite(assetId, spr =>
                    {
                        OnLoadSprite(apparel.id, apparel.position, slots[0], spr);
                        callback?.Invoke();
                    });
                    slots[0].gameObject.SetActive(true);
                    slots[1].gameObject.SetActive(false);
                }
                //both slots are used
                else
                {
                    string[] assetId = new string[] { apparel.assets[0], apparel.assets[1] };
                    if (m_IsCenser)
                    {
                        assetId[0] = m_IsCenser ? assetId[0].Replace("_Relaxed", "_Censer") : assetId[0];
                        assetId[1] = m_IsCenser ? assetId[1].Replace("_Relaxed", "_Censer") : assetId[1];
                    }

                    DownloadedAssets.GetSprite(assetId[0], spr =>
                    {
                        OnLoadSprite(apparel.id, apparel.position, slots[0], spr);
                        //onLoadAsset(aux);
                    });
                    DownloadedAssets.GetSprite(assetId[1], spr =>
                    {
                        OnLoadSprite(apparel.id, apparel.position, slots[1], spr);
                        callback?.Invoke();
                    });

                    slots[0].gameObject.SetActive(true);
                    slots[1].gameObject.SetActive(true);
                }
            }
            else
            {
                string assetId = m_IsCenser ? apparel.assets[0].Replace("_Relaxed", "_Censer") : apparel.assets[0];
                DownloadedAssets.GetSprite(assetId, spr =>
                {
                    OnLoadSprite(apparel.id, apparel.position, slots[0], spr);
                    callback?.Invoke();
                });
                slots[0].gameObject.SetActive(true);
            }
        }
    }

    private void OnLoadSprite(string cosmetic, string position, Image image, Sprite sprite)
    {
        if (position == "baseBody")
            mannequin.gameObject.SetActive(false);

        if (equippedApparel.ContainsKey(position) == false)
            return;

        if (equippedApparel[position].id != cosmetic)
            return;

        image.overrideSprite = sprite;
    }
    
    private void RefreshCensorEquips()
    {
        //reload censer related visuals that need reloading
        List<EquippedApparel> visuals = new List<EquippedApparel>();
        foreach (var pair in equippedApparel)
        {
            foreach (var assets in pair.Value.assets)
            {
                if (assets.Contains("_Relaxed") || assets.Contains("_Censer"))
                {
                    visuals.Add(pair.Value);
                    break;
                }
            }
        }

        //reload them
        foreach (var equip in visuals)
        {
            LoadVisuals(equip, null);
        }
    }
}
