using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaCharacterManager : MonoBehaviour {
	public Transform damageSpawn;
	public GameObject Damage;
	public GameObject DamageGlow;
	public Transform targetPlayer;

	public GameObject ArcaneRune;
	Animator Anim;
	public float lookSpeed = 1f;
	float t ;
	bool look ;
	public Animator CanvasAnim;
	public enum SpellType {hex,bless,curse,sunEater,whiteFlame,jinx};
	public Transform EffectSpawnPoint;
	public Transform EffectSpawnPoint_RFX4;

	public GameObject[] attack;
	public GameObject attack1Hand;
	void OnEnable () {
		Anim = GetComponent<Animator> ();
		Anim.SetFloat ("Idle", Random.Range (0, 1.0f));
	}

	public void showDamage()
	{
		var g = (GameObject) Instantiate (Damage, damageSpawn);
		g.GetComponentInChildren<Text> ().text = Random.Range (-15, -5).ToString();
		g.transform.localPosition = Vector3.zero;
		g.transform.localEulerAngles = Vector3.zero;
		DamageGlow.SetActive (true);

	}

	public void StatsManager(bool show)
	{
		if (!show)
			CanvasAnim.SetBool ("Animate", false);
		else
			CanvasAnim.SetBool ("Animate", true);
	}

	public void lookAtPlayer( )
	{
		var g = Utilities.InstantiateObject (ArcaneRune, this.transform);
		t = 0;
		look = true;
	}

	void Update () {
		if (look) {
			t += Time.deltaTime * lookSpeed;
			var rot = Quaternion.LookRotation (targetPlayer.position-transform.position); 
			transform.rotation = Quaternion.Slerp (transform.rotation, rot, t); 
			if (t >= 1) {
				look = false;
			}
		}
	}

	public void hit()
	{
//		print ("Got hit!!");
		Anim.SetFloat ("Hits", Random.Range (0, 1.0f));
		Anim.SetTrigger ("Hit");
		showDamage ();
	}

	public void active()
	{
		Anim.SetTrigger ("Active");
	}

	public void deactive()
	{
		Anim.SetTrigger ("Deactive");
	}

	public void attackPlayer(SpellType spelltype ,Color col)
	{
		if (spelltype == SpellType.bless) {
			StartCoroutine(AttackFXManger(0,true,0,col)); 
		}
		else if (spelltype == SpellType.curse) {
			StartCoroutine(AttackFXManger(1,false,1,col));
		}
		else if (spelltype == SpellType.jinx) {
			StartCoroutine(AttackType2(col));
		}
		else if (spelltype == SpellType.whiteFlame) {
			StartCoroutine(AttackType3(col));
		}
		else if (spelltype == SpellType.sunEater) {
			
		}
		else if (spelltype == SpellType.hex) {
			
		}
	}

	IEnumerator AttackFXManger(int type, bool hasBuildUp , float animationType,Color col, float animationDelay =.7f) 
	{
		yield return new WaitForSeconds (1.5f);
		if (type != 1 ) {
			Anim.SetFloat ("Attacks", animationType); 
			Anim.SetTrigger ("Attack");
		}

		if (hasBuildUp) { 
			var effectHand = (GameObject) Instantiate (attack1Hand, EffectSpawnPoint);
			var cols =  attack1Hand.GetComponentsInChildren < RFX4_EffectSettingColor >();
			foreach (var item in cols) {
				item.Color = col;
			}
			effectHand.transform.localPosition = Vector3.zero;
			effectHand.transform.localEulerAngles = Vector3.zero;
			yield return new WaitForSeconds (animationDelay);
		}

		if (type != 2) {
			GameObject fx = null;
			if (type == 0) {
				 fx = (GameObject)Instantiate (attack [type], EffectSpawnPoint);
			} else {
				 fx = (GameObject)Instantiate (attack [type], EffectSpawnPoint);
			}
			fx.transform.localPosition = Vector3.zero; 
			fx.transform.localEulerAngles = Vector3.zero;
			fx.transform.SetParent (targetPlayer);
			var cols =  fx.GetComponentsInChildren < RFX4_EffectSettingColor >();
			foreach (var item in cols) {
				item.Color = col;
			}
			//			var localT = ArenaCharacterSpawner.SelectedPlayer.transform.position;
			var localT = targetPlayer.position;
			if (type == 1)
				fx.transform.localPosition = new Vector3 (fx.transform.localPosition.x - .35f, fx.transform.localPosition.y, fx.transform.localPosition.z + .15f);
			localT.y += 2;
			fx.transform.LookAt (localT);
		}  

			if (type == 1) {
				yield return new WaitForSeconds (1);
				Anim.SetFloat ("Attacks", animationType);
				Anim.SetTrigger ("Attack");
			}
		
	}

	IEnumerator AttackType2(Color col )
	{
		yield return new WaitForSeconds (1.5f);
			Anim.SetFloat ("Attacks", .8333f);
			Anim.SetTrigger ("Attack");

			var fx = (GameObject) Instantiate (attack[2], EffectSpawnPoint);
			fx.transform.localPosition = Vector3.zero;
			fx.transform.localEulerAngles = Vector3.zero;

		var cols =  fx.GetComponentsInChildren < RFX4_EffectSettingColor >();
		foreach (var item in cols) {
			item.Color = col;
		}
			yield return new WaitForSeconds (7.9f);
		var localT = targetPlayer.position;

//			var localT = ArenaCharacterSpawner.SelectedPlayer.transform.position;
			fx.transform.SetParent (targetPlayer);
			localT.y += 2;
			fx.transform.LookAt (localT);

	}

	IEnumerator AttackType3(Color col )
	{
		yield return new WaitForSeconds (1.5f);
		Anim.SetFloat ("Attacks", .6666f);
		Anim.SetTrigger ("Attack");
		yield return new WaitForSeconds (1f);

		var fx = (GameObject) Instantiate (attack[3], EffectSpawnPoint_RFX4);
		fx.transform.localPosition = Vector3.zero;
		fx.transform.localEulerAngles = Vector3.zero;

		var cols =  fx.GetComponentsInChildren < RFX4_EffectSettingColor >();
		foreach (var item in cols) {
			item.Color = col;
		}
		var localT = targetPlayer.position;
//		var localT = ArenaCharacterSpawner.SelectedPlayer.transform.position;
		fx.transform.SetParent (targetPlayer);
		fx.transform.localPosition = new Vector3(0,0,-3f);
		localT.y += 2;
		fx.transform.LookAt (localT);
	}
}
