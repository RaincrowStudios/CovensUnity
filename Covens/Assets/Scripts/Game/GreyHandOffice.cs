using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GreyHandOffice : MonoBehaviour {

	public GameObject NotToday;
	
	public TextMeshProUGUI greyHandOffice;
	public TextMeshProUGUI toolNum;
	public TextMeshProUGUI drachNum;

	public TextMeshProUGUI rewardDrachs;

	public CanvasGroup SavCG;
	public GameObject Sav;

	public CanvasGroup whole;

	public CanvasGroup BGCG;

	public CanvasGroup ToolsCG;

	public CanvasGroup DrachsCG;

	public CanvasGroup TextContainer;

	public CanvasGroup WarningTextCont;
	public CanvasGroup WarningBG;

	public GameObject accept;

	public Vector3 SavScale;

	private int forbiddenCount = 0;


	// Use this for initialization
	void Start () {
		for (int i = 0; i < PlayerDataManager.playerData.ingredients.tools.Count; i++) {
			if (PlayerDataManager.playerData.ingredients.tools [i].forbidden) {
				forbiddenCount += PlayerDataManager.playerData.ingredients.tools [i].count;
			}
		}
		toolNum.text = forbiddenCount.ToString ();
		//for now this is hard coded, but will change later
		drachNum.text = (forbiddenCount * 10).ToString();

		if (forbiddenCount < 1) {
			transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI> ().color = Color.red;
			transform.GetChild(2).GetChild(4).GetComponent<Button> ().enabled = false;
		} else {
			transform.GetChild(2).GetChild(4).GetComponent<Button> ().onClick.AddListener (() => {
				print("clicking this button");
				APIManager.Instance.GetData("vendor/give", TurnInCallback);
			});
		}

		transform.GetChild (2).GetChild (5).GetComponent<Button> ().onClick.AddListener (Warning);

		transform.GetChild (4).GetComponent<Button> ().onClick.AddListener (() => {
			LeanTween.alphaCanvas(whole, 0f,  1f).setEase(LeanTweenType.easeOutCubic).setOnComplete (() => {
				Destroy(this.gameObject);
			});

		});

		InitAnims ();
	}

	public void TurnInCallback(string result, int code)
	{
		if (code == 200) {
			print ("success");
			print (result);
		} else {
			print ("error code: " + code);
			print (result);
		}
	}

	void InitAnims() {
		NotToday.SetActive (false);
		accept.SetActive (false);
		WarningBG.alpha = 0;
		SavScale = Vector3.one * 0.3f;
		BGCG.alpha = 0;
		WarningTextCont.alpha = 0;
		SavCG.alpha = 0;
		ToolsCG.alpha = 0;
		DrachsCG.alpha = 0;
		TextContainer.alpha = 0;
		LeanTween.alphaCanvas (BGCG, 1f, 0.5f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (TextContainer, 1f, 2f).setEase (LeanTweenType.easeOutCubic);
		Invoke ("Anim", 1f);
	}
	// Update is called once per frame
	void Anim () {
		LeanTween.scale (Sav, Vector3.one*0.7f, 7f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (SavCG, 1f, 1.5f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			Anim2();
		});
		//callOnCompletes(Anim2());
		//Invoke ("Anim2", 1f);
	}
	void Anim2 () {
		LeanTween.alphaCanvas (ToolsCG, 1f, 1f).setEase(LeanTweenType.easeOutCubic).setOnComplete  (() => {
			LeanTween.alphaCanvas (DrachsCG, 1f, 1f).setEase (LeanTweenType.easeOutCubic);
		});
		//	.setEase (LeanTweenType.easeOutCubic);

	}
	public void Warning () {
		LeanTween.alphaCanvas (TextContainer, 0f, 1f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			NotToday.SetActive (true);
			LeanTween.alphaCanvas(WarningBG, 1f, 1.5f).setEase(LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas(SavCG, 0f, 1f).setEase(LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas(WarningTextCont, 1f, 2f).setEase(LeanTweenType.easeOutCubic);
			transform.GetChild (4).gameObject.SetActive (true);
		});
	}
}
