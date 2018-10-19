using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
public class LocationUIManager : UIAnimationManager
{
	public static LocationUIManager Instance{ get; set;}
	public static string locationID { get; set;}
	public static bool isLocation = false;
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
	public Text timer;
	public Image timerProgress;
	int counter = 60;
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

	public List<SpriteRenderer> players = new List<SpriteRenderer> (); 
	public List<GameObject> spirits = new List<GameObject> (); 
	public Dictionary<string,Token> ActiveTokens = new Dictionary<string, Token>();
	public Sprite maleWhite; 
	public Sprite maleShadow;
	public Sprite maleGrey;
	public Sprite femaleWhite;
	public Sprite femaleShadow;
	public Sprite femaleGrey;

	public Image closeButton;
	public GameObject boundText;

	public CanvasGroup[] DisableInteraction;

	public Vector2 ini;
	Vector2 final;
	public bool isSummon = false;
	void Awake(){
		Instance = this;
	}

	void OnEnter()
	{
		this.StopAllCoroutines ();
		ActiveTokens.Clear ();
		players.Clear ();
		spirits.Clear ();
		foreach (var item in DisableInteraction) {
			item.blocksRaycasts = false;
		}
	}

	IEnumerator CountDown()
	{
		while (counter > 0) {
			counter--;
			timer.text = counter.ToString ();
			timerProgress.fillAmount = Mathf.Lerp (0, 1, Mathf.InverseLerp (0, PlayerDataManager. idleTimeOut, counter));
			yield return new WaitForSeconds (1);
		}
	}

	public void Bind(bool isBind)
	{
		if (isBind) {
			closeButton.color = new Color (0, 0, 0, 0);
			closeButton.GetComponent<Button> ().enabled = false;
			boundText.SetActive (true);
		} else {
			closeButton.color = Color.white;
			closeButton.GetComponent<Button> ().enabled = true;
			boundText.SetActive (false);
		}
	}

	public void AddToken(Token data)
	{
		data.position--;
		if (data.type == "witch") {
			SpriteRenderer sp = players [data.position];
			sp.gameObject.SetActive (true);
			sp.color = Color.white;
			sp.GetComponentInChildren<Text> ().text = data.displayName;
			data.Object = sp.gameObject;
			sp.gameObject.GetComponent<LocationTokenData> ().token = data;
			if (data.male) {
				if (data.degree > 0) {
					sp.sprite = maleWhite;
				} else if (data.degree < 0) { 
					sp.sprite = maleShadow;
				} else {
					sp.sprite = maleGrey;
				}
			} else {
				if (data.degree > 0) {
					sp.sprite = femaleWhite;
				} else if (data.degree < 0) { 
					sp.sprite = femaleShadow;
				} else {
					sp.sprite = femaleGrey;
				}
			}
		} else if(data.type == "spirit") {
			spirits [data.position].SetActive (true);
			data.Object = spirits [data.position];
			spirits [data.position].GetComponent<LocationTokenData> ().token = data;

		}

		ActiveTokens.Add (data.instance, data);
	}

	public void RemoveToken(string id)
	{
		if (ActiveTokens.ContainsKey (id)) {
			if (ActiveTokens [id].type == "witch") {
				players[ActiveTokens [id].position].gameObject.SetActive (false);
			} else {
				spirits [ActiveTokens [id].position].SetActive (false);
			}
		ActiveTokens.Remove (id);
		}
	}

	public void Escape(){
		print ("Escaping");
		locAnim.Play ("out");
		locRune.GetComponent<Animator> ().enabled = true;
		locRune.GetComponent<Animator> ().SetTrigger ("back");
		Destroy (locRune, 1f);
		StartCoroutine (MoveBack ());
	
		PlayerManager.marker.instance.SetActive(true);
		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.instance.SetActive(true);
		isSummon = false;
		APIManager.Instance.GetData ("/location/leave", ReceiveDataExit);
		foreach (var item in DisableInteraction) {
			item.blocksRaycasts = true;
		}
		Utilities.allowMapControl (true);
		print (PlayerDataManager.playerData.state);
		Invoke ("ShowDead", 1.4f);
		STM.enabled = false;
		isLocation = false;
	}

	void ShowDead()
	{
		if (PlayerDataManager.playerData.state == "dead") {
			DeathState.Instance.ShowDeath ();
		}
	}

