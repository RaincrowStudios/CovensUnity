using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerManagerUI : UIAnimationManager
{
	public static PlayerManagerUI Instance { get; set;}
	
	[Header("Flight")]
	public GameObject FlightObject;

	[Header("PlayerInfo UI")]
	public Text Level;
	public Text Energy;
	public Text silverDrachs;
	public Text silverDrachsStore;
	public GameObject EnergyWhite;
	public GameObject EnergyShadow;
	public GameObject EnergyGrey;
	public GameObject spiritForm;
	public GameObject physicalForm;
	public GameObject flyFX;
	public Text EnergyIso;
	public Slider degreeSlider;
	 FlightVisualManager FVM;

	public GameObject DailyBlessing;
	public Text blessingText;
	public Text locationEn;

	public GameObject levelUp;
	public Image iconLevelUp;
	public Sprite levelSp;
	public Sprite degreeSprite;
	public Text titleLevelup;
	public Text mainLevelup;


	void Awake ()
	{
		Instance = this;
		FVM = GetComponent<FlightVisualManager> ();
	}

	// ___________________________________________ Main Player UI ________________________________________________________________________________________________

	public void SetupUI()
	{
		EnergyGrey.SetActive (false);
		EnergyShadow.SetActive (false);
		EnergyWhite.SetActive (false);
		Level.text =  PlayerDataManager.playerData.level.ToString();
        if(EnergyIso)
		EnergyIso.text = PlayerDataManager.playerData.energy.ToString ();
        if(Energy)
		Energy.text = PlayerDataManager.playerData.energy.ToString();
		if ( PlayerDataManager.playerData.degree < 0) {
			EnergyShadow .SetActive (true);
		} else if ( PlayerDataManager.playerData.degree > 0) {
			EnergyWhite.SetActive (true);
		} else {
			EnergyGrey.SetActive (true);
		}
		UpdateDrachs ();
//		StartCoroutine (CheckTime ());
//		ShowBlessing();
		SetupDegree ();
	}

	public void playerlevelUp()
	{
		Level.text = PlayerDataManager.playerData.level.ToString();
		levelUp.SetActive (true);
		titleLevelup.text = "You Leveled up!";
		mainLevelup.text = "Level " + Level.text + "!";
		iconLevelUp.sprite = levelSp;
	}

	public void playerDegreeChanged()
	{
		levelUp.SetActive (true);
		titleLevelup.text = "Your Alignment Changed!";
		mainLevelup.text = Utilities.witchTypeControlSmallCaps(PlayerDataManager.playerData.degree);
		iconLevelUp.sprite = degreeSprite;
		SetupDegree ();
	}

	public void SetupDegree()
	{
		degreeSlider.value = PlayerDataManager.playerData.degree;
	}

	public void ShowBlessing()
	{
		Show (DailyBlessing);
	}
	public void HideBlessing()
	{
		Hide (DailyBlessing);
	}

	public void UpdateDrachs()
	{
		silverDrachs.text = PlayerDataManager.playerData.silver.ToString ();
		silverDrachsStore.text = PlayerDataManager.playerData.silver.ToString ();
	}

	public void UpdateEnergy()
	{
		Energy.text = PlayerDataManager.playerData.energy.ToString();
	}

	// ___________________________________________ FLIGHT UI _____________________________________________________________________________________________________

	public void Flight()
	{
		physicalForm.SetActive (false);
		FlightObject.SetActive (true);
		spiritForm.SetActive (true);
		flyFX.SetActive (true);
		FVM.FadeOut ();
	}

	public void Hunt()
	{
		flyFX.SetActive (false);
		FlightObject.SetActive (false);
		FVM.FadeIn ();

	}

	public void home()
	{
		spiritForm.SetActive (false);
		physicalForm.SetActive (true);
	}
		
	IEnumerator CheckTime()
	{
		while (true) {
			if (System.DateTime.Now.Hour == 0 || System.DateTime.Now.Minute == 0 || System.DateTime.Now.Second == 0) {
				//TODO add daily blessing check
			}
		}
	}
}

