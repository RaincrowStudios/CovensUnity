using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SpellSelectParent : MonoBehaviour
{
	public static SpellSelectParent Instance { get; set;}
	[HideInInspector]
	public SpellSelect sp;
	public float speed = 1;
	public SpellTraceManager STM;
	public CanvasGroup[] spells; 
	public CanvasGroup BlackBG;
	ScrollRect SR;

	public GameObject[] goBack;
	void Awake()
	{
		Instance = this;
		SR = GetComponent<ScrollRect> ();
	}


	public void Revert()
	{
		STM.enabled = false;
		sp.onRevert ();
		var cg = sp.GetComponent<CanvasGroup> ();
		foreach (var item in spells) {
			if (item != cg) {
				StartCoroutine (FadeIn (item));
			}
		}
		SR.horizontal = true;
		foreach (var item in goBack) {
			item.SetActive(true) ;
		}
	}

	public void OnClick()
	{
		STM.enabled = true;
		var cg = sp.GetComponent<CanvasGroup> ();
		foreach (var item in spells) {
			if (item != cg) {
				StartCoroutine (FadeOut (item));
			}
		}
		SR.horizontal = false;
		foreach (var item in goBack) {
			item.SetActive(false) ;
		}
	}

	IEnumerator FadeOut(CanvasGroup CG)
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * speed;
			BlackBG.alpha = Mathf.SmoothStep (0, 1f, t);

			CG.alpha = Mathf.SmoothStep (1, 0f, t);
			yield return null;
		}
	}
	IEnumerator FadeIn(CanvasGroup CG)
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * speed;
			BlackBG.alpha = Mathf.SmoothStep (1, 0f, t);
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
	}

}

