using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class ApparelButtonData : MonoBehaviour
{

    public GameObject newTag;
    public GameObject whiteAsset;
    public GameObject greyAsset;
    public GameObject shadowAsset;
    public GameObject whiteAssetFill;
    public GameObject greyAssetFill;
    public GameObject shadowAssetFill;
    public Image icon;
    public GameObject Selected;
    public TextMeshProUGUI apparelName;
    public CanvasGroup ConflictCG;
    public GameObject closeButton;

    int tapCount;
    int maxCount;
    ApparelView viewPlayer;
    CosmeticData apparelData;
    Dictionary<int, fillDictData> fillDict = new Dictionary<int, fillDictData>();

    public void Setup(CosmeticData data)
   {
        fillDict.Clear();
        closeButton.SetActive(false);
        viewPlayer = ApparelManager.instance.ActiveViewPlayer;
        tapCount = 0;
        maxCount = 0;
        if (data.isNew)
        {
            newTag.SetActive(true);
        }
        else
        {
            newTag.SetActive(false);
        }
        apparelData = data;
        Selected.SetActive(false);
        ConflictCG.alpha = 1;
        DownloadedAssets.GetSprite(data.iconId, (spr) =>
        {
            icon.overrideSprite = spr;
        }, true);

        apparelName.text = LocalizeLookUp.GetStoreTitle(data.id);
        if (data.position == "style")
        {
            maxCount = 0;
        }
        else if (data.assets.baseAsset != null && data.assets.baseAsset.Count > 0)
        {
            maxCount = 0;
        }
        else
        {
            // Debug.Log(data.id);
            if (data.assets.shadow != null && data.assets.shadow.Count > 0)
            {
                maxCount += 1;
                fillDict.Add(maxCount, new fillDictData { fillObject = shadowAssetFill, type = ApparelType.Shadow });
                shadowAsset.gameObject.SetActive(true);
                foreach (var item in data.assets.shadow)
                {
                    foreach (var equip in viewPlayer.equippedApparel)
                    {

                        if (equip.Value.position != "style" && equip.Value.assets[0] == item)
                        {
                            shadowAssetFill.SetActive(true);
                            tapCount = maxCount;
                        }
                    }
                }
            }
            if (data.assets.grey != null && data.assets.grey.Count > 0)
            {
                greyAsset.gameObject.SetActive(true);
                maxCount += 1;
                fillDict.Add(maxCount, new fillDictData { fillObject = greyAssetFill, type = ApparelType.Grey });
                foreach (var item in data.assets.grey)
                {
                    foreach (var equip in viewPlayer.equippedApparel)
                    {
                        if (equip.Value.position != "style" && equip.Value.assets[0] == item)
                        {
                            greyAssetFill.SetActive(true);
                            tapCount = maxCount;
                        }
                    }
                }
            }
            if (data.assets.white != null && data.assets.white.Count > 0)
            {
                whiteAsset.gameObject.SetActive(true);
                maxCount += 1;
                fillDict.Add(maxCount, new fillDictData { fillObject = whiteAssetFill, type = ApparelType.White });
                foreach (var item in data.assets.white)
                {
                    foreach (var equip in viewPlayer.equippedApparel)
                    {
                        if (equip.Value.position != "style" && equip.Value.assets[0] == item)
                        {
                            whiteAssetFill.SetActive(true);
                            tapCount = maxCount;
                        }
                    }
                }
            }
        }
    }

    public void OnClick()
    {
        Selected.SetActive(true);
        ConflictCG.alpha = 1;
        closeButton.SetActive(true);

        if (maxCount == 0)
        {
            viewPlayer.EquipApparel(apparelData);
        }
        else
        {
            if (tapCount < maxCount)
            {
                tapCount++;
                whiteAssetFill.SetActive(false);
                shadowAssetFill.SetActive(false);
                greyAssetFill.SetActive(false);
                fillDict[tapCount].fillObject.SetActive(true);
                apparelData.apparelType = fillDict[tapCount].type;
                viewPlayer.EquipApparel(apparelData);
            }
            else
            {
                tapCount = 0;
                OnClick();
            }
        }
        //ApparelManagerUI.Instance.SetConflict(apparelData.conflicts);
        ApparelManagerUI.Instance.DisableOtherSelection(apparelData.id);
        ApparelManagerUI.equipChanged = true;
    }

    public void Unequip()
    {
        tapCount = 0;
        viewPlayer.UnequipApparel(apparelData);
        Selected.SetActive(false);
        whiteAssetFill.SetActive(false);
        greyAssetFill.SetActive(false);
        closeButton.SetActive(false);
        shadowAssetFill.SetActive(false);
        ApparelManagerUI.Instance.ClearConflicts();
    }

    class fillDictData
    {
        public GameObject fillObject;
        public ApparelType type;
    }
}

