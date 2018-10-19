using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpellSpiralLoader : UIAnimationManager {
	public static SpellSpiralLoader Instance{ get; set;}
	public List<CanvasGroup> highlights;
	public float waitTime = .2f;
	public CanvasGroup spellDesc;
	public GameObject spellGlyph;
	public GameObject spellAccuracy;
	public Text spellAccuracyText;
	public GameObject loadingFX;
	public ParticleSystem prominence;
	public ParticleSystem rays;
	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}


	public void LoadingStart(bool isTrue)
	{
		if (isTrue) {
			loadingFX.SetActive (true);
			var k = prominence.emission;
			k.rateOverTime = Random.Range (10, 15);
			var j = rays.emission;
			j.rateOverTime = 150;
//		Hide (spellGlyph);
//		Show (spellAccuracy);
			if(gameObject.activeInHierarchy)
			StartCoroutine (this.FadeIn ());
//		StartCoroutine (SpellFakeFX() );
//		StartCoroutine (this.CountUp ());

		} else {
			LoadingDone ();
		}
	}

//	IEnumerator SpellFakeFX() 
//	{
//		yield return new WaitForSeconds (Random.Range (.5f,1));
//		LoadingDone ();
//	}
//
	IEnumerator CountUp()
	{
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime;
//			spellAccuracyText.text = Mathf.RoundToInt( Mathf.SmoothStep (0, SpellCastUIManager.SpellAccuracy, t)).ToString () + "%";
//			print ("running");
			yield return null;
		}
	}


	public void LoadingDone()
	{
		var k = prominence.emission;
		k.rateOverTime = 0;
		var j = rays.emission;
		j.rateOverTime = 0;
		Disable (loadingFX, 3);
		this.StopAllCoroutines ();
		StartCoroutine (this.FadeOut());
		Hide (spellAccuracy);
	}

	IEnumerator FadeIn()
	{
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime*1;
//			spellGlyph.color = new Color(1,1,1, Mathf.SmoothStep (1, .35f, t));
			spellDesc.alpha =  Mathf.SmoothStep (1, .12f, t);
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime*1;
//			spellGlyph.color = new Color(1,1,1, Mathf.SmoothStep (1, .35f, t));
			spellDesc.alpha =  Mathf.SmoothStep (1, .12f, t);
			yield return null;
		}
	
	}
	
}
