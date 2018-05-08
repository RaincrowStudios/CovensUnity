using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager : MonoBehaviour
{
	public static MovementManager Instance { get; set;} 
	List<string> tokensRemove = new List<string> (); 
	List<Token> tokensAdd = new List<Token> ();
	public float speed = 2;
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
		tokensAdd.Clear ();
		foreach (var item in tokensRemove) {
			RemoveMarker (item);
		}
		tokensRemove.Clear ();
	}
}

