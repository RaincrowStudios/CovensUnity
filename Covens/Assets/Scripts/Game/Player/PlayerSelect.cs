using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerSelect : MonoBehaviour {

	Camera cam;
	public Transform StartPos;
	public Transform EndPos; 
	public Transform FocusPos; 
	bool startMoving = false;
	public float lerpSpeed = 1;
	public float focusSpeed = 1;
	public float rotateSpeed = 1;
	public float blurSpeed = 1;
	public float blurOutSpeed = 1;

	public float BlurDelay = 1;
	PostProcessingProfile prof;

	public GameObject TargetCanvas;
	public GameObject SpellCanvas;
	public GameObject FocusPlayerCanvas;
	public GameObject PlayerNameCanvas;
	public Transform TargetPlayer;

	bool isBlurred = false;

	public ParticleSystem PS;
	// Use this for initialization
	void OnEnable () {
		prof = GetComponent<PostProcessingBehaviour> ().profile;
		ResetCC ();
		StartCoroutine (SmoothMove (StartPos,EndPos,lerpSpeed));
		StartCoroutine (Blur(BlurDelay,blurSpeed));
	}

	public void FocusPlayerHelper()
	{
		this.StopAllCoroutines ();
		FocusPlayerCanvas.SetActive (false);
		StartCoroutine (SmoothMove (EndPos,FocusPos,focusSpeed));
		TargetCanvas.SetActive (true);
		StartCoroutine( SmoothRotate (true, rotateSpeed));
		if(isBlurred)
		StartCoroutine (Blur(0,blurOutSpeed,false));
		PlayerNameCanvas.SetActive (false);
	}

	public void RevertPlayerHelper()
	{
		this.StopAllCoroutines ();
		FocusPlayerCanvas.SetActive (true);
		StartCoroutine (SmoothMove (FocusPos,EndPos,focusSpeed));
		SpellCanvas.SetActive (true);
		StartCoroutine( SmoothRotate (false, rotateSpeed));
		StartCoroutine (Blur(BlurDelay,blurSpeed));
		PlayerNameCanvas.SetActive (true);
	}

	IEnumerator SmoothMove(Transform start, Transform End, float Speed)
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * Speed;
			transform.position = Vector3.Lerp (start.position, End.position, Mathf.SmoothStep (0, 1f, Mathf.SmoothStep (0, 1f, t)));
			transform.rotation = Quaternion.Lerp (start.rotation, End.rotation, Mathf.SmoothStep (0, 1f, Mathf.SmoothStep (0, 1f, t))); 
			yield return null;
		}
	}

	IEnumerator SmoothRotate(bool focus,float Speed)
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * Speed;
			if (focus) {
				TargetPlayer.localEulerAngles = new Vector3 (0, Mathf.SmoothStep (193f, 135f, t), 0); 
			}
			else
			TargetPlayer.localEulerAngles = new Vector3 (0, Mathf.SmoothStep (135f, 195f, t),0); 
			
			yield return null;
		}
	}


	IEnumerator Blur(float delay,float speed, bool isBlur = true)
	{
		yield return new WaitForSeconds (delay);

		if (isBlur) {
			isBlurred = true;
			var emission = PS.emission;
			emission.rateOverTime = 0;
		} else {
			isBlurred = false;
			var emission = PS.emission;
			emission.rateOverTime = 200;
		}

		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime*speed;
			if (isBlur) {
				dof.aperture = Mathf.SmoothStep (1, .1f, t);
				desat.basic.saturation = Mathf.SmoothStep (1, 0, t);
			} else {
				dof.aperture = Mathf.SmoothStep (.1f, 1f, t);
				desat.basic.saturation = Mathf.SmoothStep (0, 1.0f, t);
			}
			prof.depthOfField.settings = dof;
			prof.colorGrading.settings = desat;
			yield return null;
		}
	}

	public void ResetCC()
	{
		prof = GetComponent<PostProcessingBehaviour> ().profile;

		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;

		dof.aperture = 1;
		desat.basic.saturation =1;

		prof.depthOfField.settings = dof;
		prof.colorGrading.settings = desat;
	}

	public void resetPosition()
	{
		transform.position = EndPos.position;
		transform.rotation = EndPos.rotation;
	}
}
