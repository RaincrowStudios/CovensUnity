using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpiritSelectionCard : MonoBehaviour
{
    [Header("SpiritCard")]
    //public GameObject SpiritCard;
    public Image spiritSprite;
    public TextMeshProUGUI title;
    public TextMeshProUGUI legend;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI tier;
	public TextMeshProUGUI atkButton;
    public GameObject wild;
    public GameObject owned;
    public TextMeshProUGUI ownedBy;
    public TextMeshProUGUI covenBy;
    public TextMeshProUGUI behaviorOwned;
    public TextMeshProUGUI behaviorWild;

	void Start()
	{
		var data = MarkerSpawner.SelectedMarker;

		var sData = DownloadedAssets.spiritDictData [data.id];
		title.text = sData.spiritName;
		string r = "";
		if (DownloadedAssets.spiritDictData [data.id].spiritTier == 1) {
			r = "Lesser Spirit";
		} else if (DownloadedAssets.spiritDictData [data.id].spiritTier == 2) {
			r = "Greater Spirit";
		} else if (DownloadedAssets.spiritDictData [data.id].spiritTier == 3) {
			r = "Superior Spirit";
		} else {
			r = "Legendary Spirit";
		}
		tier.text = r;

		atkButton.gameObject.GetComponent<Button> ().onClick.AddListener (() => ShowSelectionCard.Instance.Attack ());

		if (data.owner == "") {
			wild.SetActive(true);
			owned.SetActive(false);
			behaviorWild.text = DownloadedAssets.spiritDictData[data.id].spriitBehavior;
		}
		else
		{
			wild.SetActive(false);
			owned.SetActive(true);
			ownedBy.text = "Summoned By:" + data.owner;
			covenBy.text = (data.covenName == "" ? "Coven: None" : "Coven: " + data.covenName);
			behaviorOwned.text = DownloadedAssets.spiritDictData[data.id].spriitBehavior;
		}

		legend.text = sData.spiritLegend;
		desc.text = sData.spiritDescription;

		DownloadedAssets.GetSprite(data.id, spiritSprite);


	}

	public void DestroySelf()
	{
		Destroy (ShowSelectionCard.currCard);
	}
}