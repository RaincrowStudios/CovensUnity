using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class BOSSpiritDeck : BOSBase
{
    [SerializeField] private TextMeshProUGUI currentDominion;
    private CanvasGroup CG;



    void Start()
    {
        CG = GetComponent<CanvasGroup>();
        CG.alpha = 0;
        currentDominion.text = $"You are in the spawn region of {DownloadedAssets.zonesIDS[PlayerDataManager.zone]}.";

        APIManager.Instance.GetData("/character/spirits/active", (string rs, int r) =>
        {
            Debug.Log("GETTING SPIRIT DATA");
            Debug.Log(rs);
            if (r == 200)
            {
                BOSSpirit.activeSpiritsData = JsonConvert.DeserializeObject<List<SpiritData>>(rs);
                APIManager.Instance.GetData("/character/portals/active", (string res, int resp) =>
                {
                    if (r == 200)
                    {
                        Debug.Log(res);
                        BOSSpirit.activePortalsData = JsonConvert.DeserializeObject<List<SpiritData>>(res);
                        BOSSpirit.instance.CheckDisableButton();
                        StartCoroutine(Init());
                    }
                });
            }
        });

    }

    IEnumerator Init()
    {
        LeanTween.alphaCanvas(CG, 1, .5f);
        var pSData = PlayerDataManager.summonMatrixDict;
        var pData = PlayerDataManager.playerData;
        foreach (Transform item in transform.GetChild(0))
        {
            int zone = item.GetSiblingIndex();

            int totalSpiritsCount = 0;
            int discoveredSpiritsCount = 0;

            var txt = item.GetComponentInChildren<TextMeshProUGUI>();

            foreach (var k in pSData)
            {

                if (k.Value.zone.Contains(zone))
                    totalSpiritsCount++;
                if (pData.knownSpiritsDict.ContainsKey(k.Key) && (k.Value.zone.Contains(zone)))
                    discoveredSpiritsCount++;
            }
            txt.text = $"{discoveredSpiritsCount.ToString()} / {totalSpiritsCount.ToString()}";
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                BOSSpirit.undiscoveredSpirits = totalSpiritsCount - discoveredSpiritsCount;
                BOSSpirit.discoveredSpirits = discoveredSpiritsCount;
                BOSSpirit.currentZone = zone;
                LeanTween.alphaCanvas(CG, 0, .5f).setOnComplete(() =>
                {
                    BOSSpirit.instance.ShowSelectedZone();
                    BOSController.Instance.AssignCloseListner(BOSSpirit.instance.ShowSpiritDeck);
                });
            });
            item.localScale = Vector3.zero;
            LeanTween.scale(item.gameObject, Vector3.one, .45f).setEase(LeanTweenType.easeInOutQuad);
            yield return new WaitForSeconds(.08f);

        }

    }
}