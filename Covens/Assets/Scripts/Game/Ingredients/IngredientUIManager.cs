using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class IngredientUIManager : UIAnimationManager
{
	public static IngredientUIManager Instance { get; set; }

	public Button toolButton;
	public Button herbButton;
	public Button gemButton;
	public CanvasGroup toolCG;
	public CanvasGroup herbCG;
	public CanvasGroup gemCG;
	public Text MissingWarning;
	public Text selectedItemTextHerb;
	public Text selectedItemTextTool;
	public Text selectedItemTextGem;

	public Text clearWarningText;
	public Text ClearButtonText;
	public Transform container;
	public GameObject ingredientButtonPrefab;
	public GameObject ListObject;
	public static	IngredientType curType = IngredientType.none;

	List<GameObject> allItems = new List<GameObject> ();
	public RectTransform[] shiftItems;

	public ColliderScrollTrigger CS;
	public ColliderScrollTrigger[] CSFade;
	public float pullSpeed = 1;
	public GameObject currentIngredient;
	SwipeDetector SD;
	public SpellTraceManager STM;
	public Transform Navigator;
	public GameObject blocker;
	public Text header;
	bool isOpen = false;
	void Awake ()
	{
		Instance = this;
		curType = IngredientType.none;
	}

	public void HandleSwipe()
	{
		if (!isOpen) {
			SwipeDown ();
		} else {
			SwipeUp ();
		}
	}

	 void SwipeDown()
	{
		curType = IngredientType.none;
		ListObject.SetActive (false);
		foreach (var item in shiftItems) {
			item.anchoredPosition = new Vector2 ( 0,item.anchoredPosition.y);
		}
		ClearButtonText.gameObject.SetActive (false);

		MissingWarning.gameObject.SetActive (false);
		print ("Swipe Down");
		StartCoroutine (MoveWindow (true));
		SoundManagerOneShot.Instance.PlayWhisper ();
		STM.enabled = false;
		if (SpellCastUIManager.isSignatureUnlocked && SignatureScrollManager.currentSignature != null) {
			MissingWarning.gameObject.SetActive (true);
			SignatureSelected ();
		}
		isOpen = true;
	}

	IEnumerator MoveWindow(bool isDown)
	{
		RectTransform rt = GetComponent<RectTransform> ();
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * pullSpeed;
			if (isDown) {
				rt.offsetMin = new Vector2 (rt.offsetMin.x, Mathf.SmoothStep (1440, 0, t));
				Navigator.localRotation = Quaternion.Euler (0, 0, Mathf.SmoothStep (0, 180,t));
			} else {
				rt.offsetMin = new Vector2 (rt.offsetMin.x, Mathf.SmoothStep (0, 1440, t));
				Navigator.localRotation = Quaternion.Euler (0, 0, Mathf.SmoothStep (180, 0,t));
			}
			yield return null;
		}
	}

	public void SwipeUp()
	{
		print ("Swipe Up");
//		SD.canSwipe = false;
		SoundManagerOneShot.Instance.PlayWhisper ();
		StartCoroutine (MoveWindow (false));
		STM.enabled = true;
		if (SpellCastUIManager.isSignatureUnlocked && SignatureScrollManager.currentSignature != null) {
			ClearSignature ();
		}
		isOpen = false;

	}

	IEnumerator ShiftItems( )
	{
		if (shiftItems [0].anchoredPosition.x == 300)
			yield break;
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * 3;
			foreach (var item in shiftItems) {
				item.anchoredPosition = new Vector2 (Mathf.SmoothStep (0, 300, t),item.anchoredPosition.y);

			}
			yield return null;
		}
		ListObject.SetActive (true);
	}

	public void OnChangeIngredientType(string kind)
	{
		if (kind != "none") {
			StartCoroutine (ShiftItems ());
		}

		if (kind == curType.ToString ()) {
			if (curType == IngredientType.none) {
				SetupButtons ();
				SetupCount ();
				clearWarningText.gameObject.SetActive (false);
				ClearButtonText.gameObject.SetActive (false);
				return;
			}
			OnClickAction ();
		} else {
			SoundManagerOneShot.Instance.PlayButtonTap ();
			clearWarningText.gameObject.SetActive (false);
			turnOffAddIcons ();
			if (kind == "gem") {
				curType = IngredientType.gem;
				DisplayList (curType);
				gemButton.transform.GetChild (0).gameObject.SetActive (true);
				if (IngredientsSpellManager.AddedGem.Key != null) {
					ClearButtonText.gameObject.SetActive (true);
					ClearButtonText.text = "Clear Gems";
				} else {
					ClearButtonText.gameObject.SetActive (false);
				}

			} else if (kind == "tool") {
				curType = IngredientType.tool;
				toolButton.transform.GetChild (0).gameObject.SetActive (true);
				DisplayList (curType);
				if (IngredientsSpellManager.AddedTool.Key != null) {
					ClearButtonText.gameObject.SetActive (true);
					ClearButtonText.text = "Clear Tools";
				}else {
					ClearButtonText.gameObject.SetActive (false);
				}
			} else if (kind == "herb") {
				curType = IngredientType.herb;
				DisplayList (curType);
				herbButton.transform.GetChild (0).gameObject.SetActive (true);
				if (IngredientsSpellManager.AddedHerb.Key != null) {
					ClearButtonText.gameObject.SetActive (true);
					ClearButtonText.text = "Clear Herbs";
				}else {
					ClearButtonText.gameObject.SetActive (false);
				}
			} else if (kind == "none") {
				curType = IngredientType.herb;
				DisplayList (curType);
				ClearButtonText.gameObject.SetActive (false);
			}
			SetupButtons ();
		}
	}

	public void turnOffAddIcons()
	{
		gemButton.transform.GetChild (0).gameObject.SetActive (false);
		herbButton.transform.GetChild (0).gameObject.SetActive (false);
		toolButton.transform.GetChild (0).gameObject.SetActive (false);
	}

	void SetupButtons()
	{
		if (curType == IngredientType.none) {
			toolCG.alpha = .5f;
			gemCG.alpha = .5f;
			herbCG.alpha =.5f;
		} else if (curType == IngredientType.herb) {
			toolCG.alpha = .5f;
			gemCG.alpha = .5f;
			herbCG.alpha =1;
		} else if (curType == IngredientType.tool) {
			toolCG.alpha = 1;
			gemCG.alpha = .5f;
			herbCG.alpha =.5f;
		} else {
			toolCG.alpha = .5f;
			gemCG.alpha =1 ;
			herbCG.alpha =.5f;
		}

	}

	void SetupCount ()
	{
		if (IngredientsSpellManager.AddedGem.Key != null&& IngredientsSpellManager.AddedGem.Value > 0) {
			if(gameObject.activeInHierarchy)
			StartCoroutine(FadeIn (selectedItemTextGem.gameObject,2));
			selectedItemTextGem.text = DownloadedAssets.ingredientDictData [IngredientsSpellManager.AddedGem.Key].name + " (" +  IngredientsSpellManager.AddedGem.Value.ToString () +")";
		}
		else {
			if(gameObject.activeInHierarchy)
			StartCoroutine(FadeOut (selectedItemTextGem.gameObject,2));
		}
		if (IngredientsSpellManager.AddedTool.Key != null && IngredientsSpellManager.AddedTool.Value > 0) {
			if(gameObject.activeInHierarchy)
			StartCoroutine(FadeIn (selectedItemTextTool.gameObject,2));
			selectedItemTextTool.text = DownloadedAssets.ingredientDictData [IngredientsSpellManager.AddedTool.Key].name + " (" +  IngredientsSpellManager.AddedTool.Value.ToString () +")";
		}
		else {
			if(gameObject.activeInHierarchy)
			StartCoroutine(FadeOut (selectedItemTextTool.gameObject,2));
		}
		if (IngredientsSpellManager.AddedHerb.Key != null&& IngredientsSpellManager.AddedHerb.Value > 0) {
			if(gameObject.activeInHierarchy)
			StartCoroutine(FadeIn (selectedItemTextHerb.gameObject,2));
			selectedItemTextHerb.text = DownloadedAssets.ingredientDictData [IngredientsSpellManager.AddedHerb.Key].name + " (" +  IngredientsSpellManager.AddedHerb.Value.ToString () +")";
		}
		else {
			if(gameObject.activeInHierarchy)
			StartCoroutine(FadeOut (selectedItemTextHerb.gameObject,2));
		}
	}

	void DisplayList (IngredientType type)
	{
		if (type == IngredientType.gem) {
			SpawnItems (PlayerDataManager.playerData.ingredients.gemsDict.Values.ToList ()); 
		} else if (type == IngredientType.tool) {
			SpawnItems (PlayerDataManager.playerData.ingredients.toolsDict.Values.ToList ()); 

		} else if (type == IngredientType.herb) {
			SpawnItems (PlayerDataManager.playerData.ingredients.herbsDict.Values.ToList ()); 
		} else {
			SpawnItems (null);
		}
	}

	void SpawnItems (List<InventoryItems> ing)
	{
		foreach (var item in allItems) {
			Destroy (item);
		}
		allItems.Clear ();
		if (ing == null)
			return;

		for (int i = 0; i < 4; i++) {
			var g = Utilities.InstantiateObject (ingredientButtonPrefab, container);
			var idb = g.GetComponent<IngredientButtonData> ();
			idb.ingType = IngredientType.none;
			idb.Setup ("", 0,"");
			allItems.Add (g);
		}

		foreach (var item in ing) { 
			var g = Utilities.InstantiateObject (ingredientButtonPrefab, container);
			var idb = g.GetComponent<IngredientButtonData> ();
			idb.ingType = curType;
			idb.Setup (DownloadedAssets.ingredientDictData [item.id].name, item.count,item.id);
			allItems.Add (g);
		}
		for (int i = 0; i < 4; i++) {
			var g = Utilities.InstantiateObject (ingredientButtonPrefab, container);
			var idb = g.GetComponent<IngredientButtonData> ();
			idb.ingType = IngredientType.none;
			idb.Setup ("", 0,"");
			allItems.Add (g);
		}
	}

	void OnClickAction ( )
	{
		if (currentIngredient == null) {
			return;
		}
		var idb = currentIngredient.GetComponent<IngredientButtonData>();
		if (idb.ingType == IngredientType.none)
			return;
		int i = IngredientsSpellManager.AddItem (idb.ID, curType);

		if (i == 0) {
			SoundManagerOneShot.Instance.PlayItemAdded ();
			if (idb.ingType == IngredientType.gem) {
				idb.Setup (PlayerDataManager.playerData.ingredients.gemsDict [idb.ID].name, PlayerDataManager.playerData.ingredients.gemsDict [idb.ID].count, idb.ID);
				ClearButtonText.text = "Clear Gems";
			} else if (idb.ingType == IngredientType.tool) {
				idb.Setup (PlayerDataManager.playerData.ingredients.toolsDict [idb.ID].name, PlayerDataManager.playerData.ingredients.toolsDict [idb.ID].count, idb.ID);
				ClearButtonText.text = "Clear Tools";
			} else {
				idb.Setup (PlayerDataManager.playerData.ingredients.herbsDict [idb.ID].name, PlayerDataManager.playerData.ingredients.herbsDict [idb.ID].count, idb.ID);
				ClearButtonText.text = "Clear Herbs";
			}
			ClearButtonText.gameObject.SetActive (true);
		} else {
			SoundManagerOneShot.Instance.PlayError();
		}
		if (i == 1) {
			idb.Setup (idb.ID, 0,idb.ID);
		} 
		if (i == 2) {
			clearWarningText.gameObject.SetActive (true);
			clearWarningText.text = "You can only use a maximum of 5 items.";
		}

		if (i == 3) {
			clearWarningText.gameObject.SetActive (true);
			clearWarningText.text = "Clear the current ingredient before adding a new one.";
		}
		SetupCount ();
	}

	public void ClearItem ()
	{
		IngredientsSpellManager.ClearCachedItems (curType);
		clearWarningText.gameObject.SetActive (false);
		ClearButtonText.gameObject.SetActive (false);
		DisplayList (curType);
		SetupCount ();
		SoundManagerOneShot.Instance.PlayButtonTap();
	}

	IEnumerator SmoothFade(Transform tr,float finalAlpha,float finalScale )
	{
		var image = tr.GetComponent<CanvasGroup> ();
		float curScale = tr.localScale.x;
		float alpha = image.alpha;
		float t = 0;
		if (alpha == finalAlpha) {
			yield break; 
		}
		while (t <= 1f) {
			t += Time.deltaTime * 4;
			if (tr == null)
				yield break;
			image.alpha = Mathf.SmoothStep (alpha, finalAlpha, t);
			float s = Mathf.SmoothStep (curScale, finalScale, t);
			tr.localScale = new Vector3(s,s,s);
			yield return null;
		}

	}

	void OnEnable()
	{
		CS.EnterAction = OnEnter;
		CS.ExitAction = OnExit;
		OnChangeIngredientType (curType.ToString ());
		GetComponent<RectTransform>().offsetMin = new Vector2 (GetComponent<RectTransform>().offsetMin.x, 1440);
		SetupButtons ();
		Navigator.localRotation = Quaternion.Euler (0, 0, 0);
	}

	public void OnEnter(Transform t)
	{
		currentIngredient = t.parent.gameObject;
		StartCoroutine(SmoothFade(t,1,1.3f));
	}

	public void OnExit(Transform t)
	{
		StartCoroutine(SmoothFade(t,.45f,1));
	}

