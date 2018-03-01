using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance { get; set;}

	public GameObject markerPrefab;
	public GameObject physicalMarkerPrefab;

	public static OnlineMapsMarker3D marker;   				//actual marker
	public static OnlineMapsMarker3D physicalMarker;		// gyro marker

	public static bool inSpiritForm = false;
	public float playerScale = 15;
	public float playerPhysicalScale = 15;
	bool fly = true;
	public GameObject transFormPrefab;
	public GameObject AttackRingPrefab;
	public static GameObject AttackRing;

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
		if (fly && inSpiritForm) {
			if (Input.GetMouseButtonUp (0)) {
				var amp = Mathf.Abs (Input.GetAxis ("Mouse X") + Input.GetAxis ("Mouse Y"));
				AS.PlayOneShot (wings, Mathf.Clamp01 (amp));
			}
		}
	}

	public void CreatePlayerStart()
	{
		var pos = OnlineMapsLocationService.instance.position;
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
		}
	}

	void fadePlayerMarker()
	{
		var g = Utilities.InstantiateObject (transFormPrefab, marker.transform);
		marker.instance.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 1, .25f);
	}

	public void Fly()
	{
		if (fly) {
			if (!inSpiritForm) {
				AS.PlayOneShot (spiritformSound);
			}
			PlayerManagerUI.Instance.Flight ();
			fadePlayerMarker ();
			CenterMapOnPlayer ();
			RemoveAttackRing ();
	
		} else {
			PlayerManagerUI.Instance.Hunt ();
			SpawnSpiritForm ();
			if (!inSpiritForm) {
				SpawnPhysicalPlayer ();
			}
			MarkerManagerAPI.GetMarkers (false);
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
