using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class NewStoreUIorry : MonoBehaviour {

	public GameObject wheelContainer;
	public GameObject fortuna;
	public CanvasGroup fortunaCG;

	public GameObject wheelGear;
	public GameObject wheelCharms;
	public GameObject wheelIngredients;
	public GameObject wheelSilver;
	public GameObject wheelBG;
	public CanvasGroup wheelBGCG;
	public CanvasGroup wheelGearCG;
	public CanvasGroup wheelCharmsCG;
	public CanvasGroup wheelIngredientsCG;
	public CanvasGroup wheelSilverCG;

	public float moveFortuna;
	public float inTime;

	public void Start() {
		WheelSpinIn();
		FortunaSlideIn();
	}
	public void WheelSpinIn()
	{
		LeanTween.rotateZ (wheelBG, 0f, inTime).setEase(LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (wheelBGCG, 1f, inTime*0.8f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ(wheelContainer, 0f, inTime).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (wheelGearCG, 1f, inTime*0.4f);
		LeanTween.alphaCanvas (wheelSilverCG, 1f, inTime*0.6f);
		LeanTween.alphaCanvas (wheelCharmsCG, 1f, inTime*0.8f);
		LeanTween.alphaCanvas (wheelIngredientsCG, 1f, inTime*1f);
		LeanTween.rotateZ (wheelGear, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ (wheelSilver, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ (wheelCharms, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ (wheelIngredients, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
	}
	public void FortunaSlideIn()
	{
		LeanTween.moveLocalX(fortuna, moveFortuna, inTime).setEase(LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas(fortunaCG, 1f, inTime).setEase(LeanTweenType.easeOutCubic);
	}

	//not sure if this is right yet
	private void DestroyStoreInstance()
	{
		Destroy (this.gameObject);
	}
}
