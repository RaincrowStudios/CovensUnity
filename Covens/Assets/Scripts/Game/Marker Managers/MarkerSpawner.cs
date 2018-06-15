using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkerSpawner : MarkerManager
{
	
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
	public GameObject whitePOP; 
	public GameObject shadowPOP;
	public GameObject greyPOP;

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

	float scaleVal = 1;

	public enum MarkerType
	{
		lesserPortal,greaterPortal,lesserSpirit,greaterSpirit,duke,place,witch,summoningEvent,gem,herb,tool,pet,silver
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
		foreach (var item in Data) {
			AddMarker (item);
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
		} else if (Data.Type == MarkerType.duke || Data.Type == MarkerType.lesserSpirit || Data.Type == MarkerType.greaterSpirit) {
			markers =  CreateSpirit (Data);
		} else {
			markers = CreateOther (Data);
		}

		Data.Object = markers[0].instance;  
		markers[0].customData = Data; 
		markers[0].OnDoubleClick += onClickMarker;   

		if (Markers.ContainsKey (Data.instance)) {
			DeleteMarker (Data.instance); 
		} 
			Markers.Add (Data.instance, markers);
	}

	List<OnlineMapsMarker3D> CreateWitch(Token data) 
	{
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
		markerDot = SetupMarker (witchDot, pos, witchDotScale, 3, 14);
		marker.instance.GetComponent<MarkerScaleManager> ().iniScale = witchScale;
		marker.instance.GetComponent<MarkerScaleManager> ().m = marker;
		markerDot.instance.GetComponent<MarkerScaleManager> ().iniScale = witchDotScale;
		markerDot.instance.GetComponent<MarkerScaleManager> ().m = markerDot;
		var mList = new List<OnlineMapsMarker3D> ();
		mList.Add (marker);
		mList.Add (markerDot);

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
		if (data.Type == MarkerType.lesserSpirit) {
			if (data.degree == 1) {
				marker = SetupMarker (whiteLesserSpirit, pos, spiritLesserScale, 13);
			} else if (data.degree == -1) {
				marker = SetupMarker (shadowLesserSpirit, pos, spiritLesserScale, 13);
			} else if (data.degree == 0) {
				marker = SetupMarker (greyLesserSpirit, pos, spiritLesserScale, 13);
			}
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = spiritLesserScale;

		} else if (data.Type == MarkerType.greaterSpirit) {
			if (data.degree == 1) {
				marker = SetupMarker (whiteGreaterSpirit, pos, spiritGreaterScale, 13);
			} else if (data.degree == -1) {
				marker = SetupMarker (shadowGreaterSpirit, pos, spiritGreaterScale, 13);
			} else if (data.degree == 0) {
				marker = SetupMarker (greyGreaterSpirit, pos, spiritGreaterScale, 13);
			} 
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = spiritGreaterScale;

		} else if (data.Type == MarkerType.duke){
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
		if (data.Type == MarkerType.lesserPortal) {
			if (data.degree == 1) {
				marker = SetupMarker (whiteLesserPortal, pos, portalLesserScale, 13); 
			} else if (data.degree == -1) { 
				marker = SetupMarker (shadowLesserPortal, pos, portalLesserScale, 13); 
			} else if (data.degree == 0) { 
				marker = SetupMarker (greyLesserPortal, pos, portalLesserScale, 13); 
			}
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = portalLesserScale;

		} else if (data.Type == MarkerType.greaterPortal) {
			if (data.degree == 1) {
				marker = SetupMarker (whiteGreaterPortal, pos, portalGreaterScale, 13); 
			} else if (data.degree == -1) { 
				marker = SetupMarker (shadowGreaterPortal, pos, portalGreaterScale, 13); 
			} else if (data.degree == 0) { 
				marker = SetupMarker (greyGreaterPortal, pos, portalGreaterScale, 13); 
			}
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = portalGreaterScale;
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
		}else if (data.Type == MarkerType.gem) {
			marker = SetupMarker (gem, pos, GemScale, 13);
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = GemScale;
		} else if (data.Type == MarkerType.place) {
			return null;
		} else if (data.Type == MarkerType.pet) {
			marker = SetupMarker (familiar, pos, familiarScale, 13); 
			marker.instance.GetComponent<MarkerScaleManager> ().iniScale = familiarScale;
		} else if (data.Type == MarkerType.silver) {
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
		var Data = m.customData as Token;
		GetMarkerDetailAPI.GetData(Data.instance,Data.Type); 
//		Data.latitude += Random.Range(-0.005f,0.006f);
//		Data.longitude += Random.Range(-0.005f,0.006f);
//		SpiritMovementFX.Instance.SpiritRemove (Data);
//		MapZoomInManager.Instance.OnSelect(m.position);
		instanceID = Data.instance;
		SelectedMarkerPos = m.position;
		print (Data.Type);
		SelectedMarker3DT = Data.Object.transform;
		selectedType = Data.Type;
		if (Data.Type == MarkerType.witch) {
			OnPlayerSelect.Instance.OnClick (m.position);
		} else if (Data.Type == MarkerSpawner.MarkerType.gem || Data.Type == MarkerSpawner.MarkerType.herb || Data.Type == MarkerSpawner.MarkerType.tool) {
//			CollectibleSelect.instanceID = Data.instance;
			InventoryPickUpManager.Instance.PickUp (); 
		} else if (Data.Type == MarkerType.greaterPortal || Data.Type == MarkerType.lesserPortal) {
			PortalSelect.Instance.ShowLoading (Data.degree);
		} else if (Data.Type == MarkerType.lesserSpirit || Data.Type == MarkerType.greaterSpirit) {
			SpiritSelectManager.Instance.Select ();
		}
		else {
//			MapZoomInManager.Instance.OnSelect (m.position, false);
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

}

