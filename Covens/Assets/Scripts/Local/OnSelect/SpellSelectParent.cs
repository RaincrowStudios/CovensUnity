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

	public GameObject goBackNPCButton;
	public GameObject goBackPlayerButton;
	public GameObject NPCDetail;
	public GameObject MaskNPC;
	public GameObject MaskPlayer;

	public GameObject Ingredients;

	void Awake()
	{
		Instance = this;
		SR = GetComponent<ScrollRect> ();
	}

	public void SetupSpellCast()
	{
		if (OnPlayerSelect.isPlayer) {
			NPCDetail.SetActive (false);
			goBackPlayerButton.SetActive (true);
			goBackNPCButton.SetActive (false);
			MaskNPC.SetActive(false);
			MaskPlayer.SetActive (true);	
		} else {
			NPCDetail.SetActive (true);
			goBackPlayerButton.SetActive (false);
			goBackNPCButton.SetActive (true);
			MaskNPC.SetActive(true);
			MaskPlayer.SetActive (false);
		}
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

		if (OnPlayerSelect.isPlayer) {
			NPCDetail.SetActive (false);
			goBackPlayerButton.SetActive (true);
		} else {
			NPCDetail.SetActive (true);
			goBackNPCButton.SetActive (true);
		}
		Ingredients.SetActive (false);
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
		Ingredients.SetActive (true);
		SR.horizontal = false;
		goBackNPCButton.SetActive (false);
		goBackPlayerButton.SetActive (false);
		NPCDetail.SetActive (false);
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

