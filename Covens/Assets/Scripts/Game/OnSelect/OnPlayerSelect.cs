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

	public CanvasGroup SpellCastCG;
	public float attackSpeed =1;
	public float moveSpeed = 1;
	public float AttackToMainSpeed = 1;
	public GameObject yourWitch;
	Vector2 curPos;

	Vector3 camOnTapPos;
	Quaternion camOnTapRot;

	Animator anim;

	public Transform camZoomInTargetPos;

	public static bool isPlayer;

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

	void Awake()
	{
		Instance = this;
	}

	void Start () {
		Cam = Camera.main;
		CamTransform = Cam.transform;
		CamIsoToken = CamTransform.GetChild (2).GetComponent<Camera> ();
		prof = Camera.main.GetComponent<PostProcessingBehaviour> ().profile;

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
		if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.lesserSpirit) {
			SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, SpiritPrefab);
		} else {
			SelectedPlayer = OnlineMapsControlBase3D.instance.AddMarker3D (focusPos, PlayerPrefab);
		}
		SelectedPlayerTransform = SelectedPlayer.instance.transform;
		SelectedPlayer.instance.layer = 16;
		if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.lesserSpirit) {
			SelectedPlayerTransform.GetChild (5).GetComponent<SpriteRenderer> ().sprite = characters2d [5];
		} else
		SelectedPlayerTransform.GetChild (5).GetComponent<SpriteRenderer> ().sprite = characters2d [Random.Range (0, characters2d.Length)];
		anim = SelectedPlayerTransform.GetChild (2).GetComponent<Animator> ();
		playerInfoUI = SelectedPlayerTransform.GetChild (0).GetChild (0).GetComponentsInChildren<Text> ();
		playerInfoCanvasGroup = SelectedPlayerTransform.GetChild (0).GetComponent<CanvasGroup> ();
		print (displayName);
		playerInfoUI [0].text = displayName;
		AttackVisualFXManager.Instance.targetHealth = playerInfoUI [1];
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
		EventManager.Instance.CallFreezeScale (false);
	
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
		EventManager.Instance.CallFreezeScale (true);
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime * moveSpeed;
			ZoomManage (t, focusPos);
			yourWitch.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 44,Mathf.SmoothStep (0, 1f, t));
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
		print (PlayerManager.marker.scale + "   " + PlayerManager.marker.instance.transform.localScale );

		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.scale = Mathf.SmoothStep (15, 0, t*2);
		SelectedPlayerTransform.localScale = Vector3.Lerp( Vector3.zero,Vector3.one*playerMarkerScale,  Mathf.SmoothStep (0, 1, t));
		SelectedPlayerTransform.localEulerAngles =   new Vector3 (0,  Mathf.SmoothStep (0, 243f, t),0);
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

	public void SpellSuccess( )
	{
		return;
		STM.enabled = false;
		resultScreen.GetComponent<CanvasGroup> ().alpha = 1;
		StartCoroutine (Attack ( ));
		StartCoroutine (lateDesat ());
	}

	IEnumerator Attack ( )
	{
		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime;
			dof.aperture = Mathf.SmoothStep (.6f, .1f, t);
			desat.basic.saturation = Mathf.SmoothStep (1, 0, t);
			SpellCastCG.alpha = Mathf.SmoothStep (1, 0, t);
			yield return null;
		}
