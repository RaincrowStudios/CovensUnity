using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SignatureScrollManager : MonoBehaviour
{
	public RectTransform container;
	public GameObject signaturePrefab;
	public static Signature currentSignature = null;
	public static bool isActiveSig = false;
	CanvasGroup cg;
	public float SnapSpeed = 10;

	public float spacing = 50;
	List<Signature> signatures = new List<Signature>();
	 List<CanvasGroup> bttnCG = new List<CanvasGroup>();
	 List<RectTransform> bttn = new List<RectTransform>();
	public GameObject IngredientsInfo;
	bool canSelect = true;

	void Awake()
	{
		cg = GetComponent<CanvasGroup> ();
	}

	public void Initiate (List<Signature> data, int midPos) 
	{
		canSelect = true;
		foreach (RectTransform t in bttn) {
			Destroy (t.gameObject);
		}
		bttnCG.Clear ();
		bttn.Clear ();
		signatures = data;
		for (int i = 0; i < data.Count; i++) {
			var g = Utilities.InstantiateObject (signaturePrefab, container.transform);
			g.name = signatures [i].id;
//			print(data.i)
			g.GetComponent<Text> ().text = DownloadedAssets.spellDictData [data [i].id].spellName;
			var rt = g.GetComponent<RectTransform> ();
			if(i>0)
			rt.anchoredPosition = new Vector2(LayoutUtility.GetPreferredWidth(rt)*.5f+ spacing + LayoutUtility.GetPreferredWidth(bttn[i-1])*.5f + bttn[i-1].anchoredPosition.x,0);
			bttn.Add (rt);
			bttnCG.Add (g.GetComponent<CanvasGroup> ());
		}
		container.anchoredPosition = new Vector2 (-bttn [signatures.Count / 2].anchoredPosition.x, 0);
		bttnCG [signatures.Count / 2].alpha = 1;
		bttn [signatures.Count / 2].localScale = Vector3.one * 1.15f;

		StartCoroutine(LerpToBttn( bttn	[midPos].anchoredPosition.x,data[0].baseSpell));
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0) && canSelect) {
			PointerEventData ped = new PointerEventData (null);
			ped.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult> ();
			EventSystem.current.RaycastAll(ped, results);
			StopCoroutine ("LerpToBttn");
			foreach (var item in results) {
				if(item.gameObject.tag == "sig")
				StartCoroutine(LerpToBttn( item.gameObject.GetComponent<RectTransform> ().anchoredPosition.x,item.gameObject.name));
			}
		}
	}

	IEnumerator LerpToBttn(float position,string name)
	{
		canSelect = false;
		RectTransform selectedButton = null;
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * SnapSpeed;
			float newX = Mathf.Lerp (container.anchoredPosition.x, -position,t);
			Vector2 newPosition = new Vector2 (newX, container.anchoredPosition.y);
			container.anchoredPosition = newPosition;
			for (int i = 0; i < bttn.Count; i++) {
				if (name != bttn [i].name) {
					if (bttn [i].localScale.x != 1f) {
						bttn [i].localScale = Vector2.one * Mathf.SmoothStep (1.15f, 1, t*1.5f);
						bttnCG [i].alpha = Mathf.SmoothStep (1, .4f, t*1.5f);
					}
				} else {
					selectedButton = bttn [i];
					bttn [i].localScale = Vector2.one * Mathf.SmoothStep (1,1.15f, t*1.5f);
					bttnCG [i].alpha = Mathf.SmoothStep (.4f, 1f, t*1.5f);
				}
			}
			yield return null;
		}

		bool hasHerb = false;
		bool hasGem = false;
		bool hasTool = false;
		bool isBase = false;
		int missingHerb = 0;
		int missingGem = 0;
		int missingTool = 0;
		var ing = PlayerDataManager.playerData.ingredients; 
		if (SpellCarouselManager.currentSpellData.id != name) {
			foreach (var signatureItem in PlayerDataManager.playerData.signatures) {
				if (signatureItem.id == name) {
					currentSignature = signatureItem;
					isActiveSig = true;
					foreach (var ingredient in signatureItem.ingredients) {
						if (ingredient.type == "herb") {
							if (ing.herbsDict.ContainsKey (ingredient.id)) {
								if (ing.herbsDict [ingredient.id].count > ingredient.count) {
									hasHerb = true;
								} else {
									hasHerb = false;
									missingHerb = ingredient.count - ing.herbsDict [ingredient.id].count;
								}
							} else {
								hasHerb = false;
							}
						} else if (ingredient.type == "gem") {
							if (ing.gemsDict.ContainsKey (ingredient.id)) {
								if (ing.gemsDict [ingredient.id].count > ingredient.count) {
									hasGem = true;
								} else {
									hasGem = false;
									missingGem = ingredient.count - ing.gemsDict [ingredient.id].count;
								}
							} else {
								hasGem = false;
							}
						} else {
							if (ing.toolsDict.ContainsKey (ingredient.id)) {
								if (ing.toolsDict [ingredient.id].count > ingredient.count) {
									hasTool = true;
								} else {
									hasTool = false;
									missingTool = ingredient.count - ing.toolsDict [ingredient.id].count;
								}
							} else {
								hasTool = false;
							}
						}
					}
				}
			}
			if (!hasHerb || !hasGem || !hasTool) {
				SetButtonState (false); 
			} else {
				IngredientsInfo.SetActive (true);
				IngredientsInfo.GetComponentInChildren<Text> ().text = "Added Ingredients for " + DownloadedAssets.spellDictData [currentSignature.id].spellName ;
				SetButtonState (true);
			}
		} else {
			currentSignature = null;
			isActiveSig = false;
			CheckEnergy (name);
		}

			
		canSelect = true;
	}

	public void SetButtonState(bool state) 
	{
		if (state) {
			print (currentSignature.id);
			if (PlayerDataManager.playerData.cooldownDict.ContainsKey (currentSignature.id)) {
				SpellCarouselManager.Instance.SetupWarning (DownloadedAssets.spellDictData [currentSignature.id].spellName + " can be used again in 10 minutes.");
				SpellCastUIManager.Instance.SetTracing (false);
				return;
			}

			if (PlayerDataManager.playerData.energy < currentSignature.cost) {
				SpellCarouselManager.Instance.SetupWarning ("Insufficient Energy");
				SpellCastUIManager.Instance.SetTracing (false);
			} else {
				SpellCarouselManager.Instance.SetupWarning ("");
				SpellCastUIManager.Instance.SetTracing (true);
			
			}

		} else {
			SpellCarouselManager.Instance.SetupWarning ("Insufficient Ingredients");
			SpellCastUIManager.Instance.SetTracing (false);
		}
	}

	void CheckEnergy(string id)
	{
		print (id);
		if (PlayerDataManager.playerData.energy < PlayerDataManager.playerData.spellsDict[id].cost) {
			SpellCarouselManager.Instance.SetupWarning ("Insufficient Energy");
			SpellCastUIManager.Instance.SetTracing (false);
		} else {
			SpellCarouselManager.Instance.SetupWarning ("");
			SpellCastUIManager.Instance.SetTracing (true);
		}
	}

	public void Show()
	{
		StartCoroutine (FadeIn (2));
	}

	public void Hide()
	{
		StartCoroutine (FadeIn (2.5f));
	}

	public IEnumerator FadeIn(float speed=1)
	{
		float t = 0;
	
		while (t<=1) {
			t += Time.deltaTime * speed;
			cg.alpha = Mathf.SmoothStep (0, 1, t);
			transform.localScale = Vector3.one * Mathf.SmoothStep (0, 1, t);
			yield return null;
		}
	}

	public IEnumerator FadeOut(float speed=1)
	{
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * speed;
			cg.alpha = Mathf.SmoothStep (1, 0, t);
			transform.localScale = Vector3.one * Mathf.SmoothStep (1, 0, t);
			yield return null;
		}
		gameObject.SetActive (false);
	}
}