//	public void SignatureSelected()
//	{
//		print ("Sig Selected");
//		foreach (var item in shiftItems) {
//			item.anchoredPosition = new Vector2 ( 0,item.anchoredPosition.y);
//
//		}
//		foreach (var item in allItems) {
//			Destroy (item);
//		}
//		selectedItemTextHerb.gameObject.SetActive (true);
//		selectedItemTextTool.gameObject.SetActive (true);
//		selectedItemTextGem.gameObject.SetActive (true);
//		selectedItemTextGem.transform.localScale = Vector3.one;
//		selectedItemTextHerb.transform.localScale = Vector3.one;
//		selectedItemTextTool.transform.localScale = Vector3.one;
//		selectedItemTextTool.GetComponent<CanvasGroup>().alpha = 1;
//		selectedItemTextGem.GetComponent<CanvasGroup>().alpha = 1;
//		selectedItemTextHerb.GetComponent<CanvasGroup>().alpha = 1;
//		toolCG.alpha = 1;
//		herbCG.alpha = 1;
//		gemCG.alpha = 1;
//		toolButton.transform.GetChild (0).gameObject.SetActive (false);
//		herbButton.transform.GetChild (0).gameObject.SetActive (false);
//		gemButton.transform.GetChild (0).gameObject.SetActive (false);
//			
//		var pIng = PlayerDataManager.playerData.ingredients; 
//		blocker.SetActive (true);
//		ClearButtonText.gameObject.SetActive (false);
//		header.text = "Ingredients required for " + DownloadedAssets.spellDictData [SignatureScrollManager.currentSignature.id].spellName;
//		foreach (var item in SignatureScrollManager.currentSignature.ingredients) {
//			if (item.type == "herb") {
//				if (!pIng.herbsDict.ContainsKey (item.id)) {
//					selectedItemTextHerb.text = "Missing " + item.count.ToString () + DownloadedAssets.ingredientDictData [item.id].name;
//					selectedItemTextHerb.color = Utilities.Red;
//				} else {
//					if (pIng.herbsDict [item.id].count < item.count) {
//						selectedItemTextHerb.text = "Missing " + (item.count - pIng.herbsDict [item.id].count).ToString () + DownloadedAssets.ingredientDictData [item.id].name;
//						selectedItemTextHerb.color = Utilities.Red;
//					} else {
//						selectedItemTextHerb.text = DownloadedAssets.ingredientDictData [item.id].name + " (" + item.count.ToString () + ")";
//						selectedItemTextHerb.color =Color.white;
//					}
//				}
//			} else if (item.type == "gem") {
//				if (!pIng.gemsDict.ContainsKey (item.id)) {
//					selectedItemTextGem.text = "Missing " + item.count.ToString () + DownloadedAssets.ingredientDictData [item.id].name;
//					selectedItemTextGem.color = Utilities.Red;
//				} else {
//					if (pIng.gemsDict [item.id].count < item.count) {
//						selectedItemTextGem.text = "Missing " + (item.count - pIng.gemsDict [item.id].count).ToString () + DownloadedAssets.ingredientDictData [item.id].name;
//						selectedItemTextGem.color = Utilities.Red;
//					} else {
//						selectedItemTextGem.text = DownloadedAssets.ingredientDictData [item.id].name + " (" + item.count.ToString () + ")";
//						selectedItemTextGem.color =Color.white;
//					}
//				}
//			} else {
//				if (!pIng.toolsDict.ContainsKey (item.id)) {
//					selectedItemTextTool.text = "Missing " + item.count.ToString () + DownloadedAssets.ingredientDictData [item.id].name;
//					selectedItemTextTool.color = Utilities.Red;
//				} else {
//					if (pIng.toolsDict [item.id].count < item.count) {
//						selectedItemTextTool.text = "Missing " + (item.count - pIng.toolsDict [item.id].count).ToString () + DownloadedAssets.ingredientDictData [item.id].name;
//						selectedItemTextTool.color = Utilities.Red;
//					} else {
//						selectedItemTextTool.text = DownloadedAssets.ingredientDictData [item.id].name + " (" + item.count.ToString () + ")";
//						selectedItemTextTool.color = Color.white;
//					}
//				}
//			}
//		}
//	}
//
//	public void ClearSignature()
//	{
//		curType = IngredientType.none;
//		blocker.SetActive (false);
//		header.text = "Empower your spells with ingredients.";
//		selectedItemTextTool.color = Color.white;
//		selectedItemTextGem.color =Color.white;
//		selectedItemTextHerb.color =Color.white;
//		selectedItemTextHerb.gameObject.SetActive (false);
//		selectedItemTextTool.gameObject.SetActive (false);
//		selectedItemTextGem.gameObject.SetActive (false);
//		selectedItemTextGem.transform.localScale = Vector3.zero;
//		selectedItemTextHerb.transform.localScale = Vector3.zero;
//		selectedItemTextTool.transform.localScale = Vector3.zero;
//		selectedItemTextTool.GetComponent<CanvasGroup>().alpha = 0;
//		selectedItemTextHerb.GetComponent<CanvasGroup>().alpha = 0;
//		selectedItemTextGem.GetComponent<CanvasGroup>().alpha = 0;
//		IngredientsSpellManager.ClearCachedItems (IngredientType.gem);
//		IngredientsSpellManager.ClearCachedItems (IngredientType.herb);
//		IngredientsSpellManager.ClearCachedItems (IngredientType.tool);
//		toolCG.alpha = .5f;
//		herbCG.alpha = .5f;
//		gemCG.alpha = .5f;
//	}
//

	public void LocationSummoningSpirit(string toolname){
		ClearAll ();
		IngredientsSpellManager.AddItem (toolname, IngredientType.tool,1);
		SetupCount ();
	}

	public void SignatureSelected()
	{
		string missingWarning = "Missing : ";
		bool isMissing = false;
		ClearAll ();
		header.text = "Ingredients required for " + DownloadedAssets.spellDictData [SignatureScrollManager.currentSignature.id].spellName;
		var playerIngredients = PlayerDataManager.playerData.ingredients; 
		foreach (var item in SignatureScrollManager.currentSignature.ingredients) {
			if (item.type == "herb") {
				if (!playerIngredients.herbsDict.ContainsKey (item.id)) {
					missingWarning +=  item.count.ToString () + " "+ DownloadedAssets.ingredientDictData [item.id].name + "  ";
					isMissing = true;
				} else {
					if (playerIngredients.herbsDict [item.id].count < item.count) {
						missingWarning +=  (item.count - playerIngredients.herbsDict [item.id].count).ToString () + " "+ DownloadedAssets.ingredientDictData [item.id].name + "  ";
						isMissing = true;
					} else {
						IngredientsSpellManager.AddItem (item.id, IngredientType.herb,item.count);
					}
				}
			} else if (item.type == "gem") {
				if (!playerIngredients.gemsDict.ContainsKey (item.id)) {
					missingWarning +=  item.count.ToString () + " "+ DownloadedAssets.ingredientDictData [item.id].name + "  ";
					isMissing = true;
				} else {
					if (playerIngredients.gemsDict [item.id].count < item.count) {
						missingWarning +=  (item.count - playerIngredients.herbsDict [item.id].count).ToString () + " "+ DownloadedAssets.ingredientDictData [item.id].name + "  ";
						isMissing = true;
					} else {
						IngredientsSpellManager.AddItem (item.id, IngredientType.gem,item.count);
					}
				}
			} else {
				if (!playerIngredients.toolsDict.ContainsKey (item.id)) {
					missingWarning +=  item.count.ToString () + " "+ DownloadedAssets.ingredientDictData [item.id].name + "  ";
					isMissing = true;
				} else {
					if (playerIngredients.toolsDict [item.id].count < item.count) {
						missingWarning +=  (item.count - playerIngredients.herbsDict [item.id].count).ToString () + " "+ DownloadedAssets.ingredientDictData [item.id].name + "  ";
						isMissing = true;
					} else {
						IngredientsSpellManager.AddItem (item.id, IngredientType.tool,item.count);
					}
				}
			}
		}
		if (isMissing) {
			MissingWarning.text = missingWarning;
		}else
			MissingWarning.text = "";

		SetupCount ();
		toolCG.alpha = 1;
		gemCG.alpha =1 ;
		herbCG.alpha =1;
		curType = IngredientType.none;
		turnOffAddIcons ();
	}
		
	public void ClearSignature()
	{
		header.text = "Empower your spells with ingredients.";
		curType = IngredientType.none;
	}

	void ClearAll()
	{
		IngredientsSpellManager.ClearCachedItems (IngredientType.gem);
		IngredientsSpellManager.ClearCachedItems (IngredientType.tool);
		IngredientsSpellManager.ClearCachedItems (IngredientType.herb);
	}
}


