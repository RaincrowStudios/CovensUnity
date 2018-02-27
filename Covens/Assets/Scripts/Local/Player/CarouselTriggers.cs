using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarouselTriggers : MonoBehaviour
{
	public float fadeSpeed =1;
	public float moveSpeed =1;
	public enum TriggerPosition
	{
		Main,Side,Far
	};

	public TriggerPosition CurrentTriggerPos;
	// Use this for initialization
	void Start ()
	{
	
	}


	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag != "spells")
			return;

		if (CurrentTriggerPos == TriggerPosition.Main) {
			StartCoroutine(SmoothFade(col.transform,1,1.3f));
			GetComponentInParent<SpellCarousel> ().SpellInfo (col.name);
			col.transform.GetChild (0).gameObject.SetActive (true);
		} else if (CurrentTriggerPos == TriggerPosition.Side) {
			StartCoroutine(SmoothFade(col.transform,.35f,1));
			col.transform.GetChild (0).gameObject.SetActive (false);
		} else {
			StartCoroutine(SmoothFade(col.transform,.1f,.8f));
			col.transform.GetChild (0).gameObject.SetActive (false);
		}
	}

	IEnumerator SmoothFade(Transform tr,float finalAlpha,float finalScale)
	{
		var image = tr.GetComponent<Image> ();
		float curScale = tr.localScale.x;
		float alpha = image.color.a;
		float t = 0;
		while (t <= 1f) {
//			print ("MOve");
			t += Time.deltaTime * fadeSpeed;
			image.color = new Color (image.color.r, image.color.g, image.color.b, Mathf.SmoothStep (alpha, finalAlpha, t));
			float s = Mathf.SmoothStep (curScale, finalScale, t);
			tr.localScale = new Vector3(s,s,s);
			yield return null;
		}
	}
}

