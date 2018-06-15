using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class OnPlayerSelect : MonoBehaviour {
	
	public static OnPlayerSelect Instance {get; set;}
	PostProcessingProfile prof;
	public float BlurDelay = 1;
	public float blurOutSpeed = 1;
	public float blurSpeed = 1;
	public static bool hasEscaped = false;
	OnlineMapsTileSetControl mapControl;
	OnlineMapsControlBase3D control;
	OnlineMaps map;
	public CanvasGroup MainUICanvasGroup;

	public GameObject PlayerPrefab;
	public GameObject SpiritPrefab;

	Transform CamTransform;
	 public Transform AttackTransform;
	Camera Cam;
	Camera CamIsoToken;
	Vector3 curPosition;
	Quaternion curRotation;
	MarkerSpawner MS;
	public Light spotLight;
	Vector3 spotLightPos;
	public float playerMarkerScale = 41;

	OnlineMapsMarker3D SelectedPlayer;
	public static Transform SelectedPlayerTransform;
	public static int playerEnergyTemp;
	public static int playerXPTemp;

	public CanvasGroup SpellCastCG;
	public float attackSpeed =1;
	public float moveSpeed = 1;
	public float AttackToMainSpeed = 1;
	public GameObject yourWitch;
	Vector2 curPos;

	Vector3 camOnTapPos;
	Quaternion camOnTapRot;


	public Transform camZoomInTargetPos;


	public Sprite[] characters2d;

	CanvasGroup playerInfoCanvasGroup;

	public Text[] playerInfoUI;

//	public GameObject[] explosions;

	public GameObject blessFX;
	public GameObject HexFX;
	public GameObject SilenceFX;
	public GameObject SunEaterFX;
	public GameObject WhiteFlameFX;

	public SpellTraceManager STM;

	public GameObject resultScreen;
	public Text matchPercent;
	public Text resultInfo;
	public Image matchProgress;

	public GameObject[] portalPrefab;

	public static CurrentView currentView = CurrentView.MapView;

	void Awake()
	{
		Instance = this;
	}

	void Start () {
		Cam = Camera.main;
		CamTransform = Cam.transform;
		CamIsoToken = CamTransform.GetChild (2).GetComponent<Camera> ();
		prof = Camera.main.GetComponent<PostProcessingBehaviour> ().profile;
		currentView = CurrentView.MapView;
		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;
		dof.aperture = .6f;
		desat.basic.saturation = 1;
		prof.depthOfField.settings = dof;
		prof.colorGrading.settings = desat;

		control = OnlineMapsControlBase3D.instance;
		mapControl= OnlineMapsTileSetControl.instance;  
		map = OnlineMaps.instance;
		MS = MarkerSpawner.Instance;
		EventManager.OnPlayerDataReceived += UpdateEnergy;
	}

	public void OnClick(Vector2 focusPos)
	{
		hasEscaped = false;
		playerEnergyTemp = PlayerDataManager.playerData.energy;
		playerXPTemp = PlayerDataManager.playerData.xp;
		currentView = CurrentView.TransitionView;
		SpellCastCG.gameObject.SetActive (true);
		curPosition = CamTransform.position; 
		curRotation = CamTransform.rotation; 
		curPos = OnlineMaps.instance.position;
		Utilities.allowMapControl (false);
		yourWitch.SetActive (true);
		if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.lesserSpirit || MarkerSpawner.selectedType == MarkerSpawner.MarkerType.greaterSpirit  ) {
			SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, SpiritPrefab);
		} else if(MarkerSpawner.selectedType == MarkerSpawner.MarkerType.lesserPortal || MarkerSpawner.selectedType == MarkerSpawner.MarkerType.greaterPortal){
			print ("degree " + MarkerSpawner.SelectedMarker.degree);
			if(MarkerSpawner.SelectedMarker.degree == 0)
				SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, portalPrefab[0]);
			else if(MarkerSpawner.SelectedMarker.degree > 0)
				SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, portalPrefab[1]);
			else
				SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, portalPrefab[2]);
		} else {
			SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, PlayerPrefab);
		}
		SelectedPlayerTransform = SelectedPlayer.instance.transform;
		SelectedPlayer.instance.layer = 16;
		if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.lesserSpirit || MarkerSpawner.selectedType == MarkerSpawner.MarkerType.greaterSpirit) {
			SelectedPlayerTransform.GetChild (3).GetComponent<SpriteRenderer> ().sprite = characters2d [5];
		} else if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.witch)
			SelectedPlayerTransform.GetChild (5).GetComponent<SpriteRenderer> ().sprite = characters2d [Random.Range (0, characters2d.Length)];
		else {
			
		}

		if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.lesserPortal && MarkerSpawner.selectedType != MarkerSpawner.MarkerType.greaterPortal) {
			playerInfoUI = SelectedPlayerTransform.GetChild (0).GetChild (0).GetComponentsInChildren<Text> ();
			playerInfoCanvasGroup = SelectedPlayerTransform.GetChild (0).GetComponent<CanvasGroup> ();
			playerInfoUI [0].text = MarkerSpawner.SelectedMarker.displayName;
			playerInfoUI [1].text = MarkerSpawner.SelectedMarker.energy.ToString ();
			AttackVisualFXManager.Instance.targetHealth = playerInfoUI [1];
		} else {
			playerInfoCanvasGroup = SelectedPlayerTransform.GetChild (0).GetComponent<CanvasGroup> ();
			print ("SETTING UP");
			SelectedPlayerTransform.GetComponent<PortalSelectInfo> ().Setup (MarkerSpawner.SelectedMarker);
		}

			

			SelectedPlayer.scale = 0;
			spotLightPos = spotLight.transform.position;
		StartCoroutine (PersepectiveZoomIn (focusPos));
		EventManager.Instance.CallCastingStateChange (SpellCastStates.selection);
		Invoke("StartZoomDelayed",.5f);
	}
		
	public void GoBack()
	{
		MarkerSpawner.instanceID = "";
		MainUICanvasGroup.gameObject.SetActive (true);
		StartCoroutine (PersepectiveZoomOut (curPos));
		STM.enabled = false;
	}

	IEnumerator PersepectiveZoomIn(Vector2 focusPos)
	{
		float t = 0;
		EventManager.Instance.CallFreezeScale (false);
	
		while (t <= 1f) {
			t += Time.deltaTime * moveSpeed;
			map.position = Vector2.Lerp (curPos, focusPos, Mathf.SmoothStep (0, 1f, t));
			ZoomManage (t, focusPos);
			yield return null;
		}
		currentView = CurrentView.IsoView;
		camOnTapPos = Cam.transform.position;
		camOnTapRot = Cam.transform.rotation;
		MainUICanvasGroup.gameObject.SetActive (false);
	}

	IEnumerator PersepectiveZoomOut(Vector2 focusPos)
	{
		EventManager.Instance.CallFreezeScale (true);
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime * moveSpeed;
			ZoomManage (t, focusPos);
			yourWitch.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 44,Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
		currentView = CurrentView.MapView;
		EventManager.Instance.CallMapViewSet ();
		SelectedPlayer.control.RemoveMarker3D (SelectedPlayer);
		SelectedPlayer = null;
		yourWitch.SetActive (false);
		SpellCastCG.gameObject.SetActive (false);
		Utilities.allowMapControl (true);
	}

	void ZoomManage(float t, Vector2 focusPos)
	{
		var desat = prof.colorGrading.settings;
		try{
		foreach (var item in MarkerManager.Markers) {
				var data = item.Value[0].customData as Token;

				if (data.Type == MarkerSpawner.MarkerType.gem) {
					item.Value[0].scale = Mathf.SmoothStep (MS.GemScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.tool) {
					item.Value[0].scale = Mathf.SmoothStep (MS.botanicalScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.herb) {
					item.Value[0].scale = Mathf.SmoothStep (MS.botanicalScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.lesserPortal) {
					item.Value[0].scale = Mathf.SmoothStep (MS.portalLesserScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.lesserSpirit) {
					item.Value[0].scale= Mathf.SmoothStep (MS.spiritLesserScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.witch) {
					item.Value[0].scale = Mathf.SmoothStep (MS.witchScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.greaterPortal) {
					item.Value[0].scale = Mathf.SmoothStep (MS.portalGreaterScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.greaterSpirit) {
					item.Value[0].scale = Mathf.SmoothStep (MS.spiritGreaterScale, 0, t);
				}

				if (data.Type == MarkerSpawner.MarkerType.pet) {
					item.Value[0].scale = Mathf.SmoothStep (MS.familiarScale, 0, t);
				}
		}
		}catch(System.Exception e){ 
			Debug.LogError (e);
		}
		PlayerManager.marker.scale = Mathf.SmoothStep (15, 0, t*2);

		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.scale = Mathf.SmoothStep (15, 0, t*2);
		if (!hasEscaped) {
			SelectedPlayerTransform.localScale = Vector3.Lerp (Vector3.zero, Vector3.one * playerMarkerScale, Mathf.SmoothStep (0, 1, t));
			SelectedPlayerTransform.localEulerAngles = new Vector3 (0, Mathf.SmoothStep (0, 243f, t), 0);
		}
		CamTransform.position =  Vector3.Lerp (curPosition, AttackTransform.position, Mathf.SmoothStep (0, 1f, t));
		CamTransform.rotation =  Quaternion.Slerp (curRotation, AttackTransform.rotation, Mathf.SmoothStep (0, 1f, t));
		Cam.fieldOfView= Mathf.SmoothStep (60f, 35f, t);
		CamIsoToken.fieldOfView= Mathf.SmoothStep (60f, 35f, t);
		MainUICanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
		SpellCastCG.alpha = t;
		RenderSettings.ambientLight = Color.Lerp (Color.white, new Color (0,0,0), Mathf.SmoothStep (0, 1f, t));
		spotLight.intensity = Mathf.SmoothStep (0, 2f, t);
		desat.basic.saturation = Mathf.SmoothStep (1, 0, t);
		prof.colorGrading.settings = desat;
	}

	void StartZoomDelayed()
	{
		StartCoroutine (zoomDelayed ());
	}

	IEnumerator zoomDelayed( )
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * moveSpeed*2.4f;
			yourWitch.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 41,Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
	}

	public void onSelectTargetPlayer ()
	{
		if (MarkerSpawner.SelectedMarker != null) {
			StartCoroutine (TargetZoomIn ());
			OnTargetSelectUI.Instance.Onclick (MarkerSpawner.SelectedMarker);
		} else {
			return;
		}
	}

	public void onDeselectTargetPlayer()
	{
		SpellCastCG.gameObject.SetActive (true);
		StartCoroutine (TargetZoomOut ());
	}

	public void UpdateEnergy()
	{
		playerInfoUI [1].text = MarkerSpawner.SelectedMarker.energy.ToString();
	}

	IEnumerator TargetZoomIn ()
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime;
			TargetZoomControl (t);
			yield return null;
		}
		SpellCastCG.gameObject.SetActive (false);
	}

	IEnumerator TargetZoomOut ()
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime;
			TargetZoomControl (t);
			yield return null;
		}
	}

	void TargetZoomControl (float t)
	{
		SpellCastCG.alpha = Mathf.SmoothStep (1f, 0f, t);
		playerInfoCanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
		Cam.transform.position = Vector3.Lerp (camOnTapPos, camZoomInTargetPos.position, Mathf.SmoothStep (0, 1f, t));
		CamTransform.rotation =  Quaternion.Slerp (camOnTapRot, camZoomInTargetPos.rotation, Mathf.SmoothStep (0, 1f, t));
		SelectedPlayerTransform.localEulerAngles = new Vector3 (0, Mathf.SmoothStep (271, 180, t),0);
		spotLight.transform.position = Vector3.Lerp (spotLightPos, new Vector3(-534,415.4f,444), Mathf.SmoothStep (0, 1f, t));
	
	}

}
