using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IsoTokenSetup : MonoBehaviour
{
	public static IsoTokenSetup Instance { get; set;}
	public MarkerSpawner.MarkerType Type;
	public Text energy;
	public Text title;
	public Text summonedBy;
	public Text level;
	public SpriteRenderer spiritArt;
	public ApparelView witchApparel;
	void Awake()
	{
		Instance = this;
	}

	public void ChangeEnergy()
	{
		energy.text = "Energy : " + MarkerSpawner.SelectedMarker.energy.ToString ();
	}

	public void ChangeLevel()
	{
		level.text = "Level : " + MarkerSpawner.SelectedMarker.level.ToString ();
	}

	public void OnCharacterDead(bool isDead)
	{
		if (isDead)
			title.text = MarkerSpawner.SelectedMarker.displayName + " (dead)";
		else
			title.text = MarkerSpawner.SelectedMarker.displayName;
	}

	public void Setup()
	{
		var data = MarkerSpawner.SelectedMarker; 
	
		if (Type == MarkerSpawner.MarkerType.witch) {
			title.text = data.displayName;
			if (!LocationUIManager.isLocation) {
				if (MarkerSpawner.ImmunityMap [MarkerSpawner.instanceID].Contains (PlayerDataManager.playerData.instance)) {
					SpellCastUIManager.isImmune = true;
				} else {
					SpellCastUIManager.isImmune = false;
				}
			}
			energy.text ="Energy : " + data.energy.ToString ();
			level.text = "Level : " + data.level.ToString ();

			witchApparel.InitializeChar (MarkerSpawner.SelectedMarker.equipped);

		} else if (Type == MarkerSpawner.MarkerType.spirit ) {
			title.text = DownloadedAssets.spiritDictData [data.id].spiritName;
			if(DownloadedAssets.spiritArt.ContainsKey(data.id))
			spiritArt.sprite = DownloadedAssets.spiritArt [data.id];
			energy.text ="Energy : " + data.energy.ToString ();
			level.text = "Level : " + data.level.ToString ();
		} else if (Type == MarkerSpawner.MarkerType.portal) {
				title.text = "Portal";
			energy.text = "Energy : " + data.energy.ToString();
				summonedBy.text = data.owner;
		}
	}

	public void ChangeEnergy(int newEnergy)
	{
		energy.text = newEnergy.ToString ();
	}
}

