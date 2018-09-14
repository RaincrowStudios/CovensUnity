using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance { get; set;}

	public GameObject markerPrefab;
	public GameObject physicalMarkerPrefab;

	public static OnlineMapsMarker3D marker;   				//actual marker
	public static OnlineMapsMarker3D physicalMarker;		// gyro marker

	public static bool inSpiritForm = false;
	public float playerScale = 15;
	public float playerPhysicalScale = 15;
	public bool fly = true;
	public GameObject transFormPrefab;
	public GameObject AttackRingPrefab;
	public static GameObject AttackRing;

	Vector2 currentPos;

	AudioSource AS;

	public AudioClip wings;
	public AudioClip spiritformSound;
	public AudioClip physicalformSound;

	void Awake()
	{
		AS = GetComponent<AudioSource> ();
		Instance = this;
	}


	void Update()
	{

	}

	public void CreatePlayerStart()
	{
		var pos = OnlineMapsLocationService.instance.position;
		PlayerDataManager.playerPos = pos;
		SpawnPlayer (pos.x, pos.y); 
		OnlineMaps.instance.SetPositionAndZoom (pos.x, pos.y, 16);
	}

	void SpawnPlayer (float x, float y)
	{
		Vector2 pos = new Vector2 (x, y);
		marker = OnlineMapsControlBase3D.instance.AddMarker3D (pos, markerPrefab);
		marker.scale = playerScale;
		marker.range = new OnlineMapsRange (3, 20);
		marker.instance.name = "_MyMarker";
		marker.instance.GetComponentInChildren<SpriteRenderer> ().sortingOrder = 4;
		var ms = marker.instance.GetComponent<MarkerScaleManager> ();
		ms.iniScale = marker.scale;
		ms.m = marker;
		AddAttackRing ();
	}
		
	void SpawnSpiritForm()
	{
		OnlineMapsControlBase3D.instance.RemoveMarker3D (marker);
		double x, y;
		OnlineMaps.instance.GetPosition (out x, out y);
		SpawnPlayer ((float)x, (float)y);
		PlayerDataManager.playerPos = new Vector2 ((float)x, (float)y);
	}
		

	void SpawnPhysicalPlayer ( )
	{
		
		#region compare coordinates
		double x1, y1, x2, y2;
		marker.GetPosition (out x1, out y1);
		var pos = OnlineMapsLocationService.instance.position;
		x2 = pos.x;
		y2 = pos.y;
		x2 = System.Math.Round (x2, 6);
		y2 = System.Math.Round (y2, 6);
		x1 = System.Math.Round (x1, 6);
		y1 = System.Math.Round (y1, 6);
		#endregion
	
		if (x2 != x1 && y2 != y1) {
			physicalMarker = OnlineMapsControlBase3D.instance.AddMarker3D (pos, physicalMarkerPrefab);
			physicalMarker.scale = playerPhysicalScale;
			physicalMarker.range = new OnlineMapsRange (3, 20);
			physicalMarker.instance.name = "_PhysicalMarker";
			physicalMarker.instance.GetComponentInChildren<SpriteRenderer> ().sortingOrder = 4;
			inSpiritForm = true;		
			var ms = physicalMarker.instance.GetComponent<MarkerScaleManager> ();
			ms.iniScale = physicalMarker.scale;
			ms.m = physicalMarker;
		} else {
			if (physicalMarker != null) {
				OnlineMapsControlBase3D.instance.RemoveMarker3D (PlayerManager.physicalMarker);
			}
		}
	}

	void fadePlayerMarker()
	{
		var g = Utilities.InstantiateObject (transFormPrefab, marker.transform);
		marker.instance.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 1, .25f);
	}

//	public void TransitionToBG(AudioMixerSnapshot AS)
//	{
//		AS.TransitionTo (1.5f);
//	}

	public void CancelFlight()
	{
		if (!fly) {
			OnlineMaps.instance.position = marker.position;
			Fly ();
		}
	}

	public void Fly()
	{
		if (fly) {
			FlySFX.Instance.fly ();
			if (!inSpiritForm) {
				AS.PlayOneShot (spiritformSound);
			}
			PlayerManagerUI.Instance.Flight ();
			fadePlayerMarker ();
			CenterMapOnPlayer ();
			RemoveAttackRing ();
			currentPos = OnlineMaps.instance.position;
	
		} else {
			FlySFX.Instance.EndFly ();
			PlayerManagerUI.Instance.Hunt ();
				SpawnSpiritForm ();
				if (!inSpiritForm) {
					SpawnPhysicalPlayer ();
				}
			if (OnlineMaps.instance.position != currentPos) {
				currentPos = OnlineMaps.instance.position;
				MarkerManagerAPI.GetMarkers (false);
			}
		}
		fly = !fly;
	}

	public void returnphysicalSound()
	{
		AS.PlayOneShot (physicalformSound);
	}

	public static void CenterMapOnPlayer()
	{
		double x, y;
		marker.GetPosition (out x, out y); 

		OnlineMaps.instance.SetPosition (x, y);
	}


	void AddAttackRing()
	{
		AttackRing = Utilities.InstantiateObject (AttackRingPrefab, marker.transform);
	}

	void RemoveAttackRing()
	{
		Destroy (AttackRing);
	}
}
