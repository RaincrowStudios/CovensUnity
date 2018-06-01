using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


public class SpellSelectParent : MonoBehaviour
{
	public static SpellSelectParent Instance { get; set;}
	public static Spells currentSpellEnum;
	[HideInInspector]
	public SpellSelect sp;
	public float speed = 1;
	public SpellTraceManager STM;
	public CanvasGroup[] spells; 
	public CanvasGroup BlackBG;
	public GameObject closePlayer;
	public GameObject closeSpell;
	public CanvasGroup[] SpellCarouselItems;
	public Transform Container;
	public Transform fxItems;
	ScrollRect SR;

	public GameObject gemPS;
	public GameObject herbPS;
	public GameObject toolPS;

	public Text herbCount;
	public Text gemCount; 

	public Text toolCount;

	public GameObject AddedIngredients;
	public GameObject GestureRecognizer;
	public GameObject channelingRecognizer;

	public Transform containerConditions;
	public GameObject conditionObject;
	Dictionary<string,ConditionButtonData> conitionDict = new Dictionary<string, ConditionButtonData>();

	void Awake()
	{
		Instance = this;
		SR = GetComponent<ScrollRect> ();
	}

	void OnEnable()
	{
		GestureRecognizer.SetActive (false);
		BlackBG.alpha = 0;
		GetComponent<CanvasGroup> ().alpha = 1;
		GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -171);
		GetComponent<ScrollRect> ().horizontal = true;
		closePlayer.SetActive (true);
		closeSpell.SetActive (false);
		foreach (Transform item in fxItems) {
			item.gameObject.SetActive (false);
		}
		DisableGestureRecog ();
	}
		
	public void Revert()
	{
		Container = GetComponent<ScrollRect>().content.transform;
		ManageScroll (false);
		sp.onRevert ();
		SR.horizontal = true;
		closePlayer.SetActive (true);
		closeSpell.SetActive (false);
		StartCoroutine (FadeIn (Container.GetChild(1).GetComponent<CanvasGroup>())); 
		StartCoroutine (FadeIn (Container.GetChild(2).GetComponent<CanvasGroup>()));
		StartCoroutine (FadeIn (Container.GetChild(4).GetComponent<CanvasGroup>())); 
		StartCoroutine (FadeIn (Container.GetChild(5).GetComponent<CanvasGroup>()));
		FadeOutBG ();
		sp.showGlow ();
		DisableGestureRecog ();
		EventManager.Instance.CallCastingStateChange (SpellCastStates.selection);
	}

	public void DisableGestureRecog()
	{
		GestureRecognizer.SetActive (false);
		channelingRecognizer.SetActive(false);
	}

	public void OnClick()
	{
		Container = GetComponent<ScrollRect>().content.transform;
		STM.enabled = true;
		ManageScroll (true);
		print (Container.childCount);
		StartCoroutine (FadeOut (Container.GetChild(1).GetComponent<CanvasGroup>()));
		StartCoroutine (FadeOut (Container.GetChild(2).GetComponent<CanvasGroup>()));
		StartCoroutine (FadeOut (Container.GetChild(4).GetComponent<CanvasGroup>()));
		StartCoroutine (FadeOut (Container.GetChild(5).GetComponent<CanvasGroup>()));
		closePlayer.SetActive (false); 
		closeSpell.SetActive (true); 
		FadeInBG ();
		currentSpellEnum =  (Spells)Enum.Parse (typeof(Spells), SpellCarousel.currentSpell); 
		if (currentSpellEnum != Spells.spell_whiteFlame && currentSpellEnum != Spells.spell_sunEater) {
			GestureRecognizer.SetActive (true);
//			SpellGestureManager.Instance.SetGestureLibrary (currentSpellEnum);
		} else {
			channelingRecognizer.SetActive(true);

		}
	}

	public void ManageScroll(bool state)
	{
		SR.horizontal = state;

	}

	IEnumerator FadeOut(CanvasGroup CG)
	{
		if (CG.alpha == 0) {
			print (CG.name + " 0 alpha");
			yield break;
		}
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}

	}

	IEnumerator FadeIn( CanvasGroup CG)
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
	}

	public void CarouselFadeOut()
	{
		foreach (var item in SpellCarouselItems) {
			StartCoroutine( FadeOut (item));
		}
	}

	public void CarouselFadeIn()
	{
		AddedIngredients.SetActive (true);

		if (IngredientsManager.herbCount > 0) {
			herbPS.SetActive (true);
			herbCount.text = IngredientsManager.herbCount.ToString ();
		} else
			herbPS.SetActive (false);

		if (IngredientsManager.gemCount > 0) {
			gemPS.SetActive (true);
			gemCount.text = IngredientsManager.gemCount.ToString ();
		} else
			gemPS.SetActive (false);


		if (IngredientsManager.toolCount > 0) {
			toolPS.SetActive (true);
			toolCount.text = IngredientsManager.toolCount.ToString ();
		} else
			toolPS.SetActive (false);

		foreach (var item in SpellCarouselItems) {
			StartCoroutine( FadeIn (item));
		}
		
	}

	public void FadeInBG()
	{
		StartCoroutine( FadeIn(BlackBG));
	}

	public void FadeOutBG()
	{
		StartCoroutine( FadeOut(BlackBG));
	}

	public void ShowIngredients()
	{
		sp.onClickIngredientSpell ();
	}


}

public enum SpellCastStates
{
	selection, casting , attack, hit
}

