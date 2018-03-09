using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SpellSelect : MonoBehaviour
{

	public RectTransform spellMask;
	Image spellImg;
	Image containerImage;
	public float moveSpeed = 1;
	

	public void onClick()
	{
		containerImage = GetComponent<Image>();
		spellImg = transform.GetChild (1).GetComponent<Image> ();
		SpellSelectParent.Instance.sp = this;
		StartCoroutine (FocusSpell ());
		SpellSelectParent.Instance.OnClick();
		GetComponent<CanvasGroup> ().interactable = false;

	}

	IEnumerator FocusSpell()
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * moveSpeed;
			spellMask.anchoredPosition= new Vector2 (0,Mathf.SmoothStep (-170f, 255f, t));
			spellImg.transform.localScale = Vector3.Lerp (Vector3.one, Vector3.one * 5.5f, Mathf.SmoothStep (0, 1f, t));
			containerImage.color = new Color (1, 1, 1, Mathf.SmoothStep (1, 0f, t));
			yield return null;
		}
	}

	public void onRevert()
	{
		StartCoroutine (Revert ());
		GetComponent<CanvasGroup> ().interactable = true;

	}

	 IEnumerator Revert()
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * moveSpeed;
			spellMask.anchoredPosition= new Vector2 (0,Mathf.SmoothStep (-170f, 255f, t));
			spellImg.transform.localScale = Vector3.Lerp (Vector3.one, Vector3.one * 5.5f, Mathf.SmoothStep (0, 1f, t));
			containerImage.color = new Color (1, 1, 1, Mathf.SmoothStep (1, 0f, t));
			yield return null;
		}
	}
}