	void OnEnterLocation(LocationData LD){
		Utilities.allowMapControl (false);
		OnEnter ();
		isLocation = true;
		StartCoroutine (CountDown ());
		counter = PlayerDataManager.idleTimeOut;
		OnlineMaps.instance.zoom = 16;
		PlayerManager.marker.instance.SetActive(false);
		title.text = MarkerSpawner.SelectedMarker.displayName;
		if (MarkerSpawner.SelectedMarker.controlledBy != "") {
			ownedBy.text = "Owned By : " + MarkerSpawner.SelectedMarker.controlledBy;
		}
		else {
			ownedBy.text = "Unclaimed";
		}
		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.instance.SetActive(false);

		locRune = Utilities.InstantiateObject (locationPrefab, MarkerSpawner.SelectedMarker3DT); 
		var lData = locRune.GetComponent<LocationRuneData> ();
		spirits = lData.spirits;
		players = lData.players;
		if (PlayerDataManager.playerData.covenName != "") {
			if (MarkerSpawner.SelectedMarker.isCoven) {
				if (PlayerDataManager.playerData.covenName == MarkerSpawner.SelectedMarker.controlledBy) {
					lData.DisableButton (true);
				} else {
					lData.DisableButton (false);
				}
			}
		} else {
			if (!MarkerSpawner.SelectedMarker.isCoven) {
				if (PlayerDataManager.playerData.displayName == MarkerSpawner.SelectedMarker.controlledBy) {
					lData.DisableButton (true);
				} else {
					lData.DisableButton (false);
				}
			}
		}
		if (MarkerSpawner.SelectedMarker.controlledBy == "" && spirits.Count==0) {
			print ("LocationEnabled!");
			lData.DisableButton (true);
		}
		Token t = new Token ();
		t.instance = PlayerDataManager.playerData.instance;
		t.male = PlayerDataManager.playerData.male;
		t.degree = PlayerDataManager.playerData.degree;
		t.position = LD.position;
		t.type = "witch";
		t.displayName = PlayerDataManager.playerData.displayName;
		AddToken (t);

		foreach (var item in LD.tokens) { 
			AddToken (item);
		}

		locRune.transform.localRotation = Quaternion.Euler (90, 0, 0); 
		locAnim.Play ("in"); 
		StartCoroutine (MoveMap ()); 
		Invoke ("DisableRuneAnimator", 1.3f);
	}

	void DisableRuneAnimator()
	{
		locRune.GetComponent<Animator> ().enabled = false;
	}

	public void CharacterLocationGained(string instanceID)
	{
		if (isLocation && instanceID == locationID) {
			if (PlayerDataManager.playerData.covenName != "") {
				ownedBy.text = "Owned By : " + PlayerDataManager.playerData.covenName;
			} else {
				ownedBy.text = "Owned By : " + PlayerDataManager.playerData.displayName;
			}
			locRune.GetComponent<LocationRuneData> ().DisableButton (true);
		}
	}

	public void CharacterLocationLost(string instanceID)
	{
		if (isLocation && instanceID == locationID) {
			ownedBy.text = "Unclaimed";
			locRune.GetComponent<LocationRuneData> ().DisableButton (true);
		}
	}

	public void LocationLost(WSData data){
		if (isLocation && data.location == locationID) {
			ownedBy.text = "Unclaimed";
			locRune.GetComponent<LocationRuneData> ().DisableButton (true);
		}
	}

	public void LocationGained(WSData data){
		print ("LocationGained");
		if (isLocation && data.location == locationID) {
			print ("Setting Up Gain");
			ownedBy.text = "Owned By : " + data.controlledBy;
			locRune.GetComponent<LocationRuneData> ().DisableButton (true);
		}
	}

	IEnumerator MoveMap ()
	{
		var OM = OnlineMaps.instance;
		float t = 0;
		ini = OnlineMaps.instance.position;
		final = MarkerSpawner.SelectedMarkerPos;
		final.x += 0.00043027191f;
		final.y += 0.00035482578f;
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

		if (SpiritSummonUI.activeInHierarchy && isSummon) {
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
				ShowIngredients (true);
				SummonButton.enabled = true;
				SummonButtonText.text = "Summon";
				SummonButtonText.color = Color.white;
//				IngredientUIManager.Instance.LocationSummoningSpirit (PlayerDataManager.SpiritToolsDict [id]);
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
		if (show)
			Show (spellContainer, false);
		else
			Hide (spellContainer);
	}

	public void OnSummon()
	{
		Show (SpiritSummonUI,true);
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
		cards.Clear ();
		isSummon = true;

		STM.enabled = true;
		var empty = Utilities. InstantiateObject (emptyCard, container.transform,.858f); 
		empty.name = "empty"; 
		var empty1 = Utilities.InstantiateObject (emptyCard, container.transform,.858f); 
		empty1.name = "empty"; 

		foreach (var item in PlayerDataManager.playerData.KnownSpiritsList) {
			if (DownloadedAssets.spiritArt.ContainsKey (item)) {
				var g = Utilities.InstantiateObject (spiritCard, container.transform, .858f);
				g.name = item; 
				g.GetComponent<LocationSpiritData> ().Setup (g.name);
				cards.Add (g.GetComponent<RectTransform> ());
			}
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
		isSummon = false;
		Hide (SpiritSummonUI,true);
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

	public void TryEnterLocation()
	{
		var k = new {location = MarkerSpawner.instanceID};
		APIManager.Instance.PostData ("/location/enter", JsonConvert.SerializeObject(k), ReceiveData);
	}

	public void ReceiveData(string response, int code)
	{
		if (code == 200) {
//			print ("EnteringLocation");
			OnEnterLocation (JsonConvert.DeserializeObject<LocationData>(response)); 
		} else {
			print (response);
		}
	}

	public void ReceiveDataExit(string response, int code)
	{
		if (code == 200) {
			print (response);
		} else {
			Debug.LogError ("Location Leaving Error : " + response);
		}
	}
}

public class LocationData{
	public int position{get;set;}
	public List<Token> tokens{ get; set;}
}

