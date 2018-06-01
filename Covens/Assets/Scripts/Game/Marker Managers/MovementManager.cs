using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MovementManager : MonoBehaviour
{
	public static MovementManager Instance { get; set;} 
	List<string> tokensRemove = new List<string> (); 
	List<Token> tokensAdd = new List<Token> ();
	List<Token> CollectibleAdd = new List<Token> ();
	public float speed = 2;
	public GameObject[] attackerFX;
	public GameObject[] attackFX;
	public GameObject[] attackeeFX;
	public float AttackFXSpeed = 1;
	public GameObject collectibleFX;
	public GameObject result;
	public GameObject resultOther;

	public void AttackFXSelf(WebSocketResponse data)
	{
		if (OnlineMaps.instance.zoom < 12)
			return;
		if (MarkerManager.Markers.ContainsKey (data.instance)) {
			if (MarkerManager.Markers [data.instance] [0].inMapView) {
				if (data.degree > 0) {
					var g = Utilities.InstantiateObject (attackerFX [0], MarkerManager.Markers [data.instance] [0].transform,6);
					var g1 = Utilities.InstantiateObject(attackFX [0], MarkerManager.Markers [data.instance] [0].transform,2);
					g1.transform.parent = null;
					StartCoroutine (AttackTrail (g1.transform,PlayerManager.marker.transform,data.result.total,true));
				} else if (data.degree < 0) {
					var g = Utilities.InstantiateObject (attackerFX [1], MarkerManager.Markers [data.instance] [0].transform,6);
					var g1 = Utilities.InstantiateObject(attackFX [1], MarkerManager.Markers [data.instance] [0].transform,2);
					g1.transform.parent = null;
					StartCoroutine (AttackTrail (g1.transform,PlayerManager.marker.transform,data.result.total,true));
				} else {
					var g = Utilities.InstantiateObject (attackerFX [2], MarkerManager.Markers [data.instance] [0].transform,6);
					var g1 = Utilities.InstantiateObject(attackFX [2], MarkerManager.Markers [data.instance] [0].transform,2);
					g1.transform.parent = null;
					StartCoroutine (AttackTrail (g1.transform,PlayerManager.marker.transform,data.result.total,true));
				}
			}
		}
	}

	public void AttackFXOther(WebSocketResponse data)
	{
		if (OnlineMaps.instance.zoom < 12)
			return;
		if (MarkerManager.Markers.ContainsKey (data.instance) && MarkerManager.Markers.ContainsKey (data.target)) {
			OnlineMapsMarker3D caster = MarkerManager.Markers [data.instance] [0];  
			OnlineMapsMarker3D target = MarkerManager.Markers [data.target] [0]; 
			if (caster.inMapView && target.inMapView  ) { 
				var cData = caster.customData as Token;
				if (cData.degree > 0) {
					var g = Utilities.InstantiateObject (attackerFX [0], caster.transform,6);
					var g1 = Utilities.InstantiateObject(attackFX [0], caster.transform,1.4f);
					g1.transform.parent = null;
					StartCoroutine (AttackTrail (g1.transform,target.transform));
				} else if (cData.degree < 0) {
					var g = Utilities.InstantiateObject (attackerFX [1], caster.transform,6);
					var g1 = Utilities.InstantiateObject(attackFX [1], caster.transform,1.4f);
					g1.transform.parent = null;
					StartCoroutine (AttackTrail (g1.transform,target.transform));
				} else {
					var g = Utilities.InstantiateObject (attackerFX [2], caster.transform,6);
					var g1 = Utilities.InstantiateObject(attackFX [2], caster.transform,1.4f);
					g1.transform.parent = null;
					StartCoroutine (AttackTrail (g1.transform,target.transform));
				}
			}
		}
	}

	IEnumerator AttackTrail(Transform trail,Transform end,int dmg = 0, bool isSelf = false)
	{
		Vector3 startPos = trail.position;
		float t = 0;
		while (t<=1) {
			try{
			t += Time.deltaTime * AttackFXSpeed;
				trail.position = Vector3.Lerp (startPos, end.position,t);
			}catch{
				Destroy (trail.gameObject);
				break;
			}
			yield return null;
		}
		if (isSelf) {
			var g = Utilities.InstantiateObject (result, PlayerManager.marker.transform);
			g.GetComponentInChildren<Text> ().text = dmg.ToString ();
		} else {
			var g = Utilities.InstantiateObject (resultOther, end,1.5f);
		}
		Destroy (trail.gameObject,2f);
	}

	void Awake (){
		Instance = this;
	}

	void Start(){
		EventManager.OnMapViewSet += OnResumeMapView;
	}

	void OnDestroy(){
		EventManager.OnMapViewSet -= OnResumeMapView;
	}

	public void UpdateMarkerPosition(Token data){
		if (MarkerManager.Markers.ContainsKey (data.instance)) {
			var markers = MarkerManager.Markers [data.instance]; 
			markers [1].SetPosition (data.longitude, data.latitude);
			if (markers [0].inMapView) {
				StartCoroutine (MoveSpirit (markers [0].position, new Vector2 ((float)data.longitude, (float)data.latitude), markers [0]));
			} else {
				markers [0].SetPosition (data.longitude, data.latitude);
			}
		}
	}

	IEnumerator MoveSpirit(Vector2 ini,Vector2 final, OnlineMapsMarker3D marker)
	{
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime * speed;
			marker.position = Vector2.Lerp (ini, final, t);
			yield return null;
		}
	}

	public void AddMarkerInventory(Token data)
	{
		MarkerSpawner.Instance.AddMarker (data);
		print (data.type);
		print (data.Type);
		if(OnlineMaps.instance.zoom>13) {
		var g = Utilities.InstantiateObject (collectibleFX, MarkerManager.Markers [data.instance] [0].transform);
		}
	}

	public void RemoveMarker(string instanceID){
		if (MarkerManager.Markers.ContainsKey (instanceID)) {
			MarkerManager.DeleteMarker (instanceID);
		}
	}

	public void OnDeathMarker(Token data){
		if (MarkerManager.Markers.ContainsKey (data.instance)) {
			RemoveMarker (data.instance);
		}
	}

	public void AddMarker(Token Data){
		MarkerSpawner.Instance.AddMarker (Data);
	}

	public void AddMarkerInventoryIso(Token data)
	{
		CollectibleAdd.Add (data);
	}

	public void AddMarkerIso(Token Data){
		tokensAdd.Add (Data);
	}

	public void RemoveMarkerIso(string instanceID){
		tokensRemove.Add (instanceID);
	}

	public void UpdateMarkerPositionIso(Token data){
		tokensAdd.Add (data);
	}

	public void OnDeathMarkerIso(Token Data){
		RemoveMarkerIso (Data.instance);
	}

	void OnResumeMapView(){
		print ("Updating Map tokens");
		foreach (var item in tokensAdd) {
			UpdateMarkerPosition (item);
		}
		foreach (var item in CollectibleAdd) {
			AddMarkerInventory (item);
		}
		CollectibleAdd.Clear ();
		tokensAdd.Clear ();
		foreach (var item in tokensRemove) {
			RemoveMarker (item);
		}
		tokensRemove.Clear ();
	}
}

