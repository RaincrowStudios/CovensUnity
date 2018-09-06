using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LocationUIManager : UIAnimationManager
{
	public static LocationUIManager Instance{ get; set;}

	public GameObject locationPrefab;
	GameObject locRune;

	public List<RectTransform> cards = new List<RectTransform>();

	public GameObject spiritCard;
	public GameObject emptyCard;
	public RectTransform container;
	public GameObject SpiritSummonUI;
	public CanvasGroup[] CG;
	public CanvasGroup[] CGPartial;
	public SpellTraceManager STM;
	public Text requiredTool;
	public Text Desc;
	public Text SummonButtonText;
	public Button SummonButton;

	public Animator locAnim;
	public Text title;
	public Text ownedBy;

	float[] distances;
	float[] distanceReposition;
	public float snapSpeed = 1;
	public RectTransform center;
	int minButtonNum;
	bool dragging = false;

	int lastNum = 999;

	public GameObject[] EnabledObjects;
	public GameObject spellCanvas;
	public GameObject ingredient;
	public GameObject spellContainer;

	Vector2 ini;
	Vector2 final;
	bool isSummon = false;
	void Awake(){
		Instance = this;
	}

	void OnEnable()
	{
		
	}

	public void Escape(){
		locAnim.SetBool ("animate", false);  
		locRune.GetComponent<Animator> ().SetTrigger ("back");
//		Destroy (locRune, 2.5f);
		StartCoroutine (MoveBack ());
		PlayerManager.marker.instance.SetActive(true);
		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.instance.SetActive(true);

		isSummon = false;
	}

	public void OnEnterLocation(){
		OnlineMaps.instance.zoom = 16;
		PlayerManager.marker.instance.SetActive(false);
		title.text = MarkerSpawner.SelectedMarker.displayName;
		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.instance.SetActive(false);

		locRune = Utilities.InstantiateObject (locationPrefab, MarkerSpawner.SelectedMarker3DT); 
		locRune.transform.localRotation = Quaternion.Euler (90, 0, 0); 
		locAnim.SetBool ("animate", true); 
		StartCoroutine (MoveMap ()); 
	}

	IEnumerator MoveMap ()
	{
		var OM = OnlineMaps.instance;
		float t = 0;
		ini = OnlineMaps.instance.position;
		final = MarkerSpawner.SelectedMarkerPos;
		final.x += 0.00043027191f;
		final.y += 0.00055482578f;
		while (t <= 1) {
			t += Time.deltaTime * 2;
			OM.position = Vector2.Lerp (ini, final, t);


			foreach (var item in CG) {
				item.alpha = Mathf.SmoothStep (1, 0, t);
			}
			foreach (var item in CGPartial) {
				item.alpha = Mathf.SmoothStep (1, .3f, t);
			}
			yield return 0;
		}
	}

	IEnumerator MoveBack ()
	{
		var OM = OnlineMaps.instance;
		float t = 1;

		while (t >= 0) {
			t -= Time.deltaTime ;
			OM.position = Vector2.Lerp (ini, final, t*1.5f);

			foreach (var item in CG) {
				item.alpha = Mathf.SmoothStep (1, 0, t);
			}
			foreach (var item in CGPartial) {
				item.alpha = Mathf.SmoothStep (1, .3f, t);
			}
			yield return 0;
		}
	}

	void Update()
	{
		
		if (Input.GetMouseButtonDown (0) ) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider.gameObject.name == "button" && !isSummon) {
					OnSummon ();
				}
			} 
		}
		if (SpiritSummonUI.activeInHierarchy) {
			for (int i = 0; i < cards.Count; i++) {
				distanceReposition[i] = center.position.x - cards [i].position.x; 
				distances [i] = Mathf.Abs (distanceReposition [i]);
			}

			float minDistance = Mathf.Min (distances);
			for (int a = 0; a < cards.Count; a++) {
				if (minDistance == distances [a]) {
					minButtonNum = a;
				}
			}
			ShowInfoSpiritCard ();
		}
	}

	void ShowInfoSpiritCard()
	{
		if (minButtonNum != lastNum) {
			string id = cards [minButtonNum].name;
			requiredTool.text = DownloadedAssets.ingredientDictData[ PlayerDataManager.SpiritToolsDict [id]].name;
			Desc.text = DownloadedAssets.spiritDictData [id].spiritDescription;
			if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey (PlayerDataManager.SpiritToolsDict [id])) {
				SummonButton.enabled = true;
				SummonButtonText.text = "Summon";
				SummonButtonText.color = Color.white;
				IngredientUIManager.Instance.LocationSummoningSpirit (PlayerDataManager.SpiritToolsDict [id]);
				ShowIngredients (true);
			} else {
				SummonButton.enabled = false;
				SummonButtonText.text = "Missing " + requiredTool.text ;
				SummonButtonText.color = Utilities.Red;
				ShowIngredients (false);
			}
			lastNum = minButtonNum;
		}
	}

	public void CastSummon()
	{
		SpellCastAPI.CastSummoningLocation (); 
	}


	public void ShowIngredients(bool show)
	{
		foreach (var item in EnabledObjects) {
			item.SetActive (!show);
		}
		spellCanvas.SetActive (show);
		ingredient.SetActive (show);
		spellContainer.SetActive (show);
	}

	public void OnSummon()
	{
		Show (SpiritSummonUI,true);
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
		cards.Clear ();
		isSummon = true;

		ShowIngredients (true);
		STM.enabled = true;
		var empty = Utilities. InstantiateObject (emptyCard, container.transform,.858f); 
		empty.name = "empty"; 
		var empty1 = Utilities.InstantiateObject (emptyCard, container.transform,.858f); 
		empty1.name = "empty"; 

		foreach (var item in PlayerDataManager.playerData.KnownSpiritsList) {
			var g = Utilities.InstantiateObject (spiritCard, container.transform,.858f);
			g.name =item; 
			g.GetComponent<LocationSpiritData> ().Setup (g.name);
			cards.Add (g.GetComponent<RectTransform>());
		}

		var empty2 = Utilities.InstantiateObject (emptyCard, container.transform,.858f); 
		empty2.name = "empty"; 
		var empty3 =  Utilities.InstantiateObject (emptyCard, container.transform,.858f); 
		empty3.name = "empty"; 

		container.anchoredPosition = new Vector2( ((cards.Count + 4) * 650)+2, container.anchoredPosition .y); 

		distances = new float[cards.Count];
		distanceReposition = new float[cards.Count]; 
	
	}

	public void SummonClose()
	{
		Hide (SpiritSummonUI,true);
		isSummon = false;
		ShowIngredients (false);
		STM.enabled = false;

	}

	public void StartDragging()
	{
		dragging = true;
	}

	public void StopDragging()
	{
		dragging = false;
	}
}

