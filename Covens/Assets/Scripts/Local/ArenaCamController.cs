using UnityEngine;
using System.Collections;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
public class ArenaCamController : MonoBehaviour
{
	public static ArenaCamController Instance { get; set;}
	public Transform startView;
	public Transform topView;
	public Transform teamL;
	public Transform teamR;
	public Transform PlayerZoom;
	PostProcessingProfile prof;
	public float zoomSpeed = 1;
	float t = 0;
	public float lerpSpeed = 1;
	public float blurSpeed = .5f;
	Transform moveToTransform;
	bool startMoving;
	bool zoom;
	bool blur;
	bool canBlur = false;
	public GameObject playerSelectParticle;
	Camera cam;
	public Image DarkOverlay;
	public Animator ArenaAnim;
	CameraState camState = CameraState.TopView;
	// Use this for initialization
	enum CameraState
	{
		CovenA,CovenB,TopView,ActivePlayer
	};
	void Awake()
	{
		Instance = this;
	}

	void Start ()
	{
		prof = GetComponent<PostProcessingBehaviour> ().profile;
		cam = GetComponent<Camera> ();
		transform.SetPositionAndRotation (startView.position, startView.rotation);
		SetTopView ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (startMoving) {
			t += Time.deltaTime * lerpSpeed;
			transform.position = Vector3.Slerp (transform.position, moveToTransform.position, t);
			transform.rotation = Quaternion.Slerp (transform.rotation, moveToTransform.rotation, t);
		
			if (zoom) {
				if (cam.fieldOfView != 35)
					cam.fieldOfView = Mathf.SmoothStep (60, 35, t * zoomSpeed);
			} else {
				cam.fieldOfView = Mathf.SmoothStep (35, 60, t * zoomSpeed);
			}
			if (t * zoomSpeed >= 1) {
				startMoving = false;
				if (blur) {
					prof.depthOfField.enabled = true;
					DarkOverlay.gameObject.SetActive (true);
					StartCoroutine (blurControlOn ());
				}
			}
		}
	}

	IEnumerator blurControlOn()
	{
		var dof = prof.depthOfField.settings;
		playerSelectParticle.SetActive (true);
		if (dof.aperture >= .1f) {
			if(DarkOverlay.color.a<=.955f)
			DarkOverlay.color = new Color (1, 1, 1, DarkOverlay.color.a + 0.05f);
//			print (DarkOverlay.color.a);
			dof.aperture -= .01f;
			prof.depthOfField.settings = dof;
			yield return new WaitForSeconds (.01f);
			StartCoroutine (blurControlOn ());
		}

	}

	IEnumerator blurControlOff()
	{
		var dof = prof.depthOfField.settings;
		playerSelectParticle.SetActive (false);
		if (dof.aperture <= .5f) {
			dof.aperture += .025f;
			prof.depthOfField.settings = dof;
			yield return new WaitForSeconds (.015f);
			if(DarkOverlay.color.a>=0.08f)
				DarkOverlay.color = new Color (1, 1, 1, DarkOverlay.color.a - 0.08f);
			StartCoroutine (blurControlOff ());
		} else {
			prof.depthOfField.enabled = false;
			DarkOverlay.gameObject.SetActive (false);
		}

	}

	public void SetTopView( )
	{
		camState = CameraState.TopView;
		SetView ();
		moveToTransform = topView;
		zoom = false;
		blur = false;
		if (prof.depthOfField.enabled = true) {
			StartCoroutine (blurControlOff ());
		}
//		ArenaCharacterSpawner.Player.GetComponent<ArenaCharacterManager> ().deactive ();
	}

	public void SetTeamR( )
	{
		camState = CameraState.CovenB;
		SetView ();
		moveToTransform = teamR;
		blur = false;
		zoom = true;
		if (prof.depthOfField.enabled = true) {
			StartCoroutine (blurControlOff ());
		}

	}

	public void SetTeamL( )
	{
		camState = CameraState.CovenA;
		SetView ();
		moveToTransform = teamL;
		blur = false;
		zoom = true;
		if (prof.depthOfField.enabled = true) {
			StartCoroutine (blurControlOff ());
		}
	}

	public void SetZoomToPlayer()
	{
		camState = CameraState.ActivePlayer;
		SetView ();
		moveToTransform = PlayerZoom;
		zoom = true;
		blur = true;
		ArenaCharacterSpawner.Player.GetComponent<ArenaCharacterManager> ().active (); 

	} 

	void SetView( )
	{
		t = 0;
		startMoving = true;
		 if (camState == CameraState.CovenA) {

			foreach (var item in ArenaCharacterSpawner.CovenB) {
				item.GetComponent<ArenaCharacterManager> ().StatsManager (false);
			}
			foreach (var item in ArenaCharacterSpawner.CovenA) {
				item.GetComponent<ArenaCharacterManager> ().StatsManager (true);
			}

		} else if (camState == CameraState.CovenB) {

			foreach (var item in ArenaCharacterSpawner.CovenB) {
				item.GetComponent<ArenaCharacterManager> ().StatsManager (true);
			}
			foreach (var item in ArenaCharacterSpawner.CovenA) {
				item.GetComponent<ArenaCharacterManager> ().StatsManager (false);
			}

		} else {
//			print ("Disabling All");
			foreach (var item in ArenaCharacterSpawner.CovenA) {
				item.GetComponent<ArenaCharacterManager> ().StatsManager (true);
			}
			foreach (var item in ArenaCharacterSpawner.CovenB) {
				item.GetComponent<ArenaCharacterManager> ().StatsManager (true);
			}
		}
	}

	public void LightenArena()
	{
		ArenaAnim.SetBool ("Animate", false);
	}

	public void DarkenArena()
	{
		ArenaAnim.SetBool ("Animate", true);
	}

}

