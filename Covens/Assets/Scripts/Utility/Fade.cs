using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Fade : MonoBehaviour {
	bool SpellAlpha = false;
	public float timer = 0f;
	public float fadespeed=1f;
	public bool isClose = false;
	CanvasGroup CG;

	void OnEnable()
	{
		CG = GetComponent<CanvasGroup> ();
		CG.alpha = 0;
		StartCoroutine (FadeIn ());
	}

	public void FadeInHelper()
	{
		StartCoroutine (FadeIn ());
	}

	IEnumerator FadeIn()
	{
		yield return new WaitForSeconds (timer);
		float t = 0;
	
		while (t<1) {
			t += Time.deltaTime * fadespeed;
			CG.alpha = t;
			yield return null;
		}
	}

	public void FadeOutHelper()
	{
		StartCoroutine (FadeOut ());
	}
	IEnumerator FadeOut()
	{
		float t = 1;
		while (t>0) {
			t -= Time.deltaTime * fadespeed;
			CG.alpha = t;
			yield return null;
		}
		if (isClose)
			gameObject.SetActive (false);
	}


}
