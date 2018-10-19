using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[RequireComponent(typeof(SummoningIngredientManager))]
[RequireComponent(typeof(SwipeDetector))]
public class SummoningManager : MonoBehaviour {

	public static SummoningManager Instance{ get; set;}
	public static string currentSpiritID = "";

	public GameObject summonObject;
	public Text FilterDesc;
	public Text spiritDesc;
	public Image spiritIcon;
	public Text spiritTitle;
	public Text summonCost;
	public Text countText;

	[Header("Spirit Info")]
	public GameObject infoObject;
	public Text spiritInfoTitle;
	public Text spiritInfoLore;
	public Text ingredientsReq;
	public Text spiritInfoTier;
	public Text legend;
	public Image SpiritInfoIcon;

	[Header("Auto Ingredients")]
	public GameObject ingredientObject;
	public Text gemText;
	public Text herbText;
	public Text toolText;
	public GameObject gemFX;
	public GameObject herbFX;
	public GameObject toolFX;
	public Text gemTextCount;
	public Text herbTextCount;
	public Text toolTextCount;

	[Space(10)]

	public Transform[] headerItems;

	public GameObject RandomizeLoading;
	SummonFilter filter =  SummonFilter.none;
	public List<string> tempSpList = new List<string> (); 
	public int currentIndex = 0;
	public static bool isOpen = false;
	[HideInInspector]
	public SwipeDetector SD;
	public GameObject loading;

	public GameObject SummonSuccess;
	public Image summonSuccessSpirit;
	public Text headingText; 
	public Text bodyText;  


	public Button summonButton;
	public GameObject[] buttonFX;

	public Button increasePower;

