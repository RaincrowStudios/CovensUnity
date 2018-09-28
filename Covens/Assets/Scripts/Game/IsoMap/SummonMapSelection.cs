using UnityEngine;
using System.Collections;

public class SummonMapSelection : MonoBehaviour
{
	public static SummonMapSelection Instance { get; set; }
	public static CurrentView currentView = CurrentView.MapView;
	public float speed = 1;
	public Transform summonTransform;
	public Transform summonTransformCast;
	public GameObject mainUICanvas;
//	public CanvasGroup playerIconCG;
	public GameObject SummonCanvas;
	public Light spotlight;
	public GameObject summonCircle;
//	public Animator wardrobeAnimator;

	private CanvasGroup mainUICG;
	private Camera cam;
	private Transform camTransform;
	private Vector3 camInitialPos;
	private Quaternion camInitialRot;
	private Vector2 curMapPos;
	public static Vector2 newMapPos;

	

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

	public void OnSummonStart( )
	{
		summonCircle.SetActive (true);
		currentView = CurrentView.TransitionView;
		EventManager.Instance.CallFreezeScale (false);
		Utilities.allowMapControl (false);
//		wardrobeAnimator.enabled = false;
		StartCoroutine (ZoomIn());
	}


	IEnumerator ZoomIn( )
	{
		var map = OnlineMaps.instance;
		newMapPos = curMapPos = OnlineMaps.instance.position; 
		newMapPos.x += Random.Range (-.004f, .004f);
		newMapPos.y += Random.Range (-.004f, .004f);
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * speed;
			ZoomManager (t);
			OnlineMaps.instance.position = Vector2.Lerp (curMapPos, newMapPos, t);
			yield return null;
		}
		currentView = CurrentView.IsoView;
		mainUICanvas.SetActive (false);
		SummonCanvas.SetActive (true);
		SummonUIManager.Instance.Init ();
	}

	void ZoomManager(float t)
	{
		foreach (var item in MarkerManager.Markers) {
			var data = item.Value[0].customData as Token;
			item.Value[0].scale = Mathf.SmoothStep (data.scale, 0, t);
		}

		PlayerManager.marker.scale = Mathf.SmoothStep (24, 0, t*2);

		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.scale = Mathf.SmoothStep (15, 0, t*2);
		cam.fieldOfView= Mathf.SmoothStep (60, 35f, t);
		camTransform.position =  Vector3.Lerp (camInitialPos, summonTransform.position, Mathf.SmoothStep (0, 1f, t));
		camTransform.rotation =  Quaternion.Slerp (camInitialRot, summonTransform.rotation, Mathf.SmoothStep (0, 1f, t));
		mainUICG.alpha = Mathf.SmoothStep (1f, 0f, t);
//		playerIconCG.alpha = Mathf.SmoothStep (1f, 0f, t);
		summonCircle.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 57,Mathf.SmoothStep (0, 1f, t)); 
//		summonCircle.transform.localEulerAngles = new Vector3 (-90,  0,Mathf.SmoothStep (0, 243f, t));

		RenderSettings.ambientLight = Color.Lerp (Color.white, new Color (0,0,0), Mathf.SmoothStep (0, 1f, t));
		spotlight.intensity = Mathf.SmoothStep (0, 2f, t);
	}

	IEnumerator ZoomOut()
	{
		float t = 1;
		while (t>=0) {
			t -= Time.deltaTime * speed;
			ZoomManager (t);
			yield return null;
		}
		currentView = CurrentView.MapView;
		EventManager.Instance.CallMapViewSet ();
		Utilities.allowMapControl (true);
		SummonCanvas.SetActive (false);
		summonCircle.SetActive (false);
//		wardrobeAnimator.enabled = true;

	}

	public void GoBack()
	{
		currentView = CurrentView.TransitionView;
		mainUICanvas.SetActive (true);
		StartCoroutine (ZoomOut ());
		SummonUIManager.Instance.Close ();
	}

	public void MoveCamCast()
	{
		StartCoroutine (MoveCam (summonTransform, summonTransformCast));
	}

	public void CastGoBack()
	{
		currentView = CurrentView.TransitionView;
		mainUICanvas.SetActive (true);
		StartCoroutine (ZoomOutCast ());
		SummonUIManager.Instance.Close ();

	}


	void ZoomManagerCast(float t)
	{
		foreach (var item in MarkerManager.Markers) {
			var data = item.Value[0].customData as Token;
			item.Value[0].scale = Mathf.SmoothStep (data.scale, 0, t);
		}

		PlayerManager.marker.scale = Mathf.SmoothStep (24, 0, t*2);

		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.scale = Mathf.SmoothStep (15, 0, t*2);
		cam.fieldOfView= Mathf.SmoothStep (60, 35f, t);
		camTransform.position =  Vector3.Lerp (camInitialPos, summonTransformCast.position, Mathf.SmoothStep (0, 1f, t));
		camTransform.rotation =  Quaternion.Slerp (camInitialRot, summonTransformCast.rotation, Mathf.SmoothStep (0, 1f, t));
		mainUICG.alpha = Mathf.SmoothStep (1f, 0f, t);
		summonCircle.transform.localScale = Vector3.Lerp (Vector3.zero,Vector3.one* 57,Mathf.SmoothStep (0, 1f, t)); 
		//		summonCircle.transform.localEulerAngles = new Vector3 (-90,  0,Mathf.SmoothStep (0, 243f, t));

		RenderSettings.ambientLight = Color.Lerp (Color.white, new Color (0,0,0), Mathf.SmoothStep (0, 1f, t));
		spotlight.intensity = Mathf.SmoothStep (0, 2f, t);
	}

	IEnumerator ZoomOutCast()
	{
		float t = 1;
		while (t>=0) {
			t -= Time.deltaTime * speed;
			ZoomManagerCast (t);
			yield return null;
		}
		currentView = CurrentView.MapView;
		EventManager.Instance.CallMapViewSet ();
		Utilities.allowMapControl (true);
		SummonCanvas.SetActive (false);
		summonCircle.SetActive (false);

	}


	IEnumerator MoveCam(Transform inTransform, Transform outTransform)
	{
		Vector3 iniPos = inTransform.position; 
		Quaternion iniRot = inTransform.rotation;
		Vector3 endPos = outTransform.position; 
		Quaternion endRot = outTransform.rotation; 
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * speed;
			camTransform.position = Vector3.Lerp (iniPos, endPos, Mathf.SmoothStep(0.0f,1.0f,t)); 
			camTransform.rotation = Quaternion.Lerp (iniRot, endRot,  Mathf.SmoothStep(0.0f,1.0f,t));
			yield return 0;
		}
	}
}

