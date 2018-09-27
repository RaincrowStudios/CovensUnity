using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance { get; set;}


	public Sprite maleWhite; 
	public Sprite maleShadow;
	public Sprite maleGrey;
	public Sprite femaleWhite;
	public Sprite femaleShadow;
	public Sprite femaleGrey;

	public Image playerFlyIcon;

	GameObject markerPrefab;
	public GameObject malePrefab;
	public GameObject femalePrefab;

	public GameObject physicalMarkerPrefab;

	public static OnlineMapsMarker3D marker;   				//actual marker
	public static OnlineMapsMarker3D physicalMarker;		// gyro marker
	
	public static bool inSpiritForm = false;
	public float playerScale = 15;
	public float playerPhysicalScale = 15;
	public bool fly = true;
	public GameObject transFormPrefab;
	public GameObject AttackRingPrefab;
	public float transitionSpeed =1 ;
//	public static GameObject AttackRing;
	public Transform isoPos;
	public Transform flightViewPos;
	Transform mainCam;
	public ApparelView markerApparelView;
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

	void Start()
	{
		mainCam = Camera.main.transform;
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
		if(PlayerDataManager.playerData.male)
			marker = OnlineMapsControlBase3D.instance.AddMarker3D (pos, malePrefab);
		else
			marker = OnlineMapsControlBase3D.instance.AddMarker3D (pos, femalePrefab);

		marker.scale = playerScale;
		marker.range = new OnlineMapsRange (3, 20);
		marker.instance.name = "_MyMarker";
		marker.instance.GetComponentInChildren<SpriteRenderer> ().sortingOrder = 4;
		var ms = marker.instance.GetComponent<MarkerScaleManager> ();
		ms.iniScale = playerScale;
		ms.m = marker;
		markerApparelView = marker.instance.GetComponent<PlayerMarkerData> ().aView;
		markerApparelView.InitializeChar (PlayerDataManager.playerData.equipped);
		StartCoroutine (ScalePlayer ());
//		AddAttackRing ();
		var sr = marker.transform.GetChild(1).GetComponent<SpriteRenderer>();
		var pData = PlayerDataManager.playerData;
		if (pData.male) {
			if (pData.degree > 0) {
				sr.sprite = maleWhite;
			} else if (pData.degree < 0) {
				sr.sprite = maleShadow;
			} else {
				sr.sprite = maleGrey;
			}
		} else {
			if (pData.degree > 0) {
				sr.sprite = femaleWhite;
			} else if (pData.degree < 0) {
				sr.sprite = femaleShadow;
			} else {
				sr.sprite = femaleGrey;
			}
		}
		playerFlyIcon.sprite = sr.sprite;
	}
		
	void SpawnSpiritForm()
	{
//		OnlineMapsControlBase3D.instance.RemoveMarker3D (marker);
		double x, y;
		OnlineMaps.instance.GetPosition (out x, out y);
		marker.SetPosition (x, y);
		StartCoroutine (ScalePlayer ());
//		SpawnPlayer ((float)x, (float)y);
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

//	void fadePlayerMarker()
//	{
//		var g = Utilities.InstantiateObject (transFormPrefab, marker.transform);
//		marker.instance.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 1, .25f);
//	}

//	public void TransitionToBG(AudioMixerSnapshot AS)
//	{
//		AS.TransitionTo (1.5f);
//	}

	public void CancelFlight()
	{
		if (!fly) {
			OnlineMaps.instance.position = new Vector2( marker.position.x,marker.position.y);
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
			MoveTopDown ();
			CenterMapOnPlayer ();
			currentPos = OnlineMaps.instance.position;
	
		} else {
			MoveISO ();
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

	public void MoveISO()
	{
		StartCoroutine (IsoHelper ());
	}

	public void MoveTopDown()
	{
		StartCoroutine (TopDownHelper ());
	}

	IEnumerator IsoHelper ()
	{
		if (Camera.main.fieldOfView == 32) {
			yield break;
		}
		Utilities.allowMapControl (false);
		StartCoroutine (ScalePlayer ());
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime*transitionSpeed;
			mainCam.position =  Vector3.Lerp (flightViewPos.position, isoPos.position, Mathf.SmoothStep (0, 1f, t));
			mainCam.rotation =  Quaternion.Slerp (flightViewPos.rotation, isoPos.rotation, Mathf.SmoothStep (0, 1f, t));
			Camera.main.fieldOfView = Mathf.SmoothStep (60, 32, t);
			yield return 0;
		}
		Utilities.allowMapControl (true);
	}

	IEnumerator TopDownHelper ()
	{
		if (Camera.main.fieldOfView == 60) {
			yield break;
		}
		Utilities.allowMapControl (false);
		StartCoroutine (ScaleDownPlayer ());
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime*transitionSpeed;
			mainCam.position =  Vector3.Lerp (flightViewPos.position, isoPos.position, Mathf.SmoothStep (0, 1f, t));
			mainCam.rotation =  Quaternion.Slerp (flightViewPos.rotation, isoPos.rotation, Mathf.SmoothStep (0, 1f, t));
			Camera.main.fieldOfView = Mathf.SmoothStep (60, 32, t);
			yield return 0;
		}
		Utilities.allowMapControl (true);

	}

	public void ScalePlayerUP(){
		StartCoroutine (ScalePlayer ());
	}

	public void ScalePlayerDown(){
		StartCoroutine (ScaleDownPlayer (false));
	}

	IEnumerator ScalePlayer ()
	{
		Transform tr = marker.transform.GetChild (0);
		Transform sp = marker.transform.GetChild (1);
		tr.localScale = Vector3.one;
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime*transitionSpeed;
			tr.localScale =  Vector3.one*Mathf.SmoothStep(0,1,t);
			sp.localScale =  Vector3.one*Mathf.SmoothStep(.7566f,0,t);
			tr.rotation = Quaternion.Euler (-90,0, Mathf.SmoothStep (0, 213, t)); 
			yield return null;
		}
	}

	IEnumerator ScaleDownPlayer (bool isFlight = true)
	{
		Transform sp = marker.transform.GetChild (1);
		Transform tr = marker.transform.GetChild (0);
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime*transitionSpeed;
			sp.localScale =  Vector3.one*Mathf.SmoothStep(.7566f,0,t);
			tr.localScale =  Vector3.one*Mathf.SmoothStep(0,1,t);
			tr.rotation = Quaternion.Euler(-90,0, Mathf.SmoothStep (0, 213, t));
			yield return 0;
		}
		if(isFlight)
		PlayerManagerUI.Instance.Flight ();
	}

}
