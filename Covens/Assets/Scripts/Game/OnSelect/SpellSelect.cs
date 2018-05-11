using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SpellSelect : MonoBehaviour
{

    RectTransform spellMask;
	public Image spellImg;
	public Image containerImage;
	public float moveSpeed = 1;
	public ParticleSystem[] particle;
	public CanvasGroup[] glow;
	public float speed;
	public GameObject[] glowObjects;
	void OnEnable()
	{
		spellImg.transform.localScale = Vector3.one;
		containerImage.color = Color.white;
		GetComponentInParent<CanvasGroup> ().interactable = true;
		if(spellMask == null)
			spellMask = GameObject.FindGameObjectWithTag("spellMask").GetComponent<RectTransform>();
	}

	public void onClick()
	{
		print ("CLicked");
		SpellSelectParent.Instance.sp = this;
		StartCoroutine (FocusSpell ());
		SpellSelectParent.Instance.OnClick();
		SpellSelectParent.Instance.CarouselFadeOut ();
		GetComponentInParent<CanvasGroup> ().interactable = false;
	}

	public void onClickIngredientSpell()
	{
		IngredientsUI.Instance.OnClick (this);
		SpellSelectParent.Instance.CarouselFadeOut ();
		HideGlow ();
	}

	IEnumerator FocusSpell()
	{
		spellMask.anchoredPosition= new Vector2 (0 ,156);
		containerImage.color = new Color (1, 1, 1,0);

		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * moveSpeed;

			spellImg.transform.localScale = Vector3.Lerp (Vector3.one, Vector3.one * 3.4f, Mathf.SmoothStep (0, 1f, t));
			spellImg.color = new Color (1, 1, 1, Mathf.SmoothStep (0, 1f, t));
			yield return null;
		}
	}

	public void onRevert()
	{
		StartCoroutine (Revert ());
		GetComponentInParent<CanvasGroup> ().interactable = true;
		Invoke ("ShowGlow", .5f);
	}

	 IEnumerator Revert()
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * moveSpeed;
			spellMask.anchoredPosition= new Vector2 (0,Mathf.SmoothStep (-171, 156, t));
			spellImg.transform.localScale = Vector3.Lerp (Vector3.one, Vector3.one * 3.4f, Mathf.SmoothStep (0, 1f, t));
			containerImage.color = new Color (1, 1, 1, Mathf.SmoothStep (1, 0f, t));
			yield return null;
		}
	}

	public void Reset()
	{
		spellMask.anchoredPosition= new Vector2 (0,-171);
		spellImg.transform.localScale = Vector3.one;
		containerImage.color = Color.white;
		GetComponent<CanvasGroup> ().interactable = true;
	}


	public void HideGlow()
	{
//		foreach (var item in particle) {
//			var em = item.emission;
//			em.enabled = false; 
//		}
		foreach (var item in glowObjects) {
			item.SetActive (false);
		}

		foreach (var item in glow) {
			StartCoroutine (FadeOut (item));

		}

	}

	public void showGlow()
	{
//		foreach (var item in particle) {
//			var em = item.emission;
//			em.enabled = true; 
//		}
		foreach (var item in glowObjects) {
			item.SetActive (true);
		}

		foreach (var item in glow) {
			StartCoroutine (FadeIn (item));

		}
		GetComponent<CanvasGroup> ().interactable = true;

	}

	IEnumerator FadeOut(CanvasGroup CG)
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
	}

	IEnumerator FadeIn(CanvasGroup CG)
	{
		float t = 0;
		while (t <= 1f) {
			t =+ Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
	}
}

