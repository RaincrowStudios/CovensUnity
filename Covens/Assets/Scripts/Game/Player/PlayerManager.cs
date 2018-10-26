using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance { get; set;}

	public GameObject markerPrefab;
	public GameObject physicalMarkerPrefab;

	public Sprite maleBlack;
	public Sprite maleWhite;
	public Sprite maleAsian;

	public Sprite femaleWhite;
	public Sprite femaleAsian;
	public Sprite femaleBlack;




	public Image playerFlyIcon;

	public static OnlineMapsMarker3D marker;   				//actual marker
	public static OnlineMapsMarker3D physicalMarker;		// gyro marker

	public static bool inSpiritForm = false;
	public float playerScale = 15;
	public float playerPhysicalScale = 15;
	public bool fly = true;
	public GameObject transFormPrefab;
	public GameObject AttackRingPrefab;
	public static GameObject AttackRing;
	bool connectionFailed = false;	
	Vector2 currentPos;

	AudioSource AS;

	public AudioClip wings;
	public AudioClip spiritformSound;
	public AudioClip physicalformSound;

	public bool SnapMapToPosition = true;

	DateTime applicationBG ;

	public GameObject reinitObject;
	public Image spririt;
	public Text spiritName;
	public Text syncingServer;

	bool CheckFocus = false;

	void Awake()
	{
		AS = GetComponent<AudioSource> ();
		Instance = this;

	}

	void Start ()
	{
		StartCoroutine (CheckInternetConnection ());
	}

	IEnumerator TrackMap()
	{
		while (SnapMapToPosition) {
			OnlineMaps.instance.position = marker.position;
			yield return new WaitForSeconds (2);
		}

		while (true) {
			if (SnapMapToPosition) {
				yield return new WaitForSeconds (2.5f);
				OnlineMaps.instance.position = marker.position;
			}

			if (inSpiritForm) {
				physicalMarker.position = OnlineMapsLocationService.instance.position;
			} else {
				marker.position = OnlineMapsLocationService.instance.position;
			}

			yield return new WaitForSeconds (1);
		}
	}


	float deltaTime = 0.0f;
	public Color m_Color;

	public void initStart()
	{
		LoginAPIManager.GetCharacterReInit ();

		if (IsoPortalUI.isPortal)
			IsoPortalUI.instance.DisablePortalCasting ();
		if (SummonMapSelection.isSummon) {
			SummonUIManager.Instance.Close ();
		}
		if (SpellManager.isInSpellView) {
			SpellManager.Instance.Exit ();
		}
		reinitObject.SetActive (true);
		try{
		var k =  DownloadedAssets.spiritArt.ElementAt (UnityEngine. Random.Range( 0, DownloadedAssets.spiritArt.Count));
		spririt.sprite = k.Value;
		spiritName.text = DownloadedAssets.spiritDictData [k.Key].spiritName ; 
		}catch{
			
		}
		syncingServer.text = "Syncing with server . . .";

	}

	public void InitFinished()
	{
		reinitObject.SetActive (false);
	}

	void OnApplicationFocus(bool pause)
	{
		if (!pause) {
			applicationBG = DateTime.Now;
			CheckFocus = true;
		} else {
			
			if (CheckFocus) {
				TimeSpan ts = DateTime.Now.Subtract (applicationBG);
				if (ts.TotalSeconds > 200 && LoginAPIManager.loggedIn) {
					initStart ();
					CheckFocus = false;
				}
			}
		}
	}

	public void CreatePlayerStart()
	{
		SoundManagerOneShot.Instance.LandingSound ();
		if (marker != null) {
			OnlineMapsControlBase3D.instance.RemoveMarker3D (marker);
		}
		var	pos = PlayerDataManager.playerPos;
		SpawnPlayer (pos.x, pos.y); 
		OnlineMaps.instance.SetPositionAndZoom (pos.x, pos.y, 16);
//		MarkerManagerAPI.GetMarkers (true);
		StartCoroutine (TrackMap ());
		OnlineMaps.instance.OnChangePosition += onMapChangePos;
	}


	void onMapChangePos()
	{
		SnapMapToPosition = false;
		OnlineMaps.instance.OnChangePosition -= onMapChangePos;
	}

	public void ReSnapMap()
	{
		SnapMapToPosition = true;
		OnlineMaps.instance.OnChangePosition += onMapChangePos;
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
		var data = PlayerDataManager.playerData;
		var sp = marker.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
		if (data.male) {
			if (data.race.Contains ("A")) {
				sp.sprite = maleBlack;
			} else if (data.race.Contains ("O")) {  
				sp.sprite = maleAsian; 
			} else {
				sp.sprite = maleWhite;
			}
		} else {
			if (data.race.Contains ("A")) {
				sp.sprite = femaleBlack;
			} else if (data.race.Contains ("O")) {  
				sp.sprite = femaleAsian; 
			} else {
				sp.sprite = femaleWhite;
			}
		}
		playerFlyIcon.sprite = sp.sprite;
//		StartCoroutine()
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

	public void CancelFlight()
	{
		if (!fly) {
			OnlineMaps.instance.position = marker.position;
			Fly ();
		}
	}

	public void Fly()
	{
		if (!FirstTapVideoManager.Instance.CheckFlight ())
			return;
		
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
			SoundManagerOneShot.Instance.LandingSound ();
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

	IEnumerator CheckInternetConnection()
	{
		while (true) {

			if (Application.internetReachability == NetworkReachability.NotReachable) {
				reinitObject.SetActive (true);
				var k = DownloadedAssets.spiritArt.ElementAt (UnityEngine.Random.Range (0, DownloadedAssets.spiritArt.Count));
				spririt.sprite = k.Value;
				spiritName.text = DownloadedAssets.spiritDictData [k.Key].spiritName; 
				syncingServer.text = "Trying to connect . . .";
				connectionFailed = true;
			} else if (connectionFailed) {
				initStart ();
				connectionFailed = false;
			}
		
			yield return new WaitForSeconds (5);
		}

	}
}