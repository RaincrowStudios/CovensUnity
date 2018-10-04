using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class SummonUIManager : UIAnimationManager
{
	public static SummonUIManager Instance{ get; set;}
	public float rotateSpeed =1;
	public Transform summonWheel;

	public ParticleSystem[] herbPS;
	public ParticleSystem[] gemPS;
	public ParticleSystem[] toolPS;
	public ParticleSystem HaloCenter;

	[Header("Ingredients")]
	public Text ingredientTitle;
	public GameObject textPrefab;
	public Transform container;
	public Animator anim;
	public GameObject addedHerb;
	public GameObject addedTool;
	public GameObject addedGem;
	public Text addedHerbText;
	public Text addedToolText;
	public Text addedGemText;
	public GameObject clearItem;
	public Text WarningItem;

	[Header("Spirit")]
	public Text spiritName;
	public Animator spiritAnim;
	public Text spiritDesc;
	public Image spiritArt;

	public Animator summonAnim;
	public float SummonBackTime = 1;
	public static string selectedTool =null;
	IngredientType type;

	public GameObject Desc;
	public GameObject Timer;

	public GameObject CastSummonButton;
	public GameObject closeButton;

	public CanvasGroup Focus;
	public GameObject addTool;
	public GameObject gemHint;
	public GameObject arrow1;
	public GameObject arrow2;
	bool canSummon = false;
	void Awake()
	{
		Instance = this;
	}

	void Start ()
	{
		SetAllPS (false);
	}

	public void Init()
	{
		Desc.SetActive (true);
		Timer.SetActive (false);
		clearItem.SetActive (false);
		WarningItem.gameObject.SetActive (false);
		RotateSummonWheel ("Tool");
		if (selectedTool != null) {
			setupSpiritUI (); 
		}
		SetPS (HaloCenter, true);
		anim.SetBool ("in", true);
		closeButton.SetActive (true);
		StartCoroutine (FadeInFocus ());
	}

	IEnumerator FadeInFocus ()
	{
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime;
			Focus.alpha = Mathf.SmoothStep (0, 1, t);
			yield return 0;
		}
		CastSummonButton.SetActive (false);
		addTool.SetActive (true);
		arrow1.SetActive (false);
		arrow2.SetActive (false);
		gemHint.SetActive (false);
	}

	IEnumerator FadeOutFocus ()
	{
		if (Focus.alpha == 0)
			yield break;
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime;
			Focus.alpha = Mathf.SmoothStep (1, 0, t);
			yield return 0;
		}
	}


	void SetAllPS(bool show)
	{
		foreach (var item in herbPS) {
			SetPS (item, show);
		}
		foreach (var item in gemPS) {
			SetPS (item, show);
		}
		foreach (var item in toolPS) {
			// list of known spirits
			//dict of tools/spirits ;

			SetPS (item, show);
		}
	}



	void SetPS(ParticleSystem p, bool show)
	{
		var ps = p.emission;
		ps.enabled = show;
	}
	// Update is called once per frame
	void Update ()
	{
		if (summonWheel.gameObject.activeInHierarchy) {
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					RotateSummonWheel (hit.collider.gameObject.name);
				} 

				PointerEventData ped = new PointerEventData (null);
				ped.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult> ();
				EventSystem.current.RaycastAll(ped, results);
				foreach (var item in results) {
					if (item.gameObject.tag == "SummonData") {
						OnClick (item.gameObject.GetComponent<summonIngredientData> ());
						return;
					}
				}
			}
		}
	}

	void RotateSummonWheel(string id)
	{
		if (selectedTool == null) {
//			print (selectedTool);
//			print (id);
			if (id == "Herb" || id == "Gem") {
				WarningItem.gameObject.SetActive (true);
				WarningItem.text = "Please select a tool before continuing";
				return;
			} else {
				WarningItem.gameObject.SetActive (false);
			}
		}

		SetAllPS (false);

		if (id == "Herb") {
			arrow1.SetActive (false);
			arrow2.SetActive (false);
			StartCoroutine (rotateWheel (-160));
			foreach (var item in herbPS) {
				SetPS (item, true);
			}
			if ("herb" != type.ToString ()) {
				WarningItem.gameObject.SetActive (false);
				anim.SetBool ("in", true);
			}
			type = IngredientType.herb;
			DisplayList ();
		} else if (id == "Tool") {
			foreach (var item in toolPS) {
				SetPS (item, true);
			}
			StartCoroutine (rotateWheel (-40));
			if ("tool" != type.ToString ()) {
				WarningItem.gameObject.SetActive (false);
				anim.SetBool ("in", true);
			}
			type = IngredientType.tool;
			DisplayList ();
		} else if (id == "Gem") {
			arrow1.SetActive (false);
			arrow2.SetActive (false);
			foreach (var item in gemPS) {
				SetPS (item, true);
			}
			StartCoroutine (rotateWheel (80));
			if ("gem" != type.ToString ()) {
				WarningItem.gameObject.SetActive (false);
				anim.SetBool ("in", true);
			}
			type = IngredientType.gem;
			DisplayList ();
		} else {
				PointerEventData ped = new PointerEventData (null);
				ped.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult> ();
				EventSystem.current.RaycastAll (ped, results);
				bool hide = true;
				foreach (var item in results) {
				if (item.gameObject.name == "Scroll" || item.gameObject.name == "Clear") {
						hide = false;
					if (type == IngredientType.gem) {
						foreach (var k in gemPS) {
							SetPS (k, true);
						}
					} else if (type == IngredientType.tool) {
						foreach (var k in toolPS) {
							SetPS (k, true);
						}
					} else {
						foreach (var k in herbPS) {
							SetPS (k, true);
						}
					}
					}
				}
			if (hide) {
				anim.SetBool ("in", false);
				type = IngredientType.none;
			}
		}

	}

	IEnumerator rotateWheel(float rot)
	{
		Quaternion iniRot = summonWheel.transform.rotation;
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * rotateSpeed;
			summonWheel.transform.rotation = Quaternion.Lerp (iniRot, Quaternion.Euler (90, 0, rot), t);
			yield return null;
		}
	}

	void DisplayList( )
	{
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
		if (type == IngredientType.herb) {
			ingredientTitle.text = "Botanicals";
			foreach (var item in PlayerDataManager.playerData.ingredients.herbsDict) {
				var d = spawn (item.Value);
				d.title.text += " (" + item.Value.count.ToString () + ")";
			}
		} else if (type == IngredientType.gem) {
			ingredientTitle.text = "Gems";
			foreach (var item in PlayerDataManager.playerData.ingredients.gemsDict) {
				var d = spawn (item.Value);
				d.title.text += " (" + item.Value.count.ToString () + ")";
			}
		} else {
			ingredientTitle.text = "Tools";
			foreach (var item in PlayerDataManager.playerData.KnownSpiritsList) {
			}
			foreach (var item in PlayerDataManager.playerData.ingredients.toolsDict) {
				if (item.Key != selectedTool) {

					if(PlayerDataManager.playerData.KnownSpiritsList.Contains(PlayerDataManager.ToolsSpiritDict [item.Key])){
						if (DownloadedAssets.spiritArt.ContainsKey (PlayerDataManager.ToolsSpiritDict [item.Key])) {
							spawn (item.Value);
						}
					}
				}
			}
		}
	}

	summonIngredientData spawn(InventoryItems item)
	{
		var g = Utilities.InstantiateObject (textPrefab, container);
		var d = g.GetComponent<summonIngredientData> ();
		d.title.text = DownloadedAssets.ingredientDictData [item.id].name ;
		d.id = item.id ;
		d.type = type;
//		d.onSelectItem = OnClick;
		return d;
	}

	void OnClick(summonIngredientData sd) 
	{
		if (type == IngredientType.tool) {
			if (selectedTool != null) {
				RepopulateItem ();
			}
			selectedTool = sd.id;
			Show (addedTool);
			addedToolText.text = sd.title.text;
			PlayerDataManager.playerData.ingredients.toolsDict [selectedTool].count -= 1;
			if (PlayerDataManager.playerData.ingredients.toolsDict [selectedTool].count == 0)
				PlayerDataManager.playerData.ingredients.toolsDict.Remove (selectedTool);
			setupSpiritUI ();
			gemHint.SetActive (true);
			arrow1.SetActive (true);
			arrow2.SetActive (true);
			addTool.SetActive (false);
			StartCoroutine (FadeOutFocus ());
			Show (CastSummonButton,false);
			Destroy (sd.gameObject); 
		} else if (type == IngredientType.herb) {
			
			int i = IngredientsSpellManager.AddItem (sd.id, type);
			if (i == 0) {
				sd.title.text =  DownloadedAssets.ingredientDictData [sd.id].name + " (" + PlayerDataManager.playerData.ingredients.herbsDict [sd.id].count.ToString () + ")";
				Show (addedHerb);
				addedHerbText.text = DownloadedAssets.ingredientDictData [sd.id].name + " (" + IngredientsSpellManager.AddedHerb.Value.ToString() + ")";
			}
			print ("added " + DownloadedAssets.ingredientDictData [sd.id].name);
			if (i == 2) {
				WarningItem.gameObject.SetActive (true);
				WarningItem.text = "You can only use a maximum of 5 items.";
			}
			if (i == 3) {
				WarningItem.gameObject.SetActive (true);
				WarningItem.text = "Clear the current ingredient before adding a new one.";
			}
		} else if (type == IngredientType.gem) {

			int i = IngredientsSpellManager.AddItem (sd.id, type);
			if (i == 0) {
				sd.title.text =  DownloadedAssets.ingredientDictData [sd.id].name + " (" + PlayerDataManager.playerData.ingredients.gemsDict [sd.id].count.ToString () + ")";
				Show (addedGem);
				addedGemText.text = DownloadedAssets.ingredientDictData [sd.id].name + " (" + IngredientsSpellManager.AddedGem.Value.ToString() + ")";
			}
			if (i == 2) {
				WarningItem.gameObject.SetActive (true);
				WarningItem.text = "You can only use a maximum of 5 items.";
			}
			if (i == 3) {
				WarningItem.gameObject.SetActive (true);
				WarningItem.text = "Clear the current ingredient before adding a new one.";
			}
			print ("added " + DownloadedAssets.ingredientDictData [sd.id].name);
		}
		clearItem.SetActive (true);
	}

	void setupSpiritUI()
	{
		SpiritDict spiritData = DownloadedAssets.spiritDictData [PlayerDataManager.ToolsSpiritDict [selectedTool]];  
		spiritArt.sprite = DownloadedAssets.spiritArt[spiritData.spiritID];
		spiritName.text = spiritData.spiritName;
		spiritDesc.text = spiritData.spiritDescription;
		spiritAnim.SetBool ("in", true);
	}

	public void Close()
	{

		arrow1.SetActive (false);
		arrow2.SetActive (false);
		gemHint.SetActive (false);
		addTool.SetActive (false);
		closeButton.SetActive (false);
		addedHerb.SetActive (false);
		addedGem.SetActive (false);
		addedTool.SetActive (false);
		StartCoroutine (FadeOutFocus ());
		IngredientsSpellManager.ClearCachedItems (IngredientType.gem);
		IngredientsSpellManager.ClearCachedItems (IngredientType.herb);
		spiritAnim.SetBool ("in", false);
		anim.SetBool ("in", false);
		selectedTool = null;
	}

	public void Clear()
	{
		WarningItem.gameObject.SetActive (false);
		clearItem.SetActive (false);
		spiritAnim.SetBool ("in", false);

		if (type == IngredientType.tool) {
			RepopulateItem ();
			Hide (addedTool);
			Hide (CastSummonButton);
		} else if (type == IngredientType.herb) {
			IngredientsSpellManager.ClearCachedItems (IngredientType.herb);
			Hide (addedHerb);
			DisplayList ();
		} else {
			IngredientsSpellManager.ClearCachedItems (IngredientType.gem);
			Hide (addedGem);
			DisplayList ();
		}
	}

	void RepopulateItem ()
	{
		if (type == IngredientType.tool) {
			if (selectedTool != null) {
				var k = new InventoryItems ();
				k.id = selectedTool;
				spawn (k);
				PlayerDataManager.playerData.ingredients.toolsDict [k.id].count += 1;
				selectedTool = null;
			}
		}
	}

	public void Cast()
	{
		Hide (CastSummonButton);
		StartCoroutine (CastSummoning ());
		closeButton.SetActive (false);
	}

	IEnumerator CastSummoning() 
	{
		arrow1.SetActive (false);
		arrow2.SetActive (false);
		gemHint.SetActive (false);
		SpellCastAPI.CastSummon ();
		Hide (addedGem);
		Hide (addedHerb);
		Hide (addedTool);
		SetPS (HaloCenter, false);
		SetAllPS (false);
		SummonMapSelection.Instance.MoveCamCast ();
		summonAnim.SetBool ("animate", true);
		yield return new WaitForSeconds (2f);


		yield return new WaitForSeconds (SummonBackTime);
		SummonMapSelection.Instance.CastGoBack();
		SoundManagerOneShot.Instance.SpiritSummon ();
	}

	public void ShowTimer(double summonTime)
	{
		Hide (Desc, false);
		Show (Timer, false);
		Timer.SetActive (true);
		Timer.GetComponent<Text> ().text = "Summons in : " + GetTimeRemaining (summonTime);
	}

	public static string GetTimeRemaining(double javaTimeStamp)
	{
		if (javaTimeStamp < 159348924)
		{
			string s = "unknown";
			return s;
		}
		System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
		var timeSpan = dtDateTime.Subtract(DateTime.UtcNow);
		string stamp = "";
		if (timeSpan.TotalDays < -1)
		{
			stamp = (Mathf.Abs((int)timeSpan.TotalDays)).ToString() + " days";
		}
		else
		{
			if (timeSpan.TotalHours < -1)
			{
				stamp = (Mathf.Abs((int)timeSpan.TotalHours)).ToString() + " hours";
			}
			else
			{
				if (timeSpan.TotalMinutes < -1)
				{
					stamp = (Mathf.Abs((int)timeSpan.TotalMinutes)).ToString() + " mins";
				}
				else
				{
					stamp = "few seconds";
				}
			}
		}
		return stamp;
	}

}

