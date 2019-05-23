using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class PotionsData : MonoBehaviour
{

    public Text title;
    public Text type;
    public Text desc;
    public Text consume;
    public Sprite energy;
    public Sprite xp;
    public Sprite align;
    public Image icon;

    public Button consumePotion;
    public GameObject loading;
    ConsumableItem curItem;
    PotionsManager pm;

    public void Setup(ConsumableItem item, PotionsManager PM)
    {
        pm = PM;
        curItem = item;
        var data = DownloadedAssets.storeDict[item.id];
        title.text = UppercaseWords(data.title);

        if (item.id.Contains("xp"))
        {
            icon.sprite = xp;
        }
        else if (item.id.Contains("align"))
        {
            icon.sprite = align;
        }
        else if (item.id.Contains("energy"))
        {
            icon.sprite = energy;
        }
        if (item.id.Contains("xp") || item.id.Contains("align"))
        {
            type.text = "( " + data.subtitle.ToLower() + " )";
        }
        else
        {
            type.text = "";
        }
        desc.text = data.onBuyDescription;
		consume.text = LocalizeLookUp.GetText ("consume_amount").Replace ("{{Count}}", /*"Consume (" + */item.count.ToString());// + ")";
        consumePotion.onClick.AddListener(DrinkPotion);
    }

    public void DrinkPotion()
    {
        loading.SetActive(true);
        consumePotion.interactable = false;
        SendRequest();
    }

    void SendRequest()
    {
        var data = new { consumable = curItem.id };
        APIManager.Instance.PostData("inventory/consume", JsonConvert.SerializeObject(data), Result);
    }

    public void Result(string s, int r)
    {
        Debug.Log(s + r);
        if (r == 200)
        {
            loading.SetActive(false);
            consumePotion.interactable = true;
            curItem.count--;
            pm.OnConsumeSuccess(DownloadedAssets.storeDict[curItem.id].onConsumeDescription);
            if (curItem.count > 0)
            {
				consume.text = LocalizeLookUp.GetText ("consume_amount").Replace ("{{Count}}", /* "Consume (" + */curItem.count.ToString());// + ")";
                foreach (var item in PlayerDataManager.playerData.inventory.consumables)
                {
                    if (item.id == curItem.id)
                    {
                        item.count = curItem.count;
                    }
                }
            }
            else
            {
                for (int i = 0; i < PlayerDataManager.playerData.inventory.consumables.Count; i++)
                {
                    if (PlayerDataManager.playerData.inventory.consumables[i].id == curItem.id)
                    {
                        PlayerDataManager.playerData.inventory.consumables.RemoveAt(i);
                    }
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if (s == "4711")
            {
                for (int i = 0; i < PlayerDataManager.playerData.inventory.consumables.Count; i++)
                {
                    if (PlayerDataManager.playerData.inventory.consumables[i].id == curItem.id)
                    {
                        PlayerDataManager.playerData.inventory.consumables.RemoveAt(i);
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    static string UppercaseWords(string value)
    {
        value = value.ToLower();
        char[] array = value.ToCharArray();
        if (array.Length >= 1)
        {
            if (char.IsLower(array[0]))
            {
                array[0] = char.ToUpper(array[0]);
            }
        }
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i - 1] == ' ')
            {
                if (char.IsLower(array[i]))
                {
                    array[i] = char.ToUpper(array[i]);
                }
            }
        }
        return new string(array);
    }

}
