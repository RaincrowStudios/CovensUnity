using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellCastUIManager : UIAnimationManager
{
	public static SpellCastUIManager Instance { get; set;}
	public static int SpellAccuracy = 0;
	public static bool isSpellSelected = false;
	public static bool isSignatureUnlocked = false;
	public static bool isImmune = false;
	public static bool isDead = false;

	public GameObject close;
	public GameObject spellCastBG;
	public GameObject conditionsTarget;
	public GameObject conditionsSelf;
	public GameObject Container;
	public GameObject SpellCanvas;
	public GameObject ingredientsObject;
	public GameObject listIngredientContainer;
	public Image spellGlyph;
	public GameObject spellGestureObject;
	public GameObject ingredientInfo;
	public Animator spellBackAnim;
	public Transform SpellMask;
	public SpellTraceManager STM;

	[Header("Added Ingredients")]
	public GameObject ingredientsAdded;
	public Text ingredientsAddedText;

	[Header("Conditions")]
	public Transform playerConditionContainer;
	public Transform enemyConditionContainer;
	public GameObject conditionsTitle;
	public GameObject conditionsTitleEnemy;
	public GameObject conditionsPrefab;

	public SignatureScrollManager SSM;
	public GameObject spellTitle;

	public GameObject loadingFX;
	public ParticleSystem prominence;
	public ParticleSystem rays;

	public Text PlayerImmune;
	public GameObject[]spelldescItems;

	void Awake ()
	{
		Instance = this;
	}
	public void Exit()
	{
		Hide (Container);
		Disable (SpellCanvas, 1.4f);
		MapSelection.Instance.GoBack ();
		if(!isImmune)
		SpellCarouselManager.Instance.Hide();
		SetTracing( false);
		StartCoroutine (FadeOut (ingredientsObject, 2));
		isSpellSelected = false;
		if (isImmune)
			HitFXManager.Instance.SetImmune (false);
		if (isDead)
			HitFXManager.Instance.TargetRevive (true);
	}
	
	public void Initialize()
	{
		isSpellSelected = false;
		SpellMask.localScale = Vector3.zero;
		SpellCanvas.SetActive (true);
		Show (Container);
		if (!isImmune) {
			SpellCarouselManager.Instance.Show ();
		} else {
			HitFXManager.Instance.SetImmune (true);
		}
		SetTracing( false);
		StartCoroutine (FadeOut (ingredientsObject, 2));
		if (MarkerSpawner.SelectedMarker.state == "dead") {
			HitFXManager.Instance.TargetDead ();
		}
	}

	public void SelectSpell( )
	{
		isSpellSelected = true;
		SpellCarouselManager.Instance.Hide();
		spellGlyph.sprite = DownloadedAssets.getGlyph (SpellCarouselManager.currentSpellData.id);
		Show (spellGlyph.gameObject);
		StartCoroutine (FadeIn (ingredientsObject, 2));
		Show (spellCastBG);
		spellBackAnim.SetBool ("animate", true);
		spellGestureObject.SetActive (true);
		SetTracing( true);
		SetupSignature (); 
	}

	public void SetupSignature ()
	{
		List<Signature> sigs = new List<Signature> ();
		foreach (var item in PlayerDataManager.playerData.signatures) {
			if (item.baseSpell == SpellCarouselManager.currentSpellData.id) {
				sigs.Add (item);
			}
		}
		if (sigs.Count > 0) {
			isSignatureUnlocked = true;
			Signature sg = new Signature ();
			sg.id = sigs [0].baseSpell;
			sg.baseSpell = sg.id;
			sigs.Insert (sigs.Count / 2, sg);
			SSM.gameObject.SetActive (true);
			spellTitle.SetActive (false);
			SSM.Initiate (sigs, (sigs.Count - 1) / 2);
		} else {
			isSignatureUnlocked = false;
		}
	}

	public void SpellClose()
	{
		SignatureScrollManager.currentSignature = null;
		isSpellSelected = false;
		spellBackAnim.SetBool ("animate", false);
		spellGestureObject.SetActive (false);
		Hide (spellCastBG);
		Hide (spellGlyph.gameObject);
		SpellCarouselManager.Instance.Show();
		StartCoroutine (FadeOut (ingredientsObject, 2));
		SetTracing( false);
		if (SSM.gameObject.activeInHierarchy) {
			SSM.gameObject.SetActive (false);
			spellTitle.SetActive (true);
		}
	}

	public void SetTracing(bool canTrace)
	{
		STM.enabled = canTrace;
	}

	public void Immune(bool immune){
		isImmune = immune;
		if (isImmune) {
			SpellCarouselManager.Instance.Hide ();
			PlayerImmune.text = MarkerSpawner.SelectedMarker.displayName + " is immune to you.";
			PlayerImmune.gameObject.SetActive (true);
			foreach (var item in spelldescItems) {
				item.SetActive (false);
			}
		} else {
			SpellCarouselManager.Instance.Show ();
			foreach (var item in spelldescItems) {
				item.SetActive (true);
			}
			PlayerImmune.gameObject.SetActive (false);
		}
	}

}

