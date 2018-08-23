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
	public List<Sprite> charSprites = new List<Sprite> ();

	public GameObject CharSelectUI;
	public ColliderScrollTrigger CS;
	public float delayWitches = 2f;


	// Use this for initialization
	void Start () {
		CS.EnterAction = Enter;
		CS.ExitAction = Exit;
		cam = Camera.main;
		camTransform = cam.transform;
		foreach (var item in chars) {
			int i = Random.Range (0, 2);
			item.sprite = charSprites [Random.Range (0, charSprites.Count)];
			if (i == 1)
				item.transform.localScale = new Vector3 (item.transform.localScale.x * -1, item.transform.localScale.y, item.transform.localScale.z);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			StartAnimation ();
			CharSelectUI.SetActive (false);
		}
	}
	
	public void StartAnimation()
	{
		StartCoroutine(MoveCam(camBaseAnimPos,camEndSelectPos));
		Invoke ("ShowWitches", delayWitches);
	}

	IEnumerator MoveCam(Transform inTransform, Transform outTransform)
	{
		Vector3 iniPos = inTransform.position; 
		Quaternion iniRot = inTransform.rotation;
		Vector3 endPos = outTransform.position; 
		Quaternion endRot = outTransform.rotation; 
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * moveSpeed;
			camTransform.position = Vector3.Lerp (iniPos, endPos, Mathf.SmoothStep(0.0f,1.0f,t)); 
			camTransform.rotation = Quaternion.Lerp (iniRot, endRot,  Mathf.SmoothStep(0.0f,1.0f,t));
			yield return 0;
		}
	}

	void ShowWitches()
	{
		CharSelectUI.SetActive (true);
//		var anim = CharSelectUI.GetComponent<Animator> ();
//		anim.SetBool ("open", true); 
	}


	void Enter(Transform tr)
	{
		tr.GetComponent<Animator> ().SetBool ("animate", true);
	}


	void Exit(Transform tr)
	{
		tr.GetComponent<Animator> ().SetBool ("animate", false);
	}
}

