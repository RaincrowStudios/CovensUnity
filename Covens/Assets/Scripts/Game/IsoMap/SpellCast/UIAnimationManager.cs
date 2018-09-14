using UnityEngine;
using System.Collections;
public class UIAnimationManager : MonoBehaviour
{
	public void Show(GameObject g, bool isStop = true)
	{
		if (isStop) {
			this.StopAllCoroutines ();
		}
		try{ 
			g.SetActive(true);
		var anim = g.GetComponent<Animator> ();
			anim.SetBool ("open", true); 
		}catch{
			Debug.LogError ("Please attach menuOpen animation to : " + g.name); 
		}
	} 
	public void Hide(GameObject g, bool isDisable = true , float disableTime = .59f) 
	{ 
		try{
			var anim = g.GetComponent<Animator> (); 
			anim.SetBool ("open", false); 
			if(isDisable)
				StartCoroutine(disableObject(g,disableTime));
		}catch{ 
			Debug.LogError ("Please attach menuOpen animation to : " + g.name); 
		}
	}

	public void Disable(GameObject g, float delay)
	{
		StartCoroutine(disableObject(g,1.3f));
	}

	IEnumerator disableObject(GameObject g,float delay)
	{
		yield return new WaitForSeconds (delay);
		g.SetActive (false);
	}

	public IEnumerator FadeIn(GameObject g,float speed=1)
	{
		g.SetActive (true);
		float t = 0;
		if (g.GetComponent<CanvasGroup> () == null)
			g.AddComponent<CanvasGroup> ();
		var cg = g.GetComponent<CanvasGroup> ();
		var tr = g.transform;
		if (cg.alpha == 1)
			yield break;
		while (t<=1) {
			t += Time.deltaTime * speed;
			cg.alpha = Mathf.SmoothStep (0, 1, t);
			tr.localScale = Vector3.one * Mathf.SmoothStep (0, 1, t);
		}
	}

	public IEnumerator FadeOut(GameObject g,float speed=1)
	{
		float t = 0;
		if (g.GetComponent<CanvasGroup> () == null)
			g.AddComponent<CanvasGroup> ();
		var cg = g.GetComponent<CanvasGroup> ();
		if (cg.alpha == 0)
			yield break;
		var tr = g.transform;
		while (t<=1) {
			t += Time.deltaTime * speed;
			cg.alpha = Mathf.SmoothStep (1, 0, t);
			tr.localScale = Vector3.one * Mathf.SmoothStep (1, 0, t);
		}
		g.SetActive (false);
	}
}

