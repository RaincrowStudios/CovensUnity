using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritMovementFX : MonoBehaviour { 

	public float moveSpeed = 2;
	public float attackMoveSpeed = 2;
	public float ScaleDownSpeed = 4;

	public static SpiritMovementFX Instance { get; set;}

	public GameObject whiteSpiritDie;
	public GameObject greySpiritDie;
	public GameObject shadowSpiritDie;

	public GameObject trailWhite;
	public GameObject trailGrey;
	public GameObject trailShadow;


	public GameObject instanceAttackWhite;
	public GameObject instanceAttackGrey;
	public GameObject instanceAttackShadow;


	public GameObject attackTrailWhite;
	public GameObject attackTrailGrey;
	public GameObject attackTrailShadow;

	void Awake()
	{
		Instance = this;
	}

	public void MoveSpirit(Token data)
	{
		if (MarkerManager.Markers.ContainsKey (data.instance)) {
			var a = MarkerManager.Markers [data.instance] [0].customData as Token;
			if (MapsAPI.Instance.zoom > 12) {
					StartCoroutine( SmoothScaleDown( MarkerManager.Markers [data.instance] [0],data));
			} else {
				foreach (var item in MarkerManager.Markers[data.instance]) {
					item.position = new Vector2 (data.longitude, data.longitude);
				}
			}
		} else {
			MarkerSpawner.Instance.AddMarker (data);
		}
	}

	IEnumerator SmoothScaleDown( IMarker marker, Token MD)
	{
		float scale = marker.scale;
		var data = marker.customData as Token; 
		print ("Lesser Spirit!");

		float t = 0;
	

		while (t <= 1f) {
			t += Time.deltaTime * ScaleDownSpeed;
			marker.scale = Mathf.SmoothStep (scale, 0, t);
			yield return null; 
		} 

		if(data.degree == 0){
			var death = Utilities.InstantiateObject (greySpiritDie, marker.instance.transform);
			death.transform.GetChild (1).gameObject.SetActive (false);
		} else if(data.degree == 1){
			var death = Utilities.InstantiateObject (whiteSpiritDie, marker.instance.transform);
			death.transform.GetChild (1).gameObject.SetActive (false);
		}else {
			var death = Utilities.InstantiateObject (shadowSpiritDie, marker.instance.transform);
			death.transform.GetChild (1).gameObject.SetActive (false);
		}
		yield return new WaitForSeconds (1.3f);
		MarkerManager.DeleteMarker (data.instance);
		MarkerSpawner.Instance.AddMarker (MD);
	}

	public void SpiritRemove(string instance)
	{
		if (MapsAPI.Instance.zoom > 12) {
			if (MarkerManager.Markers.ContainsKey (instance)) {
				
				StartCoroutine (DeathAnimation (MarkerManager.Markers [instance] [0]));
			}
		} else {
			MarkerManager.DeleteMarker (instance);
		}
	}

	IEnumerator DeathAnimation( IMarker marker)
	{
		float scale = marker.scale;
		var data = marker.customData as Token; 
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * ScaleDownSpeed;
			marker.scale = Mathf.SmoothStep (scale, 0, t);
			yield return null; 
		} 
		if(data.degree == 0){
			var death = Utilities.InstantiateObject (greySpiritDie, marker.instance.transform);
		} else if(data.degree == 1){
			var death = Utilities.InstantiateObject (whiteSpiritDie, marker.instance.transform);
		}else {
			var death = Utilities.InstantiateObject (shadowSpiritDie, marker.instance.transform);
		}

		yield return new WaitForSeconds (1.3f);
		MarkerManager.DeleteMarker (data.instance);
	}

	public void SpiritAttack(string instance, string target, bool isDead)
	{
		if (MapsAPI.Instance.zoom > 12) {
			if (MarkerManager.Markers.ContainsKey (instance) && MarkerManager.Markers.ContainsKey(target)) {
				StartCoroutine (Attack (MarkerManager.Markers [instance] [0],MarkerManager.Markers [target] [0],isDead,instance));
			}
		} 
	}

	IEnumerator Attack(IMarker start, IMarker end, bool isDead, string instance)
	{
		
		float t = 0;
		GameObject trail = null;
		GameObject attackFX = null;
		var data = start.customData as Token; 
		var data1 = end.customData as Token; 

		if (attackFX == null) {
			if (data.degree == 0) {
				attackFX = Utilities.InstantiateObject(instanceAttackGrey,start.instance.transform);
			} else if (data.degree == 1) {
				attackFX = Utilities.InstantiateObject(instanceAttackWhite,start.instance.transform);
			} else {
				attackFX = Utilities.InstantiateObject(instanceAttackShadow,start.instance.transform);
			}
		}

		if (trail == null) {
			if (data.degree == 0) {
				trail = Instantiate (attackTrailGrey) as GameObject;
			} else if (data.degree == 1) {
				trail = Instantiate (attackTrailWhite) as GameObject;
			} else {
				trail = Instantiate (attackTrailShadow) as GameObject;
			}

			trail.transform.localScale = Vector3.one * 4;
		}
		while (t <= 1f) {
			try{
			if (trail.transform != null) {
				t += Time.deltaTime * attackMoveSpeed;
				trail.transform.position = Vector3.Lerp (start.instance.transform.position, end.instance.transform.position, t);
				}}catch{
			}
			yield return null;
		}

		if(data1.degree == 0){
			var death = Utilities.InstantiateObject (greySpiritDie, end.instance.transform);
		} else if(data1.degree == 1){
			var death = Utilities.InstantiateObject (whiteSpiritDie, end.instance.transform);
		}else {
			var death = Utilities.InstantiateObject (shadowSpiritDie, end.instance.transform);
		}

		yield return new WaitForSeconds (1f);
		Destroy (trail);

		if (isDead) {
			MarkerManager.DeleteMarker (instance);

		}
		
	}
}
