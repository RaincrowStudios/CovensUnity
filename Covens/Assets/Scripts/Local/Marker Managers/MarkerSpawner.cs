using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkerSpawner : MarkerManager
{
	
	OnlineMapsControlBase3D Control;
	public static MarkerSpawner Instance { get; set;}

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


	public enum MarkerType
	{
		lesserPortal,greaterPortal,lesserSpirit,greaterSpirit,duke,place,witch,summoningEvent,gem,herb,tool,pet
	}

	void Awake()
	{
		Instance = this;
	}

	void Start ()
	{
		Control = OnlineMapsControlBase3D.instance;
	}

	public void CreateMarkers(List<MarkerData> Data)
	{
		DeleteAllMarkers ();
		foreach (var item in Data) {
			AddMarker (item);
		}
	}

	public void AddMarker(MarkerData Data)
	{
		List <OnlineMapsMarker3D> markers = new List<OnlineMapsMarker3D>();

		if (Data.token.Type == MarkerType.witch) {
			markers = CreateWitch (Data);
		} else if (Data.token.Type == MarkerType.duke || Data.token.Type == MarkerType.lesserSpirit || Data.token.Type == MarkerType.greaterSpirit) {
			markers =  CreateSpirit (Data);
		} else {
			markers = CreateOther (Data);
		}

		Data.token.Object = markers[0].instance;  
		markers[0].customData = Data; 
		markers[0].OnClick += onClickMarker;   

		if (Markers.ContainsKey (Data.instance)) {
			DeleteMarker (Data.instance); 
		} 
			Markers.Add (Data.instance, markers);
	}

	List<OnlineMapsMarker3D> CreateWitch(MarkerData data) 
	{
		var pos = new Vector2 (data.token.longitude, data.token.latitude);  
		OnlineMapsMarker3D marker;
		OnlineMapsMarker3D markerDot;
		if (data.token.male) {
			if (data.token.alignment > 0) {
				marker = SetupMarker(maleWhite,pos,witchScale,15);
			} else if (data.token.alignment < 0) { 
				marker = SetupMarker(maleShadow,pos,witchScale,15);
			} else {
				marker = SetupMarker(maleGrey,pos,witchScale,15);
			}
		}
		else {
			if (data.token.alignment > 0) {
				marker = SetupMarker(femaleWhite,pos,witchScale,15);
			} else if (data.token.alignment < 0) { 
				marker = SetupMarker(femaleShadow,pos,witchScale,15);
			} else {
				marker = SetupMarker(femaleGrey,pos,witchScale,15);
			}
		}

		markerDot = SetupMarker (witchDot, pos, witchDotScale, 3, 14);

		var mList = new List<OnlineMapsMarker3D> ();
		mList.Add (marker);
		mList.Add (markerDot);

		if (OnlineMaps.instance.zoom > 14) {
			markerDot.instance.gameObject.SetActive (false);
		} else
			marker.instance.gameObject.SetActive (false);

		return mList;
	}

	List<OnlineMapsMarker3D> CreateSpirit(MarkerData data) 
	{
		var pos = new Vector2 (data.token.longitude, data.token.latitude);  
		OnlineMapsMarker3D marker = new OnlineMapsMarker3D();
		OnlineMapsMarker3D markerDot = new OnlineMapsMarker3D();
		if (data.token.Type == MarkerType.lesserSpirit) {
			if (data.token.alignment == 1) {
				marker = SetupMarker (whiteLesserSpirit, pos, spiritLesserScale, 13);
			} else if (data.token.alignment == -1) {
				marker = SetupMarker (shadowLesserSpirit, pos, spiritLesserScale, 13);
			} else if (data.token.alignment == 0) {
				marker = SetupMarker (greyLesserSpirit, pos, spiritLesserScale, 13);
			}
		} else if (data.token.Type == MarkerType.greaterSpirit) {
			if (data.token.alignment == 1) {
				marker = SetupMarker (whiteGreaterSpirit, pos, spiritGreaterScale, 13);
			} else if (data.token.alignment == -1) {
				marker = SetupMarker (shadowGreaterSpirit, pos, spiritGreaterScale, 13);
			} else if (data.token.alignment == 0) {
				marker = SetupMarker (greyGreaterSpirit, pos, spiritGreaterScale, 13);
			} 
		} else if (data.token.Type == MarkerType.duke){
			if (data.token.alignment == 1) {
				marker = SetupMarker (dukeWhite, pos, DukeScale, 13);
			} else if (data.token.alignment == -1) {
				marker = SetupMarker (dukeShadow, pos, witchScale, 13);
			} else if (data.token.alignment == 0) {
				marker = SetupMarker (dukeGrey, pos, witchScale, 13);
			} 
		}

		markerDot = SetupMarker (spiritDot, pos, witchDotScale, 3, 12);

		var mList = new List<OnlineMapsMarker3D> ();
		mList.Add (marker);
		mList.Add (markerDot);

		if (OnlineMaps.instance.zoom > 12) {
			markerDot.instance.gameObject.SetActive (false);
		} else
			marker.instance.gameObject.SetActive (false);

		return mList;
	}

	List<OnlineMapsMarker3D> CreateOther(MarkerData data){
		var pos = new Vector2 (data.token.longitude, data.token.latitude);  
		OnlineMapsMarker3D marker = new OnlineMapsMarker3D();
		if (data.token.Type == MarkerType.lesserPortal) {
			if (data.token.alignment == 1) {
				marker = SetupMarker (whiteLesserPortal, pos, portalLesserScale, 13); 
			} else if (data.token.alignment == -1) { 
				marker = SetupMarker (shadowLesserPortal, pos, portalLesserScale, 13); 
			} else if (data.token.alignment == 0) { 
				marker = SetupMarker (greyLesserPortal, pos, portalLesserScale, 13); 
			}
		} 
		else if (data.token.Type == MarkerType.greaterPortal) {
			if (data.token.alignment == 1) {
				marker = SetupMarker (whiteGreaterPortal, pos, portalGreaterScale, 13); 
			} else if (data.token.alignment == -1) { 
				marker = SetupMarker (shadowGreaterPortal, pos, portalGreaterScale, 13); 
			}  else if (data.token.alignment == 0) { 
				marker = SetupMarker (greyGreaterPortal, pos, portalGreaterScale, 13); 
			}
		} 
		else if (data.token.Type == MarkerType.summoningEvent) {
			if (data.token.alignment == 1) {
				marker = SetupMarker (whiteGreaterPortal, pos, summonEventScale, 13); 
			} else if (data.token.alignment == -1) {
				marker = SetupMarker (shadowGreaterPortal, pos,summonEventScale , 13); 
			}
		} 
		else if (data.token.Type == MarkerType.herb) {
			marker = SetupMarker (herb, pos, botanicalScale , 13); 
		}  
		else if (data.token.Type == MarkerType.tool) {
			marker = SetupMarker (tool, pos, botanicalScale , 13); 
		}
		else if (data.token.Type == MarkerType.gem) {
				marker = SetupMarker (gem, pos, GemScale, 13);
		}
		else if(data.token.Type == MarkerType.place)
		{
			return null;
		}
		else if (data.token.Type == MarkerType.pet) {
			marker = SetupMarker (familiar, pos, familiarScale , 13); 
		}

		var mList = new List<OnlineMapsMarker3D> ();
		mList.Add (marker);
		return mList;
	}

	public void onClickMarker(OnlineMapsMarkerBase m)
	{
		var Data = m.customData as MarkerData;
//		GetMarkerDetailAPI.GetData(Data.instance,Data.token.Type); 
//		Data.token.latitude += Random.Range(-0.005f,0.006f);
//		Data.token.longitude += Random.Range(-0.005f,0.006f);
//		SpiritMovementFX.Instance.SpiritRemove (Data);
		MapZoomInManager.Instance.OnSelect(m.position);
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

