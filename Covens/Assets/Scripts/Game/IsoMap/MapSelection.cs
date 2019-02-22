using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelection : MonoBehaviour {
	public static MapSelection Instance { get; set; }
	public static CurrentView currentView = CurrentView.MapView;
	public static Transform selectedItemTransform;
	public float speed = 1;
	public GameObject yourWitch;
	public Transform attackTransform;
	public GameObject mainUICanvas;
//	public CanvasGroup playerIconCanvas;
//	public Animator wardrobeAnimator;
	public Light spotlight;
	public GameObject[] portals;
	public GameObject spirit;
	public GameObject witchFemale;
	public GameObject witchMale;
	public GameObject witchBrigid;

	private CanvasGroup mainUICG;
	private Camera cam;
	public static IMarker marker;
	private Transform camTransform;
	private Vector3 camInitialPos;
	private Quaternion camInitialRot;
	private Vector2 curMapPos;
	private GameObject selectedTokenGO ;
	public static bool IsSelf = false;
	public static bool banishedCharacter = false;

	public ApparelView male;
	public ApparelView female;

	void Awake () {
		Instance = this;
	}

	void Start()
	{
		cam = Camera.main;
		camTransform = cam.transform;
		camInitialPos = camTransform.position;
		camInitialRot = camTransform.rotation;
		mainUICG = mainUICanvas.GetComponent<CanvasGroup> ();
	}

	GameObject Setup(MarkerDataDetail data)
	{
		GameObject g;
		if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.portal) {
			if (data.degree > 0) {
				selectedTokenGO = portals [0];
			} else if (data.degree == 0) {
				selectedTokenGO = portals [1];
			} else {
				selectedTokenGO = portals [2];
			}
		} else if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.spirit) {
			selectedTokenGO= spirit;

		} else if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.witch) {
			if (!FTFManager.isInFTF) {
		
				if (MarkerSpawner.SelectedMarker.equipped[0].id.Contains("_m_")) {
					selectedTokenGO = witchMale;
				} else {
					selectedTokenGO = witchFemale;
				}

			} else {
				selectedTokenGO = witchBrigid;
			}
		}
		selectedTokenGO.GetComponent<IsoTokenSetup> ().Type = MarkerSpawner.selectedType;
		selectedTokenGO.GetComponent<IsoTokenSetup> ().Setup ();
        selectedTokenGO.transform.position = new Vector3(
            selectedTokenGO.transform.position.x,
            selectedTokenGO.transform.position.y + 0.1f,
            selectedTokenGO.transform.position.z);
		return selectedTokenGO;
	}

	public void OnSelect( bool isSelf = false)
	{
		IsSelf = isSelf;
		yourWitch.SetActive (true);
        if (marker != null)
            MapsAPI.Instance.RemoveMarker(marker);
		curMapPos = MapsAPI.Instance.position;
		if (!isSelf) {
			Vector2 pos = MarkerSpawner.SelectedMarkerPos;
            marker = MapsAPI.Instance.AddMarker(pos, Setup(MarkerSpawner.SelectedMarker));
			selectedItemTransform = marker.instance.transform;
			StartCoroutine (ZoomIn (pos));
		} else {
			StartCoroutine (ZoomIn (MapsAPI.Instance.position));
		}
	
		if (PlayerDataManager.playerData.male) {
			female.gameObject.SetActive (false);
			male.gameObject.SetActive (true);
			male.InitializeChar (PlayerDataManager.playerData.equipped);
		} else {
			female.gameObject.SetActive (true);
			male.gameObject.SetActive (false);
			female.InitializeChar (PlayerDataManager.playerData.equipped);
		}

		currentView = CurrentView.TransitionView;
		if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.portal) 
		ConditionsManagerIso.Instance.SetupConditions ();
		EventManager.Instance.CallFreezeScale (false);
		Utilities.allowMapControl (false);
		UIStateManager.Instance.CallWindowChanged(false);
		foreach (var item in MarkerManager.Markers) {
			if (item.Value [0].inMapView) {
				item.Value [0].instance.SetActive (false);
			}
		}
	}

	IEnumerator ZoomIn(Vector2 pos)
	{
		float t = 0;
		SoundManagerOneShot.Instance.MenuSound ();
		while (t<=1) {
			t += Time.deltaTime * speed;
			MapsAPI.Instance.position = Vector2.Lerp (curMapPos,pos , Mathf.SmoothStep (0, 1f, t));
			ZoomManager (t);
			yield return null;
		}
		currentView = CurrentView.IsoView;
		mainUICanvas.SetActive (false);
		if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.portal)
			SpellManager.Instance.Initialize ();
		else
			IsoPortalUI.instance.EnablePortalCasting ();
	}

	IEnumerator ZoomOut()
	{
		float t = 1;
		SoundManagerOneShot.Instance.MenuSound ();
		while (t>=0) {
			t -= Time.deltaTime * speed;
			ZoomManager (t);
			yield return null;
		}
		currentView = CurrentView.MapView;
		MarkerSpawner.instanceID = "";
		EventManager.Instance.CallMapViewSet ();
        if (!IsSelf)
            MapsAPI.Instance.RemoveMarker(marker);
		marker = null;
		yourWitch.SetActive (false);
		Utilities.allowMapControl (true);
		if (PlayerDataManager.playerData.state == "dead" && !LocationUIManager.isLocation) {
			DeathState.Instance.ShowDeath ();
		}
		EventManager.Instance.CallFreezeScale (true);
		UIStateManager.Instance.CallWindowChanged(true);
		foreach (var item in MarkerManager.Markers) {
			if (item.Value [0].inMapView) {
				item.Value [0].instance.SetActive (true);
			}
		}
        //MarkerManagerAPI.GetMarkers (false);
        if (SpellManager.Instance.closeButton.activeInHierarchy)
        {
            print("Force Closing SpellBook");
            SpellManager.Instance.ForceCloseSpellBook();
        }
//		wardrobeAnimator.enabled = true;
	}

	public void GoBack()
	{
		currentView = CurrentView.TransitionView;
		mainUICanvas.SetActive (true);
		StartCoroutine (ZoomOut ());
	}

	void ZoomManager(float t)
	{
//			foreach (var item in MarkerManager.Markers) {
//				var data = item.Value[0].customData as Token;
//				item.Value[0].scale = Mathf.SmoothStep (data.scale, 0, t);
//			}
			
		//PlayerManager.marker.scale = Mathf.SmoothStep (24, 0, t*2);

		//if(PlayerManager.physicalMarker != null)
		//	PlayerManager.physicalMarker.scale = Mathf.SmoothStep (15, 0, t*2);
		//if (!IsSelf) {
		//	selectedItemTransform.localScale = Vector3.Lerp (Vector3.zero, Vector3.one * 41, Mathf.SmoothStep (0, 1, t));
		//	selectedItemTransform.localEulerAngles = new Vector3 (0, Mathf.SmoothStep (0, 243f, t), 0);
		//}
		//yourWitch.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 44,Mathf.SmoothStep (0, 1f, t)); 
		//camTransform.position =  Vector3.Lerp (camInitialPos, attackTransform.position, Mathf.SmoothStep (0, 1f, t));
		//camTransform.rotation =  Quaternion.Slerp (camInitialRot, attackTransform.rotation, Mathf.SmoothStep (0, 1f, t));
		//cam.fieldOfView= Mathf.SmoothStep (60, 35f, t);
		//mainUICG.alpha = Mathf.SmoothStep (1f, 0f, t);
//		playerIconCanvas.alpha = Mathf.SmoothStep (1f, 0f, t);
		//RenderSettings.ambientLight = Color.Lerp (Color.white, new Color (0,0,0), Mathf.SmoothStep (0, 1f, t));
		//spotlight.intensity = Mathf.SmoothStep (0, 2f, t);
	}

}
