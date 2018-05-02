using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class HitPostFX : MonoBehaviour {
	PostProcessingProfile prof;
	public float speed = 5;
	int flicks ;
	int flicksCount = 0  ;


	void OnEnable()
	{
		prof = Camera.main.GetComponent<PostProcessingBehaviour> ().profile;
		StartFlicker ();
	}


	public void StartFlicker()
	{
		flicksCount = 0;
		prof.chromaticAberration.enabled = true;
		var cr = prof.chromaticAberration.settings;
		cr.intensity = 0;
		prof.chromaticAberration.settings = cr;

		var exposure = prof.colorGrading.settings;
		exposure.basic.postExposure = 0;
		prof.colorGrading.settings = exposure;

		var bloom = prof.bloom.settings;
		
		bloom.lensDirt.intensity = 0;
		bloom.bloom.intensity = 2.14f;
		bloom.bloom.threshold = 0.98f;
		prof.bloom.settings = bloom;

		flicks = Random.Range (2, 4);
		StartCoroutine (Flicker (Random.Range(2,6)));
	}

	IEnumerator Flicker( float multiplier )
	{
		var exposure = prof.colorGrading.settings;
		var bloom = prof.bloom.settings;
		var cr = prof.chromaticAberration.settings;

		float crIntensity = Random.Range (1, 3);
		float blDirtIntensity = Random.Range (1, 5);
		float expIntensity = Random.Range (1, 5);
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime*speed*multiplier;
			exposure.basic.postExposure= Mathf.SmoothStep (0, expIntensity, t*Random.Range(0.5f,2f));
			cr.intensity = Mathf.SmoothStep (0, crIntensity, t*Random.Range(0.5f,2f));
			bloom.lensDirt.intensity =  Mathf.SmoothStep (0, blDirtIntensity, t*Random.Range(0.5f,2f));;
			bloom.bloom.threshold = Mathf.SmoothStep (.98f, 0, t*Random.Range(0.5f,2f));
			prof.colorGrading.settings = exposure;
			prof.bloom.settings = bloom;
			prof.chromaticAberration.settings = cr;
//			print (t + " t");
			yield return null;
		}

		float k = 1;
		while (k>=0) {
			k -= Time.deltaTime*speed*multiplier;
			exposure.basic.postExposure= Mathf.SmoothStep (0, expIntensity, k *Random.Range(0.5f,2f));
			cr.intensity = Mathf.SmoothStep (0, crIntensity, k*Random.Range(0.5f,2f) );
			bloom.lensDirt.intensity =  Mathf.SmoothStep (0, blDirtIntensity, k*Random.Range(0.5f,2f) );;
			bloom.bloom.threshold = Mathf.SmoothStep (.98f, 0, k *Random.Range(0.5f,2f));
			prof.colorGrading.settings = exposure;
			prof.bloom.settings = bloom;
			prof.chromaticAberration.settings = cr;
//			print (t + " k");
			yield return null;
		}
		flicksCount++;
		if (flicksCount < flicks) {
			StartCoroutine (Flicker (Random.Range(2,6)));
		}
	}
}
