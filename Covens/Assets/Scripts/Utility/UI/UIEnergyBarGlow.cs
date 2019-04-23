using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyBarGlow : MonoBehaviour {
	public static UIEnergyBarGlow Instance{ get; set;}
	// Use this for initialization
	void Start () {
		Instance = this;
	}
	public void Glow () {
		var y = this.GetComponent<CanvasGroup> ();
		y.alpha = 0;
		LeanTween.alphaCanvas (y, 1f, 0.4f).setEase (LeanTweenType.easeInCubic).setOnComplete (() => Countup ());
	}
	
	// Update is called once per frame
	void Countup () {
		var y = this.GetComponent<CanvasGroup> ();
		LeanTween.value (0f, 1f, 0.8f).setOnComplete (() => LeanTween.alphaCanvas (y, 0f, 0.5f).setEase (LeanTweenType.easeOutCubic));//.setOnComplete(() => gameObject.SetActive(false));
	}
}
