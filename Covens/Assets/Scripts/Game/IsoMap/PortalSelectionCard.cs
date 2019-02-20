using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalSelectionCard : MonoBehaviour
{


    [Header("PortalCard")]
    //public GameObject PortalCard;
    public GameObject[] portalType;
    public TextMeshProUGUI portalLevel;
    public TextMeshProUGUI creator;
    public TextMeshProUGUI portalTitle;
    public TextMeshProUGUI portalEnergy;
    public TextMeshProUGUI summonsIn;
	public Sprite spiritSprite;
	public Button castButton;
	public TextMeshProUGUI castingText;
	//public TextMeshProUGUI castText;

    void Start()
    {
		var data = MarkerSpawner.SelectedMarker;

		castButton.onClick.AddListener (() => ShowSelectionCard.Instance.Attack ());

		//Trying to check if player is silenced here so that they can't cast on a portal if they are silenced
		if (BanishManager.isSilenced) {
			castingText.color = new Color (.35f, .35f, .35f, 1f);
			castButton.enabled = false;
		} else {
			castingText.color = Color.white;
			castButton.enabled = true;
		}

		
		portalTitle.text = "Portal";
		foreach (var item in portalType)
		{
			item.SetActive(false);
		}
		if (data.degree > 0)
		{
			portalType[0].SetActive(true);
		}
		else if (data.degree == 0)
		{
			portalType[1].SetActive(true);
		}
		else
		{
			portalType[2].SetActive(false);
		}

		//			SpellCarouselManager.targetType = "portal";
		creator.text = data.owner;
		portalEnergy.text = "Energy : " + data.energy.ToString();
		summonsIn.text = "Summon in " + Utilities.EpocToDateTime(data.summonOn);
		portalLevel.text = Utilities.ToRoman(data.level);
    }

	public void DestroySelf()
	{
		Destroy (ShowSelectionCard.currCard);
	}
}