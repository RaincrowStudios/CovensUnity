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
		currentDominion.text = LocalizeLookUp.GetText ("ftf_spawn_region").Replace ("{{region}}", LocalizeLookUp.GetZoneName(PlayerDataManager.zone));//}.";

        //BOSSpirit.instance.CheckDisableButton();
        //StartCoroutine(Init());

        //APIManager.Instance.Get("/character/spirits/active", (string rs, int r) =>
        //{
        //    if (r == 200)
        //    {
        //        BOSSpirit.activeSpiritsData = JsonConvert.DeserializeObject<List<SpiritInstance>>(rs);
        //        APIManager.Instance.Get("/character/portals/active", (string res, int resp) =>
        //        {
        //            if (r == 200)
        //            {
        //                Debug.Log(res);
        //                BOSSpirit.activePortalsData = JsonConvert.DeserializeObject<List<SpiritInstance>>(res);
        //                BOSSpirit.instance.CheckDisableButton();
        //                StartCoroutine(Init());
        //            }
        //        });
        //    }
        //});
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        LeanTween.alphaCanvas(CG, 1, .5f);
        var pData = PlayerDataManager.playerData;

        Dictionary<string, KnownSpirits> knownSpiritsDict = new Dictionary<string, KnownSpirits>();
        foreach (KnownSpirits entry in pData.knownSpirits)
            knownSpiritsDict.Add(entry.spirit, entry);

        foreach (Transform item in transform.GetChild(0))
        {
            int zone = item.GetSiblingIndex();

            int totalSpiritsCount = 0;
            int discoveredSpiritsCount = 0;

            var txt = item.GetComponentInChildren<TextMeshProUGUI>();

            foreach (var spirit in DownloadedAssets.spiritDict.Values)
            {
                if (spirit.zones == null)
                    continue;

                if (spirit.zones.Contains(zone))
                {
                    totalSpiritsCount++;

                    if (knownSpiritsDict.ContainsKey(spirit.id))
                        discoveredSpiritsCount++;
                }
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