using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

public class BOSSpiritDeck : BOSBase
{
    [SerializeField] private TextMeshProUGUI currentDominion;
    [SerializeField] private GameObject card;
    [SerializeField] private Transform container;
    void Start()
    {
        currentDominion.text = $"You are in the spawn region of {DownloadedAssets.zonesIDS[PlayerDataManager.zone]}.";

        APIManager.Instance.GetData("/character/spirits/active", (string rs, int r) =>
        {
            if (r == 200)
            {
                BOSSpirit.activeSpiritsData = JsonConvert.DeserializeObject<List<SpiritData>>(rs);
                APIManager.Instance.GetData("/character/portals/active", (string res, int resp) =>
                {
                    if (r == 200)
                    {
                        BOSSpirit.activePortalsData = JsonConvert.DeserializeObject<List<SpiritData>>(res);
                        BOSSpirit.instance.CheckDisableButton();
                        CreateDeckCards();
                    }
                });
            }
        });

    }

    void CreateDeckCards()
    {
        foreach (var item in DownloadedAssets.zonesIDS)
        {
            var g = Utilities.InstantiateObject(card, container);
            if (item.Key == PlayerDataManager.zone)
                g.transform.SetAsFirstSibling();
            g.GetComponent<BOSSpiritDeckCardItem>().Setup(item.Key);
        }
    }
}