using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpellSpiralLoader : MonoBehaviour {
	public static SpellSpiralLoader Instance{ get; set;}
	public List<CanvasGroup> highlights;
	public float waitTime = .2f;
	public Image bg;
	public CanvasGroup spellGlyph;
	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}
	void reset () {
		foreach (var item in highlights) {
			item.alpha = 0;
		}
	}

	public void LoadingStart()
	{
		spellGlyph = SpellSelectParent.Instance.sp.spellImg.GetComponent<CanvasGroup> ();
		reset ();
		StartCoroutine (FadeIn());
		StartCoroutine (Spiral ());
	}

	public void LoadingDone()
	{
		reset ();
		this.StopAllCoroutines ();
		StartCoroutine (FadeOut());
	}

	// Update is called once per frame
	IEnumerator Spiral()
	{
		for (int i = 0; i < highlights.Count; i++) {
			highlights [i].alpha = 1;

			if (i == 0) {
				highlights [highlights.Count - 3].alpha = 0;
			}
			if (i == 1) {
				highlights [highlights.Count - 2].alpha = 0;
			}
			if (i == 2) {
				highlights [highlights.Count - 1].alpha = 0;
			}

			if (i > 0) {
				highlights [i-1].alpha = .65f;
			}
			if (i > 1) {
				highlights [i-2].alpha = .25f;
			} 

			if (i > 2) {
				highlights [i-3].alpha = 0;
			} 
			yield return new WaitForSeconds (waitTime);
		}
		StartCoroutine (Spiral ());
	}

	IEnumerator FadeIn()
	{
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime*2;
			spellGlyph.alpha = Mathf.SmoothStep (1, .35f, t);
			bg.color = new Color(1,1,1, Mathf.SmoothStep (.2f, .4f, t));
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		float t = 1;
		while (t >= 0) {
			t -= Time.deltaTime*2;
			spellGlyph.alpha = Mathf.SmoothStep (1, .35f, t);
			bg.color = new Color(1,1,1, Mathf.SmoothStep (.2f, .4f, t));
			yield return null;
		}
	}
	
}
