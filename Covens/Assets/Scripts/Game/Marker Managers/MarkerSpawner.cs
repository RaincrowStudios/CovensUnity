using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MarkerSpawner : MarkerManager
{
	public static Dictionary<string,HashSet<string>> ImmunityMap = new Dictionary<string, HashSet<string>> ();
	OnlineMapsControlBase3D Control;
	public static MarkerSpawner Instance { get; set;}
	public static MarkerType selectedType;
	public static MarkerDataDetail SelectedMarker = null;
	public static Transform SelectedMarker3DT = null;
	public static Vector2 SelectedMarkerPos;
	public static string instanceID = "";
	[Header("Witch")]
	public GameObject maleWhite; 
	public GameObject maleShadow;
	public GameObject maleGrey;
	public GameObject femaleWhite;
	public GameObject femaleShadow;
	public GameObject femaleGrey;
	public GameObject witchDot;
	public GameObject physicalEnemy;
	public GameObject physicalFriend;
	public GameObject spiritForm;
	public GameObject spiritFormFriend;
	public GameObject spiritFormEnemy;
	[Header("Portals")]
	public GameObject whiteLesserPortal; 
	public GameObject shadowLesserPortal;
	public GameObject greyLesserPortal;
	public GameObject whiteGreaterPortal; 
	public GameObject shadowGreaterPortal;
	public GameObject greyGreaterPortal; 
	public GameObject summoningEventPortal;

	[Header("Spirits")]
	public GameObject whiteLesserSpirit; 
	public GameObject shadowLesserSpirit;
	public GameObject greyLesserSpirit;
	public GameObject whiteGreaterSpirit;  
	public GameObject shadowGreaterSpirit;
	public GameObject greyGreaterSpirit;
	public GameObject dukeWhite;
	public GameObject dukeShadow;
	public GameObject dukeGrey;
	public GameObject spiritDot;

	[Header("Place Of Power")]
	public GameObject level1Loc; 
	public GameObject level2Loc;
	public GameObject level3Loc;

	[Header("Collectibles")] 
	public GameObject herb;  
	public GameObject familiar;
	public GameObject tool;
	public GameObject gem; 
	public GameObject silver; 

	[Header("Marker Scales")]
	public float witchScale = 4;
	public float witchDotScale = 4;
	public float summonEventScale = 4;
	public float portalGreaterScale = 4;
	public float portalLesserScale = 4;
	public float spiritLesserScale = 4;
	public float spiritGreaterScale = 4;
	public float DukeScale = 4;
	public float spiritDotScale = 4;
	public float placeOfPowerScale = 4;
	public float botanicalScale = 4;
	public float familiarScale = 4;
	public float GemScale = 4;


	public GameObject loadingObjectPrefab;
	private GameObject loadingObject;

	float scaleVal = 1;

	public enum MarkerType
	{
		portal,spirit,duke,location,witch,summoningEvent,gem,herb,tool,silver 
	}

	void Awake()
	{
		Instance = this; 
	}

	void Start ()
	{
		Control = OnlineMapsControlBase3D.instance;
	}

	public void CreateMarkers(List<Token> Data)
	{
		DeleteAllMarkers ();
		StartCoroutine (CreateMarkersHelper (Data));
	}

	IEnumerator CreateMarkersHelper(List<Token> Data)
	{
		foreach (var item in Data) {
			AddMarker (item);
			yield return 0;
		}
	}

	void callzoom()
	{
		EventManager.Instance.CallSmoothZoom ();

	}

	public void AddMarker(Token Data)
	{
		List <OnlineMapsMarker3D> markers = new List<OnlineMapsMarker3D>();
		if (Data.Type == MarkerType.witch) {
			markers = CreateWitch (Data);
		} else if (Data.Type == MarkerType.duke || Data.Type == MarkerType.spirit) {
			markers =  CreateSpirit (Data);
		} else {
			markers = CreateOther (Data);
		}

		Data.Object = markers[0].instance;  
		Data.scale = markers [0].scale;
		markers[0].customData = Data; 
		markers[0].OnClick += onClickMarker;   

		if (Markers.ContainsKey (Data.instance)) {
			DeleteMarker (Data.instance); 
		} 
			Markers.Add (Data.instance, markers);
	}

	List<OnlineMapsMarker3D> CreateWitch(Token data) 
	{
		ImmunityMap [data.instance] = data.immunityList; 

		var pos = new Vector2 (data.longitude, data.latitude);  
		OnlineMapsMarker3D marker;
		OnlineMapsMarker3D markerDot;
		if (data.male) {
			if (data.degree > 0) {
				marker = SetupMarker(maleWhite,pos,witchScale,15);
			} else if (data.degree < 0) { 
				marker = SetupMarker(maleShadow,pos,witchScale,15);
			} else {
				marker = SetupMarker(maleGrey,pos,witchScale,15);
			}
		}
		else {
			if (data.degree > 0) {
				marker = SetupMarker(femaleWhite,pos,witchScale,15);
			} else if (data.degree < 0) { 
				marker = SetupMarker(femaleShadow,pos,witchScale,15);
			} else {
				marker = SetupMarker(femaleGrey,pos,witchScale,15);
			}
		}
		try{
			if (ImmunityMap[data.instance].Contains(PlayerDataManager.playerData.instance)) {
			marker.instance.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 1, .3f);
			}}catch(System.Exception e){
			Debug.LogError (e);
		}
		markerDot = SetupMarker (witchDot, pos, witchDotScale, 3, 14);
		marker.instance.GetComponent<MarkerScaleManager> ().iniScale = witchScale;
		marker.instance.GetComponent<MarkerScaleManager> ().m = marker;
		markerDot.instance.GetComponent<MarkerScaleManager> ().iniScale = witchDotScale;
		markerDot.instance.GetComponent<MarkerScaleManager> ().m = markerDot;
		marker.instance.GetComponentInChildren<UnityEngine.UI.Text> ().text = data.displayName;
		var mList = new List<OnlineMapsMarker3D> ();
		if (PlayerDataManager.playerData.coven != "") {
			if (data.coven == PlayerDataManager.playerData.coven) {
				marker.instance.transform.GetChild (0).GetChild (0).gameObject.SetActive (true);
			}
		}
		mList.Add (marker);
		mList.Add (markerDot);
		SetupStance (marker.instance.transform, data);
		if (OnlineMaps.instance.zoom > 14) {
			markerDot.instance.gameObject.SetActive (false);
		} else
			marker.instance.gameObject.SetActive (false);
		return mList;
	}

	List<OnlineMapsMarker3D> CreateSpirit(Token data) 
	{
		var pos = new Vector2 (data.longitude, data.latitude);  
		OnlineMapsMarker3D marker = new OnlineMapsMarker3D();
		OnlineMapsMarker3D markerDot = new OnlineMapsMarker3D();
		if (data.Type == MarkerType.spirit) {
			if (data.degree > 0) {
				marker = SetupMarker (whiteLesserSpirit, pos, spiritLesserScale, 13);
			} else if (data.degree < 0) {
				marker = SetupMarker (shadowLesserSpirit, pos, spiritLesserScale, 13);
			} else if (data.degree == 0) {
				marker = SetupMarker (greyLesserSpirit, pos, spiritLesserScale, 13);
			}
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = spiritLesserScale;

		}else if (data.Type == MarkerType.duke){
			if (data.degree == 1) {
				marker = SetupMarker (dukeWhite, pos, DukeScale, 13);
			} else if (data.degree == -1) {
				marker = SetupMarker (dukeShadow, pos, DukeScale, 13);
			} else if (data.degree == 0) {
				marker = SetupMarker (dukeGrey, pos, DukeScale, 13);
			} 
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = DukeScale;
		}

		markerDot = SetupMarker (spiritDot, pos, witchDotScale, 3, 12);

		markerDot.instance.GetComponent<MarkerScaleManager> ().iniScale = witchDotScale;
		marker.instance.GetComponent<MarkerScaleManager> ().m = marker;
		markerDot.instance.GetComponent<MarkerScaleManager> ().m = markerDot;
		var mList = new List<OnlineMapsMarker3D> ();
		mList.Add (marker);
		mList.Add (markerDot);

		if (OnlineMaps.instance.zoom > 12) {
			markerDot.instance.gameObject.SetActive (false);
		} else
			marker.instance.gameObject.SetActive (false);

		return mList;
	}

	List<OnlineMapsMarker3D> CreateOther(Token data){
		var pos = new Vector2 (data.longitude, data.latitude);  
		OnlineMapsMarker3D marker = new OnlineMapsMarker3D();
//		print ("Adding Portal!");
		if (data.Type == MarkerType.portal) {
			if (data.degree == 1) {
				marker = SetupMarker (whiteLesserPortal, pos, portalLesserScale, 13); 
			} else if (data.degree == -1) { 
				marker = SetupMarker (shadowLesserPortal, pos, portalLesserScale, 13); 
			} else if (data.degree == 0) { 
				marker = SetupMarker (greyLesserPortal, pos, portalLesserScale, 13); 
			}
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = portalLesserScale;
//			print ("Adding Portal done");

		} else if (data.Type == MarkerType.summoningEvent) {
			if (data.degree == 1) {
				marker = SetupMarker (whiteGreaterPortal, pos, summonEventScale, 13); 
			} else if (data.degree == -1) {
				marker = SetupMarker (shadowGreaterPortal, pos, summonEventScale, 13); 
			}
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = summonEventScale;
		} else if (data.Type == MarkerType.herb) {
			marker = SetupMarker (herb, pos, botanicalScale, 13); 
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = botanicalScale;
		} else if (data.Type == MarkerType.tool) {
			marker = SetupMarker (tool, pos, botanicalScale, 13); 
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = botanicalScale;
		} else if (data.Type == MarkerType.silver) {
			marker = SetupMarker (silver, pos, botanicalScale, 13); 
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = botanicalScale;
		} else if (data.Type == MarkerType.gem) {
			marker = SetupMarker (gem, pos, GemScale, 13);
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = GemScale;
		} else if (data.Type == MarkerType.location) {
			if (data.tier == 1) {
				marker = SetupMarker (level1Loc, pos, placeOfPowerScale, 13);
				marker.instance.GetComponent<MarkerScaleManager> ().iniScale = placeOfPowerScale;
			} else if (data.tier == 2) {
				marker = SetupMarker (level2Loc, pos, placeOfPowerScale, 13);
				marker.instance.GetComponent<MarkerScaleManager> ().iniScale = placeOfPowerScale;
			} else {
				marker = SetupMarker (level3Loc, pos, placeOfPowerScale, 13);
				marker.instance.GetComponent<MarkerScaleManager> ().iniScale = placeOfPowerScale;
			}

		}  else if (data.Type == MarkerType.silver) {
			marker = SetupMarker (tool, pos, botanicalScale, 13); 
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = botanicalScale;
		} 
		marker.instance.GetComponent<MarkerScaleManager> ().m = marker;

		var mList = new List<OnlineMapsMarker3D> ();
		mList.Add (marker);
		return mList;
	}

	public void onClickMarker(OnlineMapsMarkerBase m)
	{
		if (!PlayerManager.Instance.fly)
			return;
		var Data = m.customData as Token;
//		GetMarkerDetailAPI.GetData(Data.instance,Data.Type); 
		instanceID = Data.instance;
		SelectedMarkerPos = m.position;
		SelectedMarker3DT = Data.Object.transform;
		selectedType = Data.Type;
		TargetMarkerDetailData data = new TargetMarkerDetailData();
		data.target = instanceID;
		APIManager.Instance.PostData ("map/select",JsonConvert.SerializeObject(data), GetResponse);
		if (loadingObject != null)
			Destroy (loadingObject);
		if (selectedType == MarkerType.portal ) {
			loadingObject = Utilities.InstantiateObject (loadingObjectPrefab, MarkerSpawner.SelectedMarker3DT,.16f);
		}else if(selectedType == MarkerType.location){
			loadingObject = Utilities.InstantiateObject (loadingObjectPrefab, MarkerSpawner.SelectedMarker3DT,2f);
			}else{
				loadingObject = Utilities.InstantiateObject (loadingObjectPrefab, MarkerSpawner.SelectedMarker3DT,1f);
			}
	}

	public void GetResponse(string response, int code)
	{
		Destroy (loadingObject);
		print("Getting Data success " + response);
		print (code);
		if (code == 200) {
			var data = JsonConvert.DeserializeObject<MarkerDataDetail> (response);
			if (data.conditions != null) {
				foreach (var item in data.conditions) {
					item.spellID = DownloadedAssets.conditionsDictData [item.condition].spellID;
					data.conditionsDict [item.conditionInstance] = item; 
				}
			}
			SelectedMarker = data;
			if (selectedType == MarkerType.witch || selectedType == MarkerType.portal || selectedType == MarkerType.spirit || selectedType == MarkerType.location ) {
				print ("Showing Card : " + selectedType );
				ShowSelectionCard.Instance.ShowCard (selectedType);
			} else if(selectedType == MarkerType.tool || selectedType == MarkerType.gem || selectedType == MarkerType.herb) {
				InventoryPickUpManager.Instance.OnDataReceived (); 
			}
		}
	}

	OnlineMapsMarker3D SetupMarker( GameObject prefab, Vector2 pos , float scale, int rangeMin =3 , int rangeMax =20)
	{
		OnlineMapsMarker3D marker;
		marker = Control.AddMarker3D (pos, prefab);
		marker.scale = scale;
		marker.range = new OnlineMapsRange (rangeMin, rangeMax);
		return marker;
	}

	public void SetupStance(Transform witchMarker, Token data)
	{
		Dictionary<string,GameObject> names = new Dictionary<string,GameObject> (); 
		foreach (Transform item in witchMarker) { 
			names.Add (item.name,item.gameObject);
		}

		if (StanceDict.ContainsKey (data.instance)) {
			
			if (names.ContainsKey ("spirit"))
				Destroy (names ["spirit"]);
			
			if (data.physical) {
				if (StanceDict[data.instance]) {
					if (!names.ContainsKey ("enemyP")) {
						var g = Utilities.InstantiateObject (physicalEnemy, witchMarker);
						g.name = "enemyP";

						if (names.ContainsKey ("friendP"))
							Destroy (names ["friendP"]);
						if (names.ContainsKey ("enemyS"))
							Destroy (names ["enemyS"]);
						if (names.ContainsKey ("friendS"))
							Destroy (names ["friendS"]);
						
					}
				} else {
					if (!names.ContainsKey ("friendP")) {
						var g = Utilities.InstantiateObject (physicalFriend, witchMarker);
						g.name = "friendP";

						if (names.ContainsKey ("enemyP"))
							Destroy (names ["enemyP"]);
						if (names.ContainsKey ("enemyS"))
							Destroy (names ["enemyS"]);
						if (names.ContainsKey ("friendS"))
							Destroy (names ["friendS"]);
						
					}
				}
			} else {
				if (StanceDict[data.instance]) {
					if (!names.ContainsKey ("enemyS")) {
						var g = Utilities.InstantiateObject (spiritFormEnemy, witchMarker);
						g.name = "enemyS";

						if (names.ContainsKey ("enemyP"))
							Destroy (names ["enemyP"]);
						if (names.ContainsKey ("friendP"))
							Destroy (names ["friendP"]);
						if (names.ContainsKey ("friendS"))
							Destroy (names ["friendS"]);
						
					}
				} else {
					if (!names.ContainsKey ("friendS")) {
						var g = Utilities.InstantiateObject (spiritFormFriend, witchMarker);
						g.name = "friendS";

						if (names.ContainsKey ("enemyP"))
							Destroy (names ["enemyP"]);
						if (names.ContainsKey ("friendP"))
							Destroy (names ["friendP"]);
						if (names.ContainsKey ("enemyS"))
							Destroy (names ["enemyS"]);
						
					}
				}
			}
		} 

		if (!data.physical && !names.ContainsKey ("spirit")) {
			var g = Utilities.InstantiateObject (spiritForm, witchMarker);
			g.name = "spirit";
		}
	}
}

