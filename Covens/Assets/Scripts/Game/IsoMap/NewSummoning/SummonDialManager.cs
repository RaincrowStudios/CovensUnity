using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SummonDialManager : MonoBehaviour
{

	public static SummonDialManager Instance { get; set; }
	public static bool isSummonOn = false;
	public GameObject summonContainer;
	public GameObject RotDialText;
	public GameObject NextButton;
	public GameObject BackButton;
	public RectTransform summonCenter;
	public Transform rotateWheel;
	public Text spiritName;
	public Text summonTitle;
	public Text spiritDesc;
	public Text spiritBehave;
	public GameObject rotateDialArrows;

	public Image spiritPic;

	public float speed = 1;
	public float rotateSpeed = 1;
	public float MaxSpeed = 18;
	public float centerChangeSpeed = 3f;
	public float spiritChangeSpeed = 1;
	public RectTransform[] ingButtons;

	int direction = 0;
	float tempSpeed = 0;
	bool enableButton = false;
	int curIndex = 0;

	public Transform GemContainer;
	bool isMain = true;


	public GameObject gemText;
	public SummonHerbScroller SHS;

	public Text AddedGemText;
	public Text AddedHerbText;

	public static int addedGems;
	public static int addedHerbs; 

	public static string addedGemsID = "";
	public static string addedHerbsID = ""; 
	public Animator anim;
	public Text WarningText;
	public GameObject loading;

	string selectedTool;

	public GameObject SummonSuccess;
	public Image summonSuccessSpirit;
	public Text headingText; 
	public Text bodyText;  

	List<string> knownSpirits = new List<string> ();
	public Text CastSummoningButtonText;

	void Awake ()
	{
		Instance = this;
	}

	public void Initiate ()
	{
		this.CancelInvoke ();
		isMain = true;
		SoundManagerOneShot.Instance.MenuSound ();
		foreach (Transform item in GemContainer) {
			Destroy (item.gameObject);
		}
		isSummonOn = true;
		UIStateManager.Instance.CallWindowChanged(false);
		foreach (var item in PlayerDataManager.playerData.ingredients.toolsDict) { 
			if (PlayerDataManager.playerData.KnownSpiritsList.Contains (PlayerDataManager.ToolsSpiritDict [item.Key])) {
				knownSpirits.Add(PlayerDataManager.ToolsSpiritDict [item.Key]); 
			}
		}
		knownSpirits.Sort ();
		loading.SetActive (false);
		AddedGemText.text = "";
		AddedHerbText.text = "";
		summonContainer.SetActive (true);
		anim.Play ("in");
		rotateDialArrows.SetActive (true);
		NextButton.SetActive (false);
		spiritPic.sprite = DownloadedAssets.spiritArt [knownSpirits[0]];
		spiritDesc.text = DownloadedAssets.spiritDictData [knownSpirits[0]].spiritDescription;
		spiritName.text = DownloadedAssets.spiritDictData [knownSpirits[0]].spiritName;
		spiritBehave.text = DownloadedAssets.spiritDictData [knownSpirits[0]].spriitBehavior;
		enableButton = false;
		Invoke ("EnableButtons", .2f);
		summonCenter.anchoredPosition = new Vector2 (0, 0);
		summonTitle.text = "Summoning";
		NextButton.SetActive (false);
		rotateDialArrows.SetActive (true);
		BackButton.SetActive (false);
		SpawnGems();
		SHS.InitScroll ();
		WarningText.text = "";
		rotateWheel.localEulerAngles = Vector3.zero;
		foreach (var item in ingButtons) {
			item.localEulerAngles = Vector3.zero;
		}

	}

	void EnableButtons()
	{
		enableButton = true;
	}

	void Update ()
	{
		if (!isSummonOn)
			return;
		if (Input.GetMouseButtonUp (0) && enableButton) {
			rotateDialArrows.SetActive (false);
			NextButton.SetActive (true);
			enableButton = false;	
		}
		if (Input.GetMouseButton (0)) {

			if (isMain) {
				PointerEventData ped = new PointerEventData (null);
				ped.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult> ();
				EventSystem.current.RaycastAll (ped, results);
				foreach (var item in results) {

					if (item.gameObject.name == "next") {
						return;
					} 
				}

			rotateSpeed = Input.GetAxis ("Mouse Y") * speed;
			if (rotateSpeed > 0)
				direction = 1;
			else
				direction = -1;

			rotateSpeed = Mathf.Clamp (Mathf.Abs (rotateSpeed), 0, MaxSpeed);
			rotateWheel.Rotate (0, 0, rotateSpeed * direction);
			foreach (var item in ingButtons) {
				item.Rotate (0, 0, rotateSpeed * direction);
			}

			int rSpeed = Mathf.RoundToInt ( Input.GetAxis ("Mouse Y") * spiritChangeSpeed);

			if (rSpeed == 0 ) {
				tempSpeed += Input.GetAxis ("Mouse Y") * spiritChangeSpeed;
			} else {
				tempSpeed = Input.GetAxis ("Mouse Y") * spiritChangeSpeed;
			}

			if (tempSpeed>=1) {
				if (curIndex + Mathf.RoundToInt (tempSpeed) < PlayerDataManager.playerData.KnownSpiritsList.Count) {
					curIndex += Mathf.RoundToInt (tempSpeed);
					changeSpiritArt ();

				} else {
					curIndex = 0;
					changeSpiritArt ();
				}
				tempSpeed = 0;
			}

			if (tempSpeed<=-1) {
				if (curIndex+- Mathf.RoundToInt (tempSpeed) > 0) {
					curIndex += Mathf.RoundToInt (tempSpeed);

					changeSpiritArt ();

				} else {
					curIndex = PlayerDataManager.playerData.KnownSpiritsList.Count-1;
					changeSpiritArt ();
				}
				tempSpeed = 0;
			}
			}
	}
		if (Input.GetMouseButtonDown (0)) {
			if (!isMain) {
				PointerEventData ped = new PointerEventData (null);
				ped.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult> ();
				EventSystem.current.RaycastAll (ped, results);
				foreach (var item in results) {

					if (item.gameObject.tag == "Herb") {
						TryAddingHerb (item.gameObject.name);
					} 
					if (item.gameObject.tag == "Gem") {
						TryAddingGem (item.gameObject.name);
					}
				}

			}

		}
	}

	void TryAddingGem(string curGem)
	{
		if (addedGemsID == curGem) {
			if (addedGems >= 5) {
				WarningText.text = "Cannot add more than 5 items";
				return;
			}
			if (PlayerDataManager.playerData.ingredients.gemsDict [curGem].count > 0) {
				addedGems++;
				PlayerDataManager.playerData.ingredients.gemsDict [curGem].count--;
				AddedGemText.text = DownloadedAssets.ingredientDictData [curGem].name + " (" + addedGems.ToString () + ")";
			} else {
				WarningText.text = "Out of " + DownloadedAssets.ingredientDictData [curGem].name;
			}
		} else {
			addedGems = 1;
			addedGemsID = curGem;
			PlayerDataManager.playerData.ingredients.gemsDict [curGem].count--;
			AddedGemText.text = DownloadedAssets.ingredientDictData [curGem].name + " (" + addedGems.ToString () + ")";
		}
	}

	void TryAddingHerb(string curHerb)
	{
		if (addedHerbsID == curHerb) {
			if (addedHerbs >= 5) {
				WarningText.text = "Cannot add more than 5 items";
				return;
			}
			if (PlayerDataManager.playerData.ingredients.herbsDict [curHerb].count > 0) {
				addedHerbs++;
				PlayerDataManager.playerData.ingredients.herbsDict [curHerb].count--;
				AddedHerbText.text = DownloadedAssets.ingredientDictData [curHerb].name + " (" + addedHerbs.ToString() + ")";
			} else {
				WarningText.text = "Out of " + DownloadedAssets.ingredientDictData [curHerb].name;
			}
		}else {
			addedHerbs = 1;
			addedHerbsID = curHerb;
			PlayerDataManager.playerData.ingredients.herbsDict [curHerb].count--;
			AddedHerbText.text = DownloadedAssets.ingredientDictData [curHerb].name + " (" + addedHerbs.ToString () + ")";
		}
	}

	public void OnCastSummon()
	{
		CastSummon ();
	}

	public void Close()
	{
		SoundManagerOneShot.Instance.MenuSound ();

		if (addedHerbsID != "") {
			PlayerDataManager.playerData.ingredients.herbsDict [addedHerbsID].count += addedHerbs;
		}

		if (addedGemsID != "") {
			PlayerDataManager.playerData.ingredients.gemsDict [addedGemsID].count += addedGems;
		}
	
		anim.Play ("out");
		addedGems = addedHerbs = 0;
		AddedHerbText.text = AddedGemText.text = addedHerbsID = addedGemsID = "";
		Invoke ("DisableObject", 1.4f);
		print ("close");
		UIStateManager.Instance.CallWindowChanged(true);
		isSummonOn = false;
	}

	void DisableObject()
	{
		summonContainer.SetActive (false);
	}

	void SpawnGems()
	{
		foreach (var item in PlayerDataManager.playerData.ingredients.gemsDict) {
			var k = Utilities.InstantiateObject (gemText, GemContainer);
			k.GetComponent<Text> ().text = DownloadedAssets.ingredientDictData [item.Key].name;
			k.name = item.Key;
		}
	}

	void changeSpiritArt ()
	{
		try {
			spiritPic.sprite = DownloadedAssets.spiritArt [knownSpirits[curIndex]];
			spiritDesc.text = DownloadedAssets.spiritDictData [knownSpirits[curIndex]].spiritDescription;
			spiritName.text = DownloadedAssets.spiritDictData [knownSpirits[curIndex]].spiritName;
			selectedTool = PlayerDataManager.SpiritToolsDict[knownSpirits[curIndex]];
			spiritBehave.text = DownloadedAssets.spiritDictData[knownSpirits[curIndex]].spriitBehavior;
		} catch {
		
		}
	}

	public void OnContinue(bool isForward){
		StartCoroutine (move (isForward));
	}

	IEnumerator move (bool isForward)
	{
		SoundManagerOneShot.Instance.MenuSound ();

	SetupButtons (!isForward);
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime*centerChangeSpeed;
		if (isForward) {
			summonCenter.anchoredPosition = new Vector2 (Mathf.SmoothStep (0, -2638,t), 0);
		} else {
			summonCenter.anchoredPosition = new Vector2 (Mathf.SmoothStep (-2638, 0,t), 0);
		}
			yield return 0;
		}


	}

	void SetupButtons(bool ismain)
	{
		if (!ismain) {
			CastSummoningButtonText.text = "Summon your " + spiritName.text;
		}
		isMain = ismain;
		enableButton = isMain;
		NextButton.SetActive (isMain);
		BackButton.SetActive (!isMain);
		summonTitle.text = (isMain?"Summoning": "Empower your spirits");
		RotDialText.SetActive (isMain);
	}

	void CastSummon(  )
	{
		loading.SetActive (true);
		var data = new {latitude = OnlineMaps.instance.position.y,longitude =  OnlineMaps.instance.position.x, ingredients = GetIngredients()}; 
		addedGemsID = "";
		addedHerbsID = "";
		if (PlayerDataManager.playerData.ingredients.toolsDict [selectedTool].count > 1) {
			PlayerDataManager.playerData.ingredients.toolsDict [selectedTool].count--;
		} else {
			PlayerDataManager.playerData.ingredients.toolsDict.Remove (selectedTool);
		}
		selectedTool = "";
		APIManager.Instance.PostCoven ("spirit/summon", JsonConvert.SerializeObject(data), (string s , int r)=>{
			loading.SetActive(false);
			print(s);
			if(r == 200){
				Close();
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
			bodyText.text = spiritName.text + " will summon in " + Utilities.GetTimeRemaining (result);
			summonSuccessSpirit.sprite = spiritPic.sprite;
			StartCoroutine (StartTimer (result));
		} else {
			//handle fail;
		}
	}

	public void StopTimerUpdate()
	{
		StopCoroutine ("StartTimer");
	}

	IEnumerator StartTimer(double result)
	{
		while (true) {
			if (Utilities.GetTimeRemaining(result) != "null") {
				bodyText.text = spiritName.text + " will summon in " + Utilities.GetTimeRemaining (result);
			}
			yield return new WaitForSeconds (1);
		}
	}

	List<spellIngredientsData> GetIngredients()
	{
		var data = new SpellTargetData ();
		data.ingredients = new List<spellIngredientsData> ();

		data.ingredients.Add (new spellIngredientsData{ id = selectedTool, count = 1 });
		if (addedHerbsID != "") {
			data.ingredients.Add (new spellIngredientsData{ id = addedHerbsID, count = addedHerbs});
		}
		if (addedGemsID != "") {
			data.ingredients.Add (new spellIngredientsData{ id = addedGemsID, count = addedGems});
		}
		return 	data.ingredients;
	}

}

