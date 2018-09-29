using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;

public class PlayerManagerUI : UIAnimationManager
{
	public static PlayerManagerUI Instance { get; set;}
	
	[Header("Flight")]
	public GameObject FlightObject;

	[Header("PlayerInfo UI")]
	public Text Level;
	public Text Energy;
	public Slider EnergySlider;
	public GameObject overFlowEn;
	public Text silverDrachs;
	public Text silverDrachsStore;
	public GameObject spiritForm;
	public GameObject physicalForm;
	public GameObject flyFX;
	public Text EnergyIso;
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
	public Image LunarPhaseHolder;
	public Sprite[] LunarPhase;
	public Slider xpSlider;
	public Text xpText;
	void Awake ()
	{
		Instance = this;
		FVM = GetComponent<FlightVisualManager> ();
	}

	// ___________________________________________ Main Player UI ________________________________________________________________________________________________

	public void SetupUI()
	{

		Level.text =  PlayerDataManager.playerData.level.ToString();
        
		EnergyIso.text = PlayerDataManager.playerData.energy.ToString ();
       
//		Energy.text = PlayerDataManager.playerData.energy.ToString() + PlayerDataManager.pla;
		SetupEnergy();
		UpdateDrachs ();
		StartCoroutine (CheckTime ());
		SetupAlignmentPhase ();
		setupXP ();
	}

	void SetupEnergy()
	{
		var pData = PlayerDataManager.playerData;
		if (pData.baseEnergy >= pData.energy) {
			Energy.text = pData.energy.ToString () + "/" + pData.baseEnergy;
			EnergySlider.maxValue = pData.baseEnergy;
			EnergySlider.value = pData.energy;
		} else {
			overFlowEn.SetActive (true);
			EnergySlider.maxValue = pData.baseEnergy;
			EnergySlider.value =pData.baseEnergy;
			Energy.text = "<b>"+pData.energy.ToString () + "</b>/" + pData.baseEnergy;
		}
	}

	public void setupXP()
	{
		xpSlider.maxValue = PlayerDataManager.playerData.xpToLevelUp ;
		xpSlider.value = PlayerDataManager.playerData.xp;
		xpText.text	 = PlayerDataManager.playerData.xp.ToString () + "/" + PlayerDataManager.playerData.xpToLevelUp.ToString();
	}

	void SetupAlignmentPhase()
	{
		var lp = PlayerDataManager.playerData.degree;
		if (lp == 0)
			LunarPhaseHolder.sprite = LunarPhase [7];
		if (lp == 1 || lp == 2)
			LunarPhaseHolder.sprite= LunarPhase [8];
		if (lp == 3 || lp == 4)
			LunarPhaseHolder.sprite= LunarPhase [9];
		if (lp == 5 || lp == 6)
			LunarPhaseHolder.sprite= LunarPhase [10];
		if (lp == 7 || lp == 8)
			LunarPhaseHolder.sprite= LunarPhase [11];
		if (lp == 9 || lp == 10)
			LunarPhaseHolder.sprite= LunarPhase [12];
		if (lp == 11 || lp == 12)
			LunarPhaseHolder.sprite = LunarPhase[13];
		if (lp == 13 || lp == 14)
			LunarPhaseHolder.sprite= LunarPhase [14];


		if (lp == -1 || lp == -2)
			LunarPhaseHolder.sprite = LunarPhase[6];
		if (lp == -3 || lp == -4)
			LunarPhaseHolder.sprite = LunarPhase[5];
		if (lp == -5 || lp == -6)
			LunarPhaseHolder.sprite= LunarPhase [4];
		if (lp == -7 || lp == -8)
			LunarPhaseHolder.sprite = LunarPhase[3];
		if (lp == -9 || lp == -10)
			LunarPhaseHolder.sprite = LunarPhase[2];
		if (lp == -11 || lp == -12)
			LunarPhaseHolder.sprite = LunarPhase[1];
		if (lp == -13 || lp == -14)
			LunarPhaseHolder.sprite= LunarPhase [0];
	}

	public void playerlevelUp()
	{
		Level.text = PlayerDataManager.playerData.level.ToString();
		levelUp.SetActive (true);
		titleLevelup.text = "You Leveled up!";
		mainLevelup.text = "Level " + Level.text + "!";
		iconLevelUp.sprite = levelSp;
		setupXP ();
	}

	public void playerDegreeChanged()
	{
		levelUp.SetActive (true);
		titleLevelup.text = "Your Alignment Changed!";
		mainLevelup.text = Utilities.witchTypeControlSmallCaps(PlayerDataManager.playerData.degree);
		iconLevelUp.sprite = degreeSprite;
		SetupAlignmentPhase ();
	}



	void SetupBlessing()
	{
		blessingText.text = "The Dea Savannah Grey has granted you her daily gift of " + PlayerDataManager.playerData.blessing.daily.ToString () + " energy";
		if (PlayerDataManager.playerData.blessing.locations > 0) {
			locationEn.text = "You also gained " + PlayerDataManager.playerData.blessing.locations.ToString () + " energy from your Places of Power";
		} else {
			locationEn.text = "";
		}
	}

	public void ShowBlessing()
	{
		SetupBlessing ();
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
		SetupEnergy ();
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
			if (System.DateTime.Now.Hour == 0 && System.DateTime.Now.Minute == 0 && System.DateTime.Now.Second == 0) {
				//TODO add daily blessing check
				yield return new WaitForSeconds (1);
				print("Checking Reset");
				APIManager.Instance.GetData ("character/get",(string s, int r)=>{

					if(r == 200){
						var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(s);
						if(rawData.dailyBlessing){
							PlayerDataManager.playerData.blessing = rawData.blessing;
							ShowBlessing();
						}
					}


				});
			}
			yield return new WaitForSeconds (1);
		}
	}
}

