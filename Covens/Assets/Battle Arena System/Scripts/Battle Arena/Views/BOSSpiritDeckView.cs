using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class BOSSpiritDeckView : BOSBase
    {
        [SerializeField] private TextMeshProUGUI currentDominion;
        private CanvasGroup CG;



        void Start()
        {
            CG = GetComponent<CanvasGroup>();
            CG.alpha = 0;
            currentDominion.text = LocalizeLookUp.GetText("ftf_spawn_region").Replace("{{region}}", LocalizeLookUp.GetZoneName(PlayerDataManager.zone));//}.";
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            LeanTween.alphaCanvas(CG, 1, .5f);
            var pData = PlayerDataManager.playerData;

            Dictionary<string, KnownSpirits> knownSpiritsDict = new Dictionary<string, KnownSpirits>();
            foreach (KnownSpirits entry in pData.knownSpirits)
            {
                knownSpiritsDict[entry.spirit] = entry;
            }

            //foreach (Transform item in transform.GetChild(0))
            //    item.localScale = new Vector3(0, 0, 1);

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
                    BOSSpiritView.undiscoveredSpirits = totalSpiritsCount - discoveredSpiritsCount;
                    BOSSpiritView.discoveredSpirits = discoveredSpiritsCount;
                    BOSSpiritView.currentZone = zone;
                    LeanTween.alphaCanvas(CG, 0, .5f).setOnComplete(() =>
                    {
                        BOSSpiritView.instance.ShowSelectedZone();
                        BookOfShadowsView.Instance.AssignCloseListner(BOSSpiritView.instance.ShowSpiritDeck);
                    });
                });
            }
            yield return 0;
        }
    }
}
