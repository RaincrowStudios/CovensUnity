using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : UIAnimationManager {
	Camera cam;
	public Transform camBaseAnimPos;
	public Transform camEndSelectPos;
	public Transform camFinalPos;
	Transform camTransform;
	public float moveSpeed;
	public List<SpriteRenderer> chars = new List<SpriteRenderer> ();
	string currentSelect = "";
	public GameObject CharSelectUI;
	public ColliderScrollTrigger CS;
	public float delayWitches = 2f;
	public GameObject loadingObject;
	public GameObject blocker;
	public CanvasGroup LoginCanvas;
	public GameObject charSelectSprite;
	public float fadeSpeed=1.5f;
	public GameObject loginWheel;

	void init () {
		CS.EnterAction = Enter;
		CS.ExitAction = Exit;
		cam = Camera.main;
		camTransform = cam.transform;
//		foreach (var item in chars) {
//			int i = Random.Range (0, 2);
//			item.sprite = DownloadedAssets.charSelectArt [Random.Range (0, DownloadedAssets.charSelectArt.Count)]; 
//			if (i == 1)
//				item.transform.localScale = new Vector3 (item.transform.localScale.x * -1, item.transform.localScale.y, item.transform.localScale.z);
//		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			StartAnimation ();
		}
	}

	public void SkipCharSelect()
	{
		DownloadedAssets.charSelectArt.Clear ();
		Destroy (CharSelectUI);
		if (charSelectSprite != null)
			Destroy (charSelectSprite);
		Destroy (this);
	}

	public void StartAnimation()
	{
		print ("Starting Animation!");
		init ();
		Destroy (loginWheel);
		charSelectSprite.SetActive (true);
		CharSelectUI.SetActive (false);
//		StartCoroutine(MoveCam(camBaseAnimPos,camEndSelectPos,moveSpeed));
		StartCoroutine (FaddeCanvas ());
		ShowWitches ();
	}

	IEnumerator FaddeCanvas ()
	{
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime*fadeSpeed;
			LoginCanvas.alpha = Mathf.SmoothStep (1, 0, t);
			yield return 0;
		}
	}

	IEnumerator MoveCam(Transform inTransform, Transform outTransform, float speed)
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

	void ShowWitches()
	{
		Destroy (charSelectSprite);
		CharSelectUI.SetActive (true);
		CharSelectUI.GetComponent<Animator> ().Play ("in");
	}

	public void SelectionDone()
	{
		OnlineMaps.instance.transform.GetChild (0).gameObject.SetActive (false);
		loadingObject.SetActive (true);
		blocker.SetActive (true);
		LoginAPIManager.CreateCharacter (currentSelect);
	}

	public void OnCharacterGet()
	{
//		StartCoroutine (MoveCam (camEndSelectPos, camFinalPos,1.74f));
		print("Animating char Out!!!!");
		CharSelectUI.GetComponent<Animator> ().Play ("out");
		Invoke ("invokeInit", .7f);
	}

	void invokeInit()
	{
		LoginAPIManager.InitiliazingPostLogin ();
		WebSocketClient.websocketReady = true;
		Invoke ("SkipCharSelect", 1f);
	}

	void Enter(Transform tr)
	{
		tr.GetComponent<Animator> ().SetBool ("animate", true);
		currentSelect = tr.name;
	}

	void Exit(Transform tr)
	{
		tr.GetComponent<Animator> ().SetBool ("animate", false);
	}
}

