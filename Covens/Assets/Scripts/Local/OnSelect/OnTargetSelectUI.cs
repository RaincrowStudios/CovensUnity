using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTargetSelectUI : MonoBehaviour {
	
	public static OnTargetSelectUI Instance {get; set;}

	public Text playerName;
	public Text cardInfo;
	public Text alignment;
	public Text coven;
	public Text homeDominion;
	public Text worldRank;
	public Text covenStatus;
	public Text favSpell;
	public Text Achievement;

	public Sprite whiteCard;
	public Sprite ShadowCard;
	public Sprite greyCard;

	public Image card;

	public Material blueEmber;
	public Material yellowEmber;
	public Material purpleEmber;
	public Material blueFire;
	public Material yellowFire;
	public Material purpleFire;

	public Color Orange;
	public Color blue;
	public Color purple;

	public Light pointLight;
	public Light pointLight2;

	public GameObject fire;
	public GameObject embers;

	public CanvasGroup CG;

	public ParticleSystem PS;

	void Awake(){
		Instance = this;
	}

	public void OnBack()
	{
		StartCoroutine (FadeOut ());
		OnPlayerSelect.Instance.onDeselectTargetPlayer ();
	}


	public void Onclick (MarkerDataDetail MD)
	{
		CG.gameObject.SetActive (true);
		playerName.text = MD.displayName;
		alignment.text = Utilities.witchTypeControl (MD.degree);
		coven.text = "Coven : " + MD.coven;
		homeDominion.text = "Home dominion : " +MD.dominion;
		worldRank.text ="World rank : " + MD.worldRank.ToString();
		covenStatus.text="Coven status : " + MD.covenStatus;
		favSpell.text = "Favorite spell : " +MD.favoriteSpell;
		Achievement.text = "Achievement : " +"Not much here..";

		var module = PS.main;

		if (MD.degree > 0) {
			cardInfo.text = Constants.whiteCard;
			card.sprite = whiteCard;
			module.startColor = Orange;
			pointLight2.color = Orange;
			pointLight.color = Orange;
			fire.GetComponent<Renderer> ().material = yellowFire;
			embers.GetComponent<Renderer> ().material = yellowEmber;

		} else if (MD.degree < 0) {
			cardInfo.text = Constants.shadowCard;
			card.sprite = ShadowCard;
			pointLight2.color = purple;
			module.startColor = purple;
			pointLight.color = purple;
			fire.GetComponent<Renderer> ().material = purpleFire;
			embers.GetComponent<Renderer> ().material = purpleEmber;
		} else {
			cardInfo.text = Constants.greyCard;
			pointLight2.color = blue;
			module.startColor = blue;
			card.sprite = greyCard;
			pointLight.color = blue;
			fire.GetComponent<Renderer> ().material = blueFire;
			embers.GetComponent<Renderer> ().material = blueEmber;
		}
		StartCoroutine (FadeIn ());
	}

	IEnumerator FadeIn ()
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime;
			FadeController (t);
			yield return null;
		}
	}

	IEnumerator FadeOut ()
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime;
			FadeController (t);
			yield return null;
		}
		CG.gameObject.SetActive (false);
	}

	void FadeController (float t)
	{
		CG.alpha = Mathf.SmoothStep (0, 1f, t);
	}
	
}
