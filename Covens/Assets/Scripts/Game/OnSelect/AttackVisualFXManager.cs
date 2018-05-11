using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class AttackVisualFXManager : MonoBehaviour
{
	public static AttackVisualFXManager Instance { get; set;}

	

	public Text selftDamage;
	public Text selfResistedDamage;
	
	public Text targetHealth;
	public Text targetDamage;
	public Text XP;
	public Text targetResistedDamage;
	public CanvasGroup[] fadeItems;
	public GameObject[] attackFX;
	public float speed = 1;
	public float enChangeSpeed = 1;
	public float enScaleSpeed = 1;
	public float hitFXSpeed = 1;
	public GameObject centerSpellTrigger;
	public SpellTraceManager STM ;
	public List<Light> witchLights = new List<Light>();
	Vector3 lightIntensities;
	public Light spotLight;
	public Transform playerRune;
	public GameObject witchHitFx;
	public GameObject spiritLightAttack;
	public GameObject spiritLightTrace;
	public Text playerEnergy;
	CanvasGroup playerEnergyCG;
	public GameObject continueButton;
	public GameObject escapeFX;
	public Text escapeText;
	public GameObject blastFX;
	public GameObject banishText;
	public GameObject blastFXSelf;
	public GameObject DeathScreen;
	public InteractionType iType;
	public GameObject AttackFail;
	Vector4 FadeItemsAlpha;
	SpellCastStates currentState; 
	List<WebSocketResponse> hits = new List<WebSocketResponse>();

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		lightIntensities = new Vector3(witchLights[0].intensity,witchLights[1].intensity,witchLights[2].intensity);
		playerEnergyCG = playerEnergy.GetComponent<CanvasGroup> ();
		EventManager.CastingStateChange += ManageHitQueue;
	}

	public void AttackTest()
	{
//		WebSocketResponse ad = new  ();
//		ad.crit = false;
//		ad.damage = -30;
//		ad.currentEnergy = int.Parse (targetHealth.text);
//		ad.success = true;
//		ad.xp = 45;
//		ad.type = iType;
//		XP.gameObject.SetActive (false);
//		targetDamage.gameObject.SetActive (false);
//		targetResistedDamage.gameObject.SetActive (false);
//		Attack (ad);
//		STM.enabled = false;
	}

	public void SpellUnsuccessful()
	{
		AttackFail.SetActive (true);
		STM.enabled = true;
		SpellSpiralLoader.Instance.LoadingDone ();
	}

	public void Attack(WebSocketResponse data)
	{
		StartCoroutine (AttackHelper (data));
	}

	IEnumerator AttackHelper(WebSocketResponse data)
	{
		EventManager.Instance.CallCastingStateChange (SpellCastStates.attack);
		SpellSpiralLoader.Instance.LoadingDone ();
		STM.enabled = false;
		targetDamage.gameObject.SetActive (true);
		targetDamage.fontSize = 100;
		targetDamage.color = Color.white;
		if (data.result.critical) {
			targetDamage.color = Color.red;
			targetDamage.fontSize = 130;
		}
		if (data.result.resist) {
			targetResistedDamage.gameObject.SetActive (true);
			targetResistedDamage.text = "Resisted";
		}
		XP.gameObject.SetActive (true);
		XP.text= ( data.xp - OnPlayerSelect.playerXPTemp ).ToString () + "XP";
		var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform.GetChild (2), 1.4f);
		if (data.targetStatus != "dead") {
			StartCoroutine (EnergyCounter (data, targetHealth));
		} else {
			StartCoroutine (EnergyCounter (data, targetHealth,true));
		}
		foreach (var item in fadeItems) {
			StartCoroutine (_FadeOut (item));
		} 
		yield return new WaitForSeconds (.8f);
		SpellSelectParent.Instance.sp.Reset ();
		foreach (var item in fadeItems) {
			if(item.name!="FadeBlack")
				StartCoroutine (_FadeIn (item));
		} 

		SpellSelectParent.Instance.ManageScroll (true);
		centerSpellTrigger.SetActive (true);
		yield return new WaitForSeconds (1);
		SpellSelectParent.Instance.sp.showGlow ();
		yield return new WaitForSeconds (2);
		XP.gameObject.SetActive (false);
		selfResistedDamage.gameObject.SetActive (false);
		selftDamage.gameObject.SetActive (false);
		targetDamage.gameObject.SetActive (false);
		targetResistedDamage.gameObject.SetActive (false);

		SpellSelectParent.Instance.DisableGestureRecog ();
		EventManager.Instance.CallCastingStateChange (SpellCastStates.selection);

	}

	IEnumerator EnergyCounter(WebSocketResponse data, Text energyText, bool isDead = false)
	{
		float t = 0;
		StartCoroutine (EnergyScale (energyText.transform));
		while (t <= 1f) {
			t += Time.deltaTime * enChangeSpeed;
			int energy = (int)Mathf.Lerp (MarkerSpawner.SelectedMarker.energy, data.targetEnergy, t); 
			energyText.text = energy.ToString ();
			yield return null;
		}
		MarkerSpawner.SelectedMarker.energy = data.targetEnergy;
		if(isDead)
			StartCoroutine (ShowBlast ());
	}

	IEnumerator EnergyScale(Transform text)
	{
		float t = 0;
		var finalS = Vector3.one * 1.4f;
		while (t <= 1f) {
			t += Time.deltaTime * enScaleSpeed;
			text.localScale = Vector3.Lerp (Vector3.one, finalS, t);
			yield return null;
		}

		float k = 1;
		while (k >= 0) {
			k -= Time.deltaTime * enScaleSpeed*.8f;
			text.localScale = Vector3.Lerp (Vector3.one, finalS, k);
			yield return null;
		}
	}

	IEnumerator ShowBlast()
	{
		blastFX.SetActive (true);
		StartCoroutine (EscapeScaleDown (OnPlayerSelect.SelectedPlayerTransform));
		yield return new WaitForSeconds (1.8f);
		OnPlayerSelect.Instance.GoBack ();
		yield return new WaitForSeconds (1f);
		banishText.SetActive (true);
	}

	public void AddHitQueue(WebSocketResponse data)
	{
		if (currentState != SpellCastStates.selection) {
			hits.Add (data);
		} else {
			performHit (data);
		}
	}

	void ManageHitQueue(SpellCastStates state)
	{
		currentState = state;
		if (OnPlayerSelect.currentView != CurrentView.IsoView) {
			hits.Clear ();
			return;
		}
		if (state == SpellCastStates.selection) {
			if (hits.Count > 0) {
				performHit (hits[0]);
				hits.RemoveAt (0);
				EventManager.Instance.CallCastingStateChange (SpellCastStates.hit); 
			}
		}
	}

	void performHit(WebSocketResponse data)
	{
		print ("performing hit");
		StartCoroutine (GotHit (data));
	}

	IEnumerator GotHit(WebSocketResponse data)
	{
		SpellSelectParent.Instance.sp.HideGlow ();
		foreach (var item in fadeItems) {
			if(item.name != "FadeBlack")
				StartCoroutine (_FadeOut(item));
		}
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * hitFXSpeed;
			witchLights[0].intensity = Mathf.SmoothStep (lightIntensities.x,0,t);
			witchLights[1].intensity = Mathf.SmoothStep (lightIntensities.y,0,t);
			witchLights[2].intensity = Mathf.SmoothStep (lightIntensities.z,0,t);
			playerEnergyCG.alpha = Mathf.SmoothStep (1, 0, t);
			spotLight.intensity = Mathf.SmoothStep (2, 1.24f, t);
			spotLight.spotAngle = Mathf.SmoothStep (116, 61, t);
			playerRune.transform.localScale = Vector3.one * Mathf.SmoothStep(1,0,t);
			yield return null;
		}
		spiritLightTrace.SetActive (true);
		yield return new WaitForSeconds (.8f);
		spiritLightAttack.SetActive (true);
		yield return new WaitForSeconds (1);

		print ("Turning on Damage");
		selftDamage.gameObject.SetActive (true);
		selftDamage.fontSize = 100;
		selftDamage.color = Color.white;
		if (data.result.critical) {
			selftDamage.color = Color.red;
			selftDamage.fontSize = 130;
		}
		if (data.result.resist) {
			selfResistedDamage.gameObject.SetActive (true);
			selfResistedDamage.text = "Resisted";
		}

		selftDamage.gameObject.SetActive (true);
		selftDamage.transform.localScale = Vector3.one;
		selftDamage.color = Color.white;
		witchHitFx.SetActive (true);
		StartCoroutine (EnergyCounterSelf (data, playerEnergy));
		StartCoroutine (EnergyScale (playerEnergy.transform));
		StartCoroutine (GotHitRevert (data));
	}

	IEnumerator GotHitRevert(WebSocketResponse data)
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * hitFXSpeed;
			playerEnergyCG.alpha = Mathf.SmoothStep (1, 0, t);
			witchLights[0].intensity = Mathf.SmoothStep (lightIntensities.x,0,t);
			witchLights[1].intensity = Mathf.SmoothStep (lightIntensities.y,0,t);
			witchLights[2].intensity = Mathf.SmoothStep (lightIntensities.z,0,t);
			spotLight.intensity = Mathf.SmoothStep (2, 1.24f, t);
			spotLight.spotAngle = Mathf.SmoothStep (116, 61, t);
			playerRune.transform.localScale = Vector3.one * Mathf.SmoothStep(1,0,t);
			yield return null;
		}
		foreach (var item in fadeItems) {
			if(item.name != "FadeBlack")
			StartCoroutine (_FadeIn(item));
		}
			yield return new WaitForSeconds (1);
			SpellSelectParent.Instance.sp.showGlow ();
		SpellSelectParent.Instance.DisableGestureRecog ();
		yield return new WaitForSeconds (2);
		selfResistedDamage.gameObject.SetActive (false);
		selftDamage.gameObject.SetActive (false);
		targetDamage.gameObject.SetActive (false);
		targetResistedDamage.gameObject.SetActive (false);
		yield return new WaitForSeconds (2);
		EventManager.Instance.CallCastingStateChange (SpellCastStates.selection);
	
	}

	IEnumerator EnergyCounterSelf(WebSocketResponse data, Text energyText, bool isDead = false)
	{
		float t = 0;
		StartCoroutine (EnergyScale (energyText.transform));
		while (t <= 1f) {
			t += Time.deltaTime * enChangeSpeed;
			int energy = (int)Mathf.Lerp (OnPlayerSelect.playerEnergyTemp, data.energy, t); 
			energyText.text = energy.ToString ();
			yield return null;
		}
		if(isDead)
			StartCoroutine (ShowBlast ());
	}


	IEnumerator _FadeOut(CanvasGroup CG)
	{
		if (CG.alpha == 0)
			yield break;
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
	}

	IEnumerator _FadeIn( CanvasGroup CG)
	{
		if (CG.alpha == 1)
			yield break;
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
	}
	#region old
	public void AttackOld( WebSocketResponse data )
	{
		print ("Attacked");
		STM.enabled = false;
		if (data.iType == InteractionType.AttackCrit) { 
			targetDamage.gameObject.SetActive (true);
			targetDamage.text = (MarkerSpawner.SelectedMarker.energy - data.targetEnergy).ToString();
			targetDamage.transform.localScale = Vector3.one * 1.5f;
			targetDamage.color = Utilities.Orange;
			XP.gameObject.SetActive (true);
			XP.text = ( data.xp - PlayerDataManager.playerData.xp ).ToString () + "XP";
			var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform.GetChild (2), 1.4f);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item));
			}
			StartCoroutine (EnergyCounter (data, targetHealth));
		} else if (data.iType == InteractionType.AttackNormal) {
			targetDamage.gameObject.SetActive (true);
			targetDamage.text =  (MarkerSpawner.SelectedMarker.energy - data.targetEnergy).ToString();  
			targetDamage.transform.localScale = Vector3.one;
			targetDamage.color = Color.white;
			XP.gameObject.SetActive (true);
			XP.text = ( data.xp - PlayerDataManager.playerData.xp ).ToString () + "XP";
			var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform.GetChild (2), 1.4f);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item));
			}
			StartCoroutine (EnergyCounter (data, targetHealth));
			centerSpellTrigger.SetActive (false);

		} else if (data.iType == InteractionType.AttackResist) {
			targetDamage.gameObject.SetActive (true);
			targetDamage.text = (MarkerSpawner.SelectedMarker.energy - data.targetEnergy).ToString();
			targetDamage.transform.localScale = Vector3.one;
			targetDamage.color = Color.white;
			targetResistedDamage.gameObject.SetActive (true);
			targetResistedDamage.text = "? "  + " Resisted";
			XP.gameObject.SetActive (true);
			XP.text = data.xp.ToString () + "XP";
			var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform, 1.52f);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item));
			}  
			StartCoroutine (EnergyCounter (data, targetHealth)); 
		} else if (data.iType == InteractionType.Hit) {
			SpellSelectParent.Instance.sp.HideGlow ();
			StartCoroutine (GotHit (data));
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item, 3));
			}
		} else if (data.iType == InteractionType.TargetEscape) {
			SpellSelectParent.Instance.sp.HideGlow ();
			escapeFX.SetActive (true);
			continueButton.SetActive (true);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item, .8f, false));
			}
			Invoke ("showEscapeText", 1f);
			StartCoroutine (EscapeScaleDown (OnPlayerSelect.SelectedPlayerTransform));
		} else if (data.iType == InteractionType.TargetDied) {
			SpellSelectParent.Instance.sp.HideGlow ();
			//			data.damage = -data.currentEnergy; 
			targetDamage.gameObject.SetActive (true); 
			//			targetDamage.text = data.damage.ToString (); 
			targetDamage.transform.localScale = Vector3.one;
			targetDamage.color = Color.white;
			XP.gameObject.SetActive (true);
			XP.text = data.xp.ToString () + "XP";
			var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform.GetChild (2), 1.4f);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item, .8f, false));
			}
			StartCoroutine (EnergyCounter (data, targetHealth, true));
			centerSpellTrigger.SetActive (false);
		} else if (data.iType == InteractionType.Death) {
			SpellSelectParent.Instance.sp.HideGlow ();
			StartCoroutine (GotHit (data));
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item, 3,false));
			}
			StartCoroutine (ShowDeathFX ());
		}
	}

	IEnumerator ShowDeathFX()
	{
		yield return new WaitForSeconds (3.87f);
		StartCoroutine (EscapeScaleDown (OnPlayerSelect.Instance.yourWitch.transform,false));
		blastFXSelf.SetActive (true);
		yield return new WaitForSeconds (1.3f);
		DeathScreen.SetActive (true);
	}


	void showEscapeText()
	{
		escapeText.text = MarkerSpawner.instanceID + " has escaped.";
		escapeText.gameObject.SetActive (true);
	}

	IEnumerator EscapeScaleDown(Transform token, bool isRot = true)
	{
		OnPlayerSelect.hasEscaped = true;
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * speed;
			token.localScale = Vector3.one*44* Mathf.SmoothStep (0, 1f, t);
			if(isRot)
				token.localEulerAngles =   new Vector3 (0,  Mathf.SmoothStep (271f, 0f, t),0);
			yield return null;
		}
	}


	IEnumerator FadeOut(CanvasGroup CG, float fadeTimer = .8f, bool isFadeIn = true)
	{
		if (CG.alpha == 0)
			yield return null;
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
		if (isFadeIn){
			SpellSelectParent.Instance.sp.Reset ();
			if (CG.name != "FadeBlack") {
				yield return new WaitForSeconds (fadeTimer);
				StartCoroutine (FadeIn (CG));
			}
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
		SpellSelectParent.Instance.ManageScroll (true);
		centerSpellTrigger.SetActive (true);
		yield return new WaitForSeconds (3);
		XP.gameObject.SetActive (false);
		selfResistedDamage.gameObject.SetActive (false);
		selftDamage.gameObject.SetActive (false);
		targetDamage.gameObject.SetActive (false);
		targetResistedDamage.gameObject.SetActive (false);
	}
	#endregion


}