//		var g = Utilities.InstantiateObject (explosions[ Random.Range (0, explosions.Length)], SelectedPlayer.instance.transform.GetChild(4));
//		var col =  g.GetComponent< RFX4_EffectSettingColor>();
//		if (type == 0)
//			col.Color = Utilities.Blue;
//		else if (type == 1)
//			col.Color = Utilities.Orange;
//		else if (type == -1)
//			col.Color = Utilities.Purple;

		print (SpellCarousel.currentSpell);

		if (SpellCarousel.currentSpell == "Hex" || SpellCarousel.currentSpell == "Waste"  || SpellCarousel.currentSpell == "Banish" ) {
			HexFX.SetActive (true);
		} else if (SpellCarousel.currentSpell == "Bless" || SpellCarousel.currentSpell == "Grace" || SpellCarousel.currentSpell == "Ressurect" ) {
			blessFX.SetActive (true);
		} else if (SpellCarousel.currentSpell == "SunEater") {
			SunEaterFX.SetActive (true);
		} else if (SpellCarousel.currentSpell == "WhiteFlame") {
			WhiteFlameFX.SetActive (true);
		} else if (SpellCarousel.currentSpell == "Silence" || SpellCarousel.currentSpell == "Bind" ) {
			SilenceFX.SetActive (true);
		} 
		if (SpellCarousel.currentSpell != "Bless" || SpellCarousel.currentSpell != "Grace" || SpellCarousel.currentSpell != "Ressurect") {
			anim.SetFloat ("Hits", Random.Range (0, 1.0f));
			anim.SetTrigger ("Hit");
		}
	}

	IEnumerator lateDesat()
	{
		yield return new WaitForSeconds (2.8f);
		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;
//		resultScreen.SetActive (true);
//		if(SpellCarousel.currentSpell == "Bless" || SpellCarousel.currentSpell == "Grace" )
//			resultInfo.text = "You cast <color=#ff9900> " + SpellCarousel.currentSpell + "</color> on " + MarkerSpawner.SelectedMarker.displayName + ", giving them <color=#ff9900>" + Random.Range(3,20).ToString() + " </color>Energy. You gain <color=#008bff>" + Random.Range(3,20).ToString() + " </color>XP." ;
//		else if(SpellCarousel.currentSpell == "Hex" || SpellCarousel.currentSpell == "Waste" || SpellCarousel.currentSpell == "SunEater" )
//			resultInfo.text = "You cast <color=#7200ff> " + SpellCarousel.currentSpell + "</color> on " + MarkerSpawner.SelectedMarker.displayName + ", draining them <color=#7200ff>" + Random.Range(3,20).ToString() + " </color>Energy. You gain <color=#008bff>" + Random.Range(3,20).ToString() + " </color>XP." ;
//		else if(SpellCarousel.currentSpell == "WhiteFlame" )
//			resultInfo.text = "You cast <color=#ff9900> " + SpellCarousel.currentSpell + "</color> on " + MarkerSpawner.SelectedMarker.displayName + ", draining them <color=#7200ff>" + Random.Range(3,20).ToString() + " </color>Energy. You gain <color=#008bff>" + Random.Range(3,20).ToString() + " </color>XP." ;
//		else if(SpellCarousel.currentSpell == "Ressurect" )
//			resultInfo.text = "You cast <color=#7200ff> " + SpellCarousel.currentSpell + "</color> on " + MarkerSpawner.SelectedMarker.displayName + ", giving them <color=#7200ff>" + Random.Range(3,20).ToString() + " </color>Energy. You gain <color=#008bff>" + Random.Range(3,20).ToString() + " </color>XP." ;
//		else if(SpellCarousel.currentSpell == "Bind" || SpellCarousel.currentSpell == "Banish" )
//			resultInfo.text = "You cast <color=#7200ff> " + SpellCarousel.currentSpell + "</color> on " + MarkerSpawner.SelectedMarker.displayName + ". You gain <color=#008bff>" + Random.Range(3,20).ToString() + " </color>XP." ;
//		
		float t = 0;
		float succ = Random.Range (30, 100);
		while (t <= 1f) {
			t += Time.deltaTime*.8f;
			dof.aperture = Mathf.SmoothStep (.6f, .1f, t);
			matchPercent.text =  ((int) Mathf.SmoothStep (0, succ, t)).ToString();
			matchProgress.fillAmount = Mathf.SmoothStep (0, succ*.01f, t);
			desat.basic.saturation = Mathf.SmoothStep (1, 0, t);
			prof.depthOfField.settings = dof;
			prof.colorGrading.settings = desat;
			yield return null;
		}
	}

	public void OnClickResult()
	{
		MainUICanvasGroup.gameObject.SetActive (true);
		StopAllCoroutines ();
		StartCoroutine (AttackZoomOut ());
	}

	IEnumerator AttackZoomOut( )
	{
		var CGA = resultScreen.GetComponent<CanvasGroup> ();
		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime * AttackToMainSpeed;

			foreach (var item in MarkerManager.Markers) {
				if (item.Value[0].inMapView) {
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
			}
			PlayerManager.marker.scale = Mathf.SmoothStep (15, 0, t*2);
			if(PlayerManager.physicalMarker != null)
				PlayerManager.physicalMarker.scale = Mathf.SmoothStep (15, 0, t*2);

			CGA.alpha = Mathf.SmoothStep (0, 1, t);
			SelectedPlayerTransform.localScale = Vector3.Lerp( Vector3.zero,Vector3.one*playerMarkerScale,  Mathf.SmoothStep (0, 1, t));
			SelectedPlayerTransform.localEulerAngles =   new Vector3 (0,  Mathf.SmoothStep (0, 271f, t),0);
			CamTransform.position =  Vector3.Lerp (curPosition, AttackTransform.position, Mathf.SmoothStep (0, 1f, t));
			CamTransform.rotation =  Quaternion.Slerp (curRotation, AttackTransform.rotation, Mathf.SmoothStep (0, 1f, t));
			Cam.fieldOfView= Mathf.SmoothStep (60f, 35f, t);
			MainUICanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
			RenderSettings.ambientLight = Color.Lerp (new Color (.875f, .875f, .875f), new Color (0,0,0), Mathf.SmoothStep (0, 1f, t));
			spotLight.intensity = Mathf.SmoothStep (0, 7f, t);
			dof.aperture = Mathf.SmoothStep (.6f, .1f, t);
			desat.basic.saturation = Mathf.SmoothStep (1, 0, t);
			prof.depthOfField.settings = dof;
			prof.colorGrading.settings = desat;
			yourWitch.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 41,Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
		SelectedPlayer.control.RemoveMarker3D (SelectedPlayer);
		SelectedPlayer = null;
		yourWitch.SetActive (false);
		SpellCastCG.gameObject.SetActive (false);
		Utilities.allowMapControl (true);
		resultScreen.SetActive(false);

	}

	public void OnClickSpell()
	{
//		StartCoroutine (moveToTargetPos ());
	}

	public void OnClickSpellRevert()
	{
//		StartCoroutine (moveBackToIni ());
	}


	IEnumerator moveToTargetPos ()
	{
		var CG = SelectedPlayerTransform.GetComponentInChildren<CanvasGroup> ();

		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime*attackSpeed;
			Cam.transform.position = Vector3.Lerp (camOnTapPos, AttackTransform.position, Mathf.SmoothStep (0, 1f, t));
			CG.alpha  = Mathf.SmoothStep (1f, 0f, t);
			CamTransform.rotation =  Quaternion.Slerp (camOnTapRot, AttackTransform.rotation, Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
	}

	IEnumerator moveBackToIni ()
	{
		var CG = SelectedPlayerTransform.GetComponentInChildren<CanvasGroup> ();
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime*attackSpeed;
			Cam.transform.position = Vector3.Lerp (camOnTapPos, AttackTransform.position, Mathf.SmoothStep (0, 1f, t));
			CG.alpha  = Mathf.SmoothStep (1f, 0f, t);
			CamTransform.rotation =  Quaternion.Slerp (camOnTapRot, AttackTransform.rotation, Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
	}
}
