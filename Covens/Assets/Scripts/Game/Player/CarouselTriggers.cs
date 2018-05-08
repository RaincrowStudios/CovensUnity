using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarouselTriggers : MonoBehaviour
{
	public float fadeSpeed =1;
	public float moveSpeed =1;
	public Transform currentButton;
	public enum TriggerPosition
	{
		Main,Side,Far
	};

	public TriggerPosition CurrentTriggerPos;


	void OnTriggerEnter(Collider col)
	{
		if (col.tag != "spells")
			return;
		currentButton = col.transform;
		if (CurrentTriggerPos == TriggerPosition.Main) {
			col.GetComponent<CanvasGroup> ().interactable = true;
			StartCoroutine(SmoothFade(col.transform,1,1.3f));
			SpellCarousel.Instance.SpellInfo (col.name);
			SpellSelectParent.Instance.sp = col.GetComponentInChildren<SpellSelect> ();
			turnOff ();
			HighlightFX (col.transform);
			SpellCarousel.currentSpell = col.name;
			col.transform.GetChild (1).gameObject.SetActive (true);
		} else if (CurrentTriggerPos == TriggerPosition.Side) {
			StartCoroutine(SmoothFade(col.transform,.35f,.9f));
			col.GetComponent<CanvasGroup> ().interactable = false;
			col.transform.GetChild (1).gameObject.SetActive (false);
			turnOff ();

		} else {
			StartCoroutine(SmoothFade(col.transform,.05f,.7f));
			col.GetComponent<CanvasGroup> ().interactable = false;
			col.transform.GetChild (1).gameObject.SetActive (false);
			turnOff ();

		}
	}

	IEnumerator SmoothFade(Transform tr,float finalAlpha,float finalScale)
	{
		var image = tr.GetComponent<CanvasGroup> ();
		float curScale = tr.localScale.x;
		float alpha = image.alpha;
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * fadeSpeed;
			image.alpha = Mathf.SmoothStep (alpha, finalAlpha, t);
			float s = Mathf.SmoothStep (curScale, finalScale, t);
			tr.localScale = new Vector3(s,s,s);
			yield return null;
		}
	}

	public void turnOff()
	{
		currentButton.GetChild (3).gameObject.SetActive (false);
		currentButton.GetChild (4).gameObject.SetActive (false);
		currentButton.GetChild (5).gameObject.SetActive (false);

	}

	void HighlightFX(Transform tr)
	{
		int degree = SpellCastAPI.spells [tr.name].school;
		if (degree == 1) {
			tr.GetChild (1).GetComponent<Image> ().color = Utilities.Orange;
			tr.GetChild (3).gameObject.SetActive (true);
		} else if (degree == -1) {
			tr.GetChild (1).GetComponent<Image> ().color = Utilities.Purple;
			tr.GetChild (4).gameObject.SetActive (true);
		
		} else {
			tr.GetChild (1).GetComponent<Image> ().color = Utilities.Blue;
			tr.GetChild (5).gameObject.SetActive (true);
		
		}
	}
}