	Coroutine timerRoutine;
	void Awake()
	{
		Instance = this;
		SD = GetComponent<SwipeDetector> ();
		SD.SwipeRight += OnSwipeRight;
		SD.SwipeLeft += OnSwipeLeft;
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0)&& isOpen && !FTFManager.isInFTF) {
			PointerEventData ped = new PointerEventData (null);
			ped.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult> ();
			EventSystem.current.RaycastAll (ped, results);
			foreach (var item in results) {

				if (item.gameObject.tag == "Header") {
					filter = (SummonFilter)System.Enum.Parse (typeof(SummonFilter), item.gameObject.name); 
					InitHeader ();
					item.gameObject.GetComponent<CanvasGroup> ().alpha = 1f;
					item.gameObject.transform.GetChild (0).gameObject.SetActive (true);
					SetPage ();
					SoundManagerOneShot.Instance.PlayButtonTap ();
				} 
			}
		}
	}

	void InitHeader ()
	{
		foreach (var item in headerItems) {
			item.GetComponent<CanvasGroup> ().alpha = .5f;
			item.GetChild (0).gameObject.SetActive (false);
		}
	}

	public void Open()
	{
		SoundManagerOneShot.Instance.MenuSound ();
		SoundManagerOneShot.Instance.PlayWhisper (.2f);
		Show (summonObject);
		InitHeader ();
		RandomizeLoading.SetActive (false);
		filter = SummonFilter.none;
		if (currentSpiritID != "") {
			for (int i = 0; i < PlayerDataManager.playerData.knownSpirits.Count; i++) {
				if (PlayerDataManager.playerData.knownSpirits [i].id == currentSpiritID) {
					currentIndex = i;	
					break;
				}
			}
		} else {
			currentIndex = 0;
		}
		SetPage (false);
		Invoke ("enableBool", 1f);
	}

	void enableBool()
	{
		isOpen = true;
		if (!FTFManager.isInFTF) {
			SD.canSwipe = true;
		} else {
			SD.canSwipe = false;

		}
	}

	public void Exit()
	{
		isOpen = false;
		SD.canSwipe = false;
		Hide (summonObject);

	}

	void SetPage(bool isReset = true)
	{
		var knownList = PlayerDataManager.playerData.knownSpirits;
		tempSpList.Clear ();
		if(isReset)
		currentIndex = 0;
		if (filter == SummonFilter.none) {
			tempSpList = knownList.Select(l=> l.id).ToList();
			FilterDesc.text = "";
		
		} else {
			FilterDesc.text = DownloadedAssets.spiritTypeDict [filter.ToString ()].value;
			foreach (var item in knownList) {
				foreach (var tag in item.tags) {
					if (tag == filter.ToString ()) {
						tempSpList.Add (item.id);
					}
				}
			}
		}

		currentSpiritID = tempSpList [currentIndex];
		spiritIcon.sprite = DownloadedAssets.spiritArt [currentSpiritID];
		spiritTitle.text = DownloadedAssets.spiritDictData [currentSpiritID].spiritName;
		countText.text = (currentIndex +1).ToString() + "/" + (tempSpList.Count).ToString ();
		spiritDesc.text = DownloadedAssets.spiritDictData [currentSpiritID].spriitBehavior;
		if (!SummoningIngredientManager.AddBaseIngredients ()) {
			summonButton.interactable = false;
			increasePower.interactable = false;
			foreach (var item in buttonFX) {
				item.SetActive (false);
			}
		} else {
			summonButton.interactable = true;
			increasePower.interactable = true;
			foreach (var item in buttonFX) {
				item.SetActive (true);
			}
		}
		summonCost.text = "Cost : " + PlayerDataManager.config.summoningCosts [DownloadedAssets.spiritDictData [currentSpiritID].spiritTier - 1].ToString() + " Energy";
	}

	void OnSwipeLeft()
	{
		SoundManagerOneShot.Instance.PlayWhisper ();
		if (currentIndex < tempSpList.Count-1) {
			currentIndex++;
		} else {
			currentIndex = 0;
		}
		SetPage (false);
	}

	void OnSwipeRight()
	{
			SoundManagerOneShot.Instance.PlayWhisper ();

			if (currentIndex > 0) {
				currentIndex--;
			} else {
				currentIndex = tempSpList.Count - 1;
			}
			SetPage (false);
	}

	public void ShowMoreInfo()
	{
		SoundManagerOneShot.Instance.MenuSound ();
		SoundManagerOneShot.Instance.PlayButtonTap ();
		Show (infoObject);
		spiritInfoTitle.text = DownloadedAssets.spiritDictData [currentSpiritID].spiritName;
		spiritInfoLore.text = DownloadedAssets.spiritDictData [currentSpiritID].spiritDescription;
		SpiritInfoIcon.sprite = DownloadedAssets.spiritArt [currentSpiritID];

		string kind ="";
		if (DownloadedAssets.spiritDictData[currentSpiritID].spiritTier == 1) {
			kind = "Common";
		} else if (DownloadedAssets.spiritDictData[currentSpiritID].spiritTier == 2) {
			kind = "Less Common";
		} else if (DownloadedAssets.spiritDictData[currentSpiritID].spiritTier == 3) {
			kind = "Rare";
		} else {
			kind = "Exotic";
		}
		spiritInfoTier.text = kind;
		legend.text = DownloadedAssets.spiritDictData [currentSpiritID].spiritLegend;

		//TODO add ingredients here
		var reqIng= PlayerDataManager.summonMatrixDict [SummoningManager.currentSpiritID];
		string s = "";
		s += (reqIng.gem == "" ? "" : " " +DownloadedAssets.ingredientDictData [reqIng.gem].name);
		s += (reqIng.herb == "" ? "" : " " +DownloadedAssets.ingredientDictData [reqIng.herb].name);
		s += (reqIng.tool == "" ? "" : " " +DownloadedAssets.ingredientDictData [reqIng.tool].name);
		ingredientsReq.text = (s == ""?"None.":s);
	}

	public void CloseMoreInfo()
	{
		SoundManagerOneShot.Instance.MenuSound ();
		SoundManagerOneShot.Instance.PlayButtonTap ();
		Hide (infoObject);
	}

	#region Animation

	void Show(GameObject g)
	{
		if(g.activeInHierarchy)
			g.SetActive (false);
		var anim = g.GetComponent<Animator> ();
		g.SetActive (true);
		anim.Play ("in");
	}

	void Hide(GameObject g, bool isDisable = true)
	{
		if(isDisable)
			StopCoroutine("DisableObject");

		if (g.activeInHierarchy) {
			var anim = g.GetComponent<Animator> ();
			anim.Play ("out");
			if(isDisable)
				StartCoroutine (DisableObject(g));
		}
	}

	IEnumerator DisableObject(GameObject g){
		yield return new WaitForSeconds (.55f);
		g.SetActive (false);
	}
	#endregion


	public void CastSummon(  )
	{
		SoundManagerOneShot.Instance.SummonRiser ();
//		SoundManagerOneShot.Instance.PlaySpellFX();

		loading.SetActive (true);
		var data = new {spiritId = currentSpiritID, ingredients = GetIngredients()}; 
		SummoningIngredientManager.ClearIngredient ();
	
		APIManager.Instance.PostCoven ("spirit/summon", JsonConvert.SerializeObject(data), (string s , int r)=>{
			loading.SetActive(false);
			print(s);
			if(r == 200){
				SoundManagerOneShot.Instance.SpiritSummon();
				Exit();
				JObject d = JObject.Parse(s);
				ShowSpiritCastResult(true,double.Parse(d["summonOn"].ToString()));
			}
		});
	}

	void ShowSpiritCastResult(bool success, double result)
	{
		if (success) {
			SummonSuccess.SetActive (true);
			headingText.text = "Summoning Successful";
			bodyText.text = spiritTitle.text + " will summon in " + Utilities.GetTimeRemaining (result);
			summonSuccessSpirit.sprite = spiritIcon.sprite;
			try{
			if(timerRoutine != null)
			StopCoroutine (timerRoutine);
			}catch{
			}
		  timerRoutine = StartCoroutine (StartTimer (result));
		} else {
			//handle fail;
		}
	}



	IEnumerator StartTimer(double result)
	{
		if (FTFManager.isInFTF) {
			result = (System.DateTime.UtcNow.AddSeconds (5)).Subtract (new System.DateTime (1970, 1, 1)).TotalMilliseconds; 
		}
		while (true) {
			if (Utilities.GetTimeRemaining (result) != "null") {
				bodyText.text = spiritTitle.text + " will summon in " + Utilities.GetTimeRemaining (result);
			} else {
				SummonSuccess.SetActive (false);
			}
			yield return new WaitForSeconds (1);
		}
	}

	List<spellIngredientsData> GetIngredients()
	{
		var data = new SpellTargetData ();
		data.ingredients = new List<spellIngredientsData> ();

		if (SummoningIngredientManager.addedTool != "") {
			data.ingredients.Add (new spellIngredientsData{ id = SummoningIngredientManager.addedTool, count = SummoningIngredientManager.addedToolCount});
		}

		if (SummoningIngredientManager.addedHerb != "") {
			data.ingredients.Add (new spellIngredientsData{ id =  SummoningIngredientManager.addedHerb, count =  SummoningIngredientManager.addedHerbCount});
		}
		if (SummoningIngredientManager.addedGem != "") {
			data.ingredients.Add (new spellIngredientsData{ id =  SummoningIngredientManager.addedGem, count =  SummoningIngredientManager.addedGemCount});
		}
		return 	data.ingredients;
	}

}

public enum SummonFilter{
	forbidden,harvester,healer, protector,trickster, warrior,guardian,familiar,none
}
