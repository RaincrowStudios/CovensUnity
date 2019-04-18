using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsHandler : MonoBehaviour {
	[SerializeField] private GameObject CreditsPopup;
	[SerializeField] private CanvasGroup CreditsP_CG;
	[SerializeField] private GameObject ScrollingRect;
	[SerializeField] private CanvasGroup Scro_R_CG;
	[SerializeField] private Button CloseCredits;
	[Header("Spirit1")]
	[SerializeField] private CanvasGroup Spirit1_pic;
	private GameObject Spirit1_PicTrans;
	[SerializeField] private CanvasGroup Spirit1BG_one;
	private GameObject Spirit1BG_oneTrans;
	[SerializeField] private CanvasGroup Spirit1BG_two;
	private GameObject Spirit1BG_twoTrans;
	[Header("Spirit2")]
	[SerializeField] private CanvasGroup Spirit2_pic;
	private GameObject Spirit2_picTrans;
	[SerializeField] private CanvasGroup Spirit2BG_one;
	private GameObject Spirit2BG_oneTrans;
	[SerializeField] private CanvasGroup Spirit2BG_two;
	private GameObject Spirit2BG_twoTrans;
	[Header("Runes")]
	[SerializeField] private GameObject SideRuneL;
	[SerializeField] private GameObject SideRuneR;
	[SerializeField] private GameObject MidRuneL;
	[SerializeField] private GameObject MidRuneR;
	private Vector3 prescale;
	private Vector3 postscale;
	private Vector3 idlescale;




	// Use this for initialization
	IEnumerator Start () {



		ScrollingRect.transform.GetChild (0).GetComponent<CanvasGroup> ().alpha = 1;
		ScrollingRect.transform.GetChild (1).GetComponent<CanvasGroup> ().alpha = 1;
		ScrollingRect.transform.GetChild (2).GetComponent<CanvasGroup> ().alpha = 1;
		//prepping Spirit1 anim
		prescale.Set (0.8f, 0.8f, 0.8f);
		postscale.Set (1f, 1f, 1f);
		idlescale.Set (1.2f, 1.2f, 1.2f);
		//
		Spirit1_PicTrans = Spirit1_pic.gameObject;
		LeanTween.scale (Spirit1_PicTrans, prescale, 0.1f);
		Spirit1BG_oneTrans = Spirit1BG_one.gameObject;
		LeanTween.scale (Spirit1BG_oneTrans, prescale, 0.1f);
		Spirit1BG_twoTrans = Spirit1BG_two.gameObject;
		LeanTween.scale (Spirit1BG_twoTrans, prescale, 0.1f);
		Spirit1_pic.alpha = 0;
		Spirit1BG_one.alpha = 0;
		Spirit1BG_two.alpha = 0;
		//prepping Spirit2 anim
		Spirit2_picTrans = Spirit2_pic.gameObject;
		LeanTween.scale (Spirit2_picTrans, prescale, 0.1f);
		Spirit2BG_oneTrans = Spirit2BG_one.gameObject;
		LeanTween.scale (Spirit2BG_oneTrans, prescale, 0.1f);
		Spirit2BG_twoTrans = Spirit2BG_two.gameObject;
		LeanTween.scale (Spirit2BG_twoTrans, prescale, 0.1f);
		Spirit2_pic.alpha = 0;
		Spirit2BG_two.alpha = 0;
		Spirit2BG_one.alpha = 0;
		//
		CreditsP_CG.alpha = 0;
		MidRuneL.SetActive (false);
		MidRuneR.SetActive (false);
		MidRuneL.GetComponent<CanvasGroup> ().alpha = 0;
		MidRuneR.GetComponent<CanvasGroup> ().alpha = 0;
		Scro_R_CG.alpha = 0;

		int index = Random.Range (0,PlayerDataManager.config.summoningMatrix.Count-3);
		WWW www = new WWW(DownloadAssetBundle.baseURL + "spirit/" + PlayerDataManager.config.summoningMatrix[index].spirit + ".png");
		yield return www;

		if (www.texture != null)
		{
			Spirit1_pic.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
		}
		else
		{
			Debug.LogError("error loading hint spirit sprite: " +PlayerDataManager.config.summoningMatrix[index].spirit);
		}

		WWW www1 = new WWW(DownloadAssetBundle.baseURL + "spirit/" + PlayerDataManager.config.summoningMatrix[index+1].spirit + ".png");
		yield return www1;

		if (www1.texture != null)
		{
			Spirit2_pic.GetComponent<Image>().sprite = Sprite.Create(www1.texture, new Rect(0, 0, www1.texture.width, www1.texture.height), new Vector2(0, 0));
		}
		else
		{
			Debug.LogError("error loading hint spirit sprite: " +PlayerDataManager.config.summoningMatrix[index+1].spirit);
		}

		CreditScroll ();


	}
	void CreditScroll() {
			LeanTween.moveLocalY (ScrollingRect, -901f, 0.01f);
			LeanTween.alphaCanvas (CreditsP_CG, 1f, 0.5f).setEase(LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas (Scro_R_CG, 1f, 1f);
			LeanTween.value(0f,1f, 1f).setOnComplete(() => {
				LeanTween.value(0f,1f, 5.5f).setOnComplete(() => { //time to fade title
					var t = ScrollingRect.transform.GetChild(0).GetComponent<CanvasGroup>();
					var y = ScrollingRect.transform.GetChild(1).GetComponent<CanvasGroup>();
					var u = ScrollingRect.transform.GetChild(2).GetComponent<CanvasGroup>();
					LeanTween.alphaCanvas(t, 0f, 1f).setEase(LeanTweenType.easeOutCubic);
					LeanTween.alphaCanvas(y, 0f, 1f).setEase(LeanTweenType.easeOutCubic);
					LeanTween.alphaCanvas(u, 0f, 2f).setEase(LeanTweenType.easeOutCubic);
				});
				LeanTween.moveLocalY (ScrollingRect, 6863f, 45f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() => closeCredits());
				LeanTween.value(0f,1f, 10f).setOnComplete(() => { //time for spiritOne
					AnimateSpiritOne();
					LeanTween.value (0f, 1f, 6f).setOnComplete(() => { //time for midrunes
						MidRuneL.SetActive(true);
						MidRuneR.SetActive(true);
						LeanTween.alphaCanvas (MidRuneL.GetComponent<CanvasGroup>(), 0.5f, 2f).setEase(LeanTweenType.easeInCubic);
						LeanTween.alphaCanvas (MidRuneR.GetComponent<CanvasGroup>(), 0.5f, 2f).setEase(LeanTweenType.easeInCubic);
						LeanTween.value(0f, 1f, 4f).setOnComplete (() => { //time to fade midrunes
							LeanTween.alphaCanvas (MidRuneL.GetComponent<CanvasGroup>(), 0f, 2f).setEase(LeanTweenType.easeOutCubic);
							LeanTween.alphaCanvas (MidRuneR.GetComponent<CanvasGroup>(), 0f, 2f).setEase(LeanTweenType.easeOutCubic);
							LeanTween.value(0f,1f,2f).setOnComplete(() => { //time for spirit 2
								AnimateSpiritTwo();
							});
						});

					});
				});
			});

		}
	void AnimateSpiritOne () {
		LeanTween.alphaCanvas (Spirit1BG_one, 1f, 1f).setEase (LeanTweenType.easeInCubic);
		LeanTween.scale (Spirit1BG_oneTrans, postscale, 1.5f).setEase (LeanTweenType.easeInOutCubic);
		LeanTween.alphaCanvas (Spirit1BG_two, 1f, 1.3f).setEase (LeanTweenType.easeInCubic);
		LeanTween.scale (Spirit1BG_twoTrans, postscale, 1f).setEase (LeanTweenType.easeInOutCubic);
		LeanTween.value(0f,1f,0.5f).setOnComplete(() => {
			LeanTween.alphaCanvas(Spirit1_pic, 1f, 1.2f).setEase(LeanTweenType.easeInCubic);
			LeanTween.scale (Spirit1_PicTrans, postscale, 1.2f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => {
				LeanTween.scale (Spirit1BG_oneTrans, prescale, 5f).setEase (LeanTweenType.easeInCubic);
				LeanTween.scale (Spirit1BG_twoTrans, prescale, 5f).setEase (LeanTweenType.easeInCubic);
				LeanTween.scale (Spirit1_PicTrans, idlescale, 5f).setEase(LeanTweenType.easeInCubic);
			});
		});
		LeanTween.value (0f, 1f, 5f).setOnComplete (() => {
			LeanTween.alphaCanvas (Spirit1BG_one, 0f, 2f).setEase (LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas (Spirit1BG_two, 0f, 2f).setEase (LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas (Spirit1_pic, 0f, 1.5f).setEase(LeanTweenType.easeOutCubic);
		});
	}
	void AnimateSpiritTwo() {
		LeanTween.alphaCanvas (Spirit2BG_one, 1f, 1f).setEase (LeanTweenType.easeInCubic);
		LeanTween.scale (Spirit2BG_oneTrans, postscale, 1.5f).setEase (LeanTweenType.easeInOutCubic);
		LeanTween.alphaCanvas (Spirit2BG_two, 1f, 1.3f).setEase (LeanTweenType.easeInCubic);
		LeanTween.scale (Spirit2BG_twoTrans, postscale, 1f).setEase (LeanTweenType.easeInOutCubic);
		LeanTween.value(0f,1f,0.5f).setOnComplete(() => {
			LeanTween.alphaCanvas(Spirit2_pic, 1f, 1.2f).setEase(LeanTweenType.easeInCubic);
			LeanTween.scale (Spirit2_picTrans, postscale, 1.2f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => {
				LeanTween.scale (Spirit2BG_oneTrans, prescale, 5f).setEase (LeanTweenType.easeInCubic);
				LeanTween.scale (Spirit2BG_twoTrans, prescale, 5f).setEase (LeanTweenType.easeInCubic);
				LeanTween.scale (Spirit2_picTrans, idlescale, 5f).setEase(LeanTweenType.easeInCubic);
			});
		});
		LeanTween.value (0f, 1f, 5f).setOnComplete (() => {
			LeanTween.alphaCanvas (Spirit2BG_one, 0f, 2f).setEase (LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas (Spirit2BG_two, 0f, 2f).setEase (LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas (Spirit2_pic, 0f, 1.5f).setEase(LeanTweenType.easeOutCubic);
		});
	}
	public void closeCredits() {
		CloseCredits.interactable = false;
		LeanTween.pauseAll ();
		LeanTween.alphaCanvas (CreditsP_CG, 0f, 1f).setEase(LeanTweenType.easeOutCubic).setOnComplete (() => Destroy (gameObject));
	}
}