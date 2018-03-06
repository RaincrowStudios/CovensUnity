using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

public class OnPlayerSelect : MonoBehaviour {
	
	public static OnPlayerSelect Instance {get; set;}

	OnlineMapsTileSetControl mapControl;
	OnlineMapsControlBase3D control;
	OnlineMaps map;
	public CanvasGroup MainUICanvasGroup;

	public GameObject PlayerPrefab;

	Transform CamTransform;
	Camera Cam;
	Vector3 curPosition;
	Quaternion curRotation;
	MarkerSpawner MS;
	public Light spotLight;
	Vector3 spotLightPos;

	public float playerMarkerScale = 41;

	 OnlineMapsMarker3D SelectedPlayer;
	 Transform SelectedPlayerTransform;

	public CanvasGroup SpellCastCG;

	public float moveSpeed = 1;
	public GameObject yourWitch;
	Vector2 curPos;

	Vector3 camOnTapPos;
	Quaternion camOnTapRot;

	public Transform camZoomInTargetPos;

	public static bool isPlayer;

	CanvasGroup playerInfoCanvasGroup;

	public Button targetButton;
	public Text[] playerInfoUI;
	void Awake()
	{
		Instance = this;
	}

	void Start () {
		Cam = Camera.main;
		CamTransform = Cam.transform;
		control = OnlineMapsControlBase3D.instance;
		mapControl= OnlineMapsTileSetControl.instance;  
		map = OnlineMaps.instance;
		MS = MarkerSpawner.Instance;
		EventManager.OnPlayerDataReceived += UpdateEnergy;
	}

	public void OnClick(Vector2 focusPos, string displayName)
	{
		isPlayer = true;
		SpellCastCG.gameObject.SetActive (true);
		SpellSelectParent.Instance.SetupSpellCast ();
		curPosition = CamTransform.position; 
		curRotation = CamTransform.rotation; 
		curPos = OnlineMaps.instance.position;
		Utilities.allowMapControl (false);
		yourWitch.SetActive (true);
		SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, PlayerPrefab);
		SelectedPlayerTransform = SelectedPlayer.instance.transform;
		playerInfoUI = SelectedPlayerTransform.GetChild (0).GetChild (0).GetComponentsInChildren<Text> ();
		playerInfoCanvasGroup = SelectedPlayerTransform.GetChild (0).GetComponent<CanvasGroup> ();
		print (displayName);
		playerInfoUI [0].text = displayName;
		targetButton.onClick.AddListener (onSelectTargetPlayer);
		SelectedPlayer.scale = 0;
		spotLightPos = spotLight.transform.position;
		StartCoroutine (PersepectiveZoomIn (focusPos));
		Invoke("StartZoomDelayed",.5f);
	}
		
	public void GoBack()
	{
		MainUICanvasGroup.gameObject.SetActive (true);
		StartCoroutine (PersepectiveZoomOut (curPos));
	}

	IEnumerator PersepectiveZoomIn(Vector2 focusPos)
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * moveSpeed;
			map.position = Vector2.Lerp (curPos, focusPos, Mathf.SmoothStep (0, 1f, t));
			ZoomManage (t, focusPos);
			yield return null;
		}
		camOnTapPos = Cam.transform.position;
		camOnTapRot = Cam.transform.rotation;
		MainUICanvasGroup.gameObject.SetActive (false);
	}

	IEnumerator PersepectiveZoomOut(Vector2 focusPos)
	{
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime * moveSpeed;
			ZoomManage (t, focusPos);
			yourWitch.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 41,Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
		SelectedPlayer.control.RemoveMarker3D (SelectedPlayer);
		SelectedPlayer = null;
		yourWitch.SetActive (false);
		SpellCastCG.gameObject.SetActive (false);
		Utilities.allowMapControl (true);
	}

	void ZoomManage(float t, Vector2 focusPos)
	{
		foreach (var item in MarkerManager.Markers) {
			if (item.Value[0].inMapView) {
				var data = item.Value[0].customData as MarkerData;

				if (data.token.Type == MarkerSpawner.MarkerType.gem) {
					item.Value[0].scale = Mathf.SmoothStep (MS.GemScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.tool) {
					item.Value[0].scale = Mathf.SmoothStep (MS.botanicalScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.herb) {
					item.Value[0].scale = Mathf.SmoothStep (MS.botanicalScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.lesserPortal) {
					item.Value[0].scale = Mathf.SmoothStep (MS.portalLesserScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.lesserSpirit) {
					item.Value[0].scale= Mathf.SmoothStep (MS.spiritLesserScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.witch) {
					item.Value[0].scale = Mathf.SmoothStep (MS.witchScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.greaterPortal) {
					item.Value[0].scale = Mathf.SmoothStep (MS.portalGreaterScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.greaterSpirit) {
					item.Value[0].scale = Mathf.SmoothStep (MS.spiritGreaterScale, 0, t);
				}

				if (data.token.Type == MarkerSpawner.MarkerType.pet) {
					item.Value[0].scale = Mathf.SmoothStep (MS.familiarScale, 0, t);
				}
			}
		}
		PlayerManager.marker.scale = Mathf.SmoothStep (15, 0, t*2);
		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.scale = Mathf.SmoothStep (15, 0, t*2);
		SelectedPlayerTransform.localScale = Vector3.Lerp( Vector3.zero,Vector3.one*playerMarkerScale,  Mathf.SmoothStep (0, 1, t));
		SelectedPlayerTransform.localEulerAngles =   new Vector3 (0,  Mathf.SmoothStep (0, 271f, t),0);
		CamTransform.position =  Vector3.Lerp (curPosition, new Vector3 (54,246, 861), Mathf.SmoothStep (0, 1f, t));
		CamTransform.rotation =  Quaternion.Slerp (curRotation, Quaternion.Euler( new Vector3 (18.391f,-141, 1.62f)), Mathf.SmoothStep (0, 1f, t));
		Cam.fieldOfView= Mathf.SmoothStep (60f, 35f, t);
		MainUICanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
		SpellCastCG.alpha = t;
		RenderSettings.ambientLight = Color.Lerp (new Color (.875f, .875f, .875f), new Color (0,0,0), Mathf.SmoothStep (0, 1f, t));
		spotLight.intensity = Mathf.SmoothStep (0, 7f, t);
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
