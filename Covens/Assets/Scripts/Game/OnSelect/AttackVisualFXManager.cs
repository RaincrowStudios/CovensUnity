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
	public CanvasGroup playerEnergyCG;
	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
		lightIntensities = new Vector3(witchLights[0].intensity,witchLights[1].intensity,witchLights[2].intensity);
		playerEnergyCG = playerEnergy.GetComponent<CanvasGroup> ();
	}

	public void AttackTest()
	{
		AttackData ad = new AttackData ();
		ad.crit = false;
		ad.damage = 30;
		ad.currentEnergy = int.Parse (targetHealth.text);
		ad.success = true;
		ad.xp = 45;
		ad.type = InteractionType.Hit;
		XP.gameObject.SetActive (false);
		targetDamage.gameObject.SetActive (false);
		targetResistedDamage.gameObject.SetActive (false);
		Attack (ad);
		STM.enabled = false;
	}

	public void Attack( AttackData data )
	{
		if (data.type == InteractionType.AttackCrit) {
			targetDamage.gameObject.SetActive (true);
			targetDamage.text = data.damage.ToString ();
			targetDamage.transform.localScale = Vector3.one * 1.5f;
			targetDamage.color = Utilities.Orange;
			XP.gameObject.SetActive (true);
			XP.text = data.xp.ToString () + "XP";
			var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform.GetChild (2), 1.4f);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item));
			}
			StartCoroutine (EnergyCounter (data, targetHealth));
		} else if (data.type == InteractionType.AttackNormal) {
			targetDamage.gameObject.SetActive (true);
			targetDamage.text = data.damage.ToString ();
			targetDamage.transform.localScale = Vector3.one;
			targetDamage.color = Color.white;
			XP.gameObject.SetActive (true);
			XP.text = data.xp.ToString () + "XP";
			var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform.GetChild (2), 1.4f);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item));
			}
			StartCoroutine (EnergyCounter (data, targetHealth));
			centerSpellTrigger.SetActive (false);

		} else if (data.type == InteractionType.AttackResist) {
			targetDamage.gameObject.SetActive (true);
			targetDamage.text = data.damage.ToString ();
			targetDamage.transform.localScale = Vector3.one;
			targetDamage.color = Color.white;
			targetResistedDamage.gameObject.SetActive (true);
			targetResistedDamage.text = data.resist.ToString () + " Resisted";
			XP.gameObject.SetActive (true);
			XP.text = data.xp.ToString () + "XP";
			var g = Utilities.InstantiateObject (attackFX [Random.Range (0, attackFX.Length)], OnPlayerSelect.SelectedPlayerTransform, 1.52f);
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item));
			}  
			StartCoroutine (EnergyCounter (data, targetHealth)); 
		} else if (data.type == InteractionType.Hit) {
			SpellSelectParent.Instance.sp.HideGlow ();
			StartCoroutine (GotHit (data));
			foreach (var item in fadeItems) {
				StartCoroutine (FadeOut (item,3));
			}
		}
	}


	IEnumerator GotHit(AttackData data)
	{
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

		selftDamage.gameObject.SetActive (true);
		selftDamage.text = data.damage.ToString ();
		selftDamage.transform.localScale = Vector3.one;
		selftDamage.color = Color.white;
		XP.gameObject.SetActive (true);
		XP.text = data.xp.ToString () + "XP";
		witchHitFx.SetActive (true);
		StartCoroutine (EnergyCounter (data, playerEnergy));
		StartCoroutine (EnergyScale (playerEnergy.transform));
		StartCoroutine (GotHitRevert (data));
	}


	IEnumerator GotHitRevert(AttackData data)
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
		yield return new WaitForSeconds (2);
		SpellSelectParent.Instance.sp.showGlow ();
	}

	IEnumerator EnergyCounter(AttackData data, Text energyText)
	{
		float t = 0;
		int finalEnergy = data.currentEnergy + data.damage;
		StartCoroutine (EnergyScale (energyText.transform));
		while (t <= 1f) {
			t += Time.deltaTime * enChangeSpeed;
			int energy = (int)Mathf.Lerp (data.currentEnergy, finalEnergy, t);
			energyText.text = energy.ToString ();
			yield return null;
		}
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
			k -= Time.deltaTime * enScaleSpeed*.25f;
			text.localScale = Vector3.Lerp (Vector3.one, finalS, k);
			yield return null;
		}
	}

	IEnumerator FadeOut(CanvasGroup CG, float fadeTimer = .8f)
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * speed;
			CG.alpha = Mathf.SmoothStep (0, 1f, t);
			yield return null;
		}
		SpellSelectParent.Instance.sp.Reset ();
		if (CG.name != "FadeBlack") {
			yield return new WaitForSeconds (fadeTimer);
			StartCoroutine (FadeIn (CG));
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
}

