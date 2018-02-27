using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;


public class ArenaAttackManager : MonoBehaviour
{
	public static ArenaAttackManager Instance { get; set;}

	public GameObject[] attack;
	public GameObject attack1Hand;
	public GameObject DamageCanvas;
	public ArenaCharacterManager.SpellType spellType = ArenaCharacterManager.SpellType.curse;
	bool isDamagedShown = false;
	ArenaCharacterManager ACM;
	public List<GameObject> Damages = new List<GameObject> ();
	void Awake()
	{
		Instance = this;
	}

	public void hit()
	{
		
		ArenaCamController.Instance.SetTopView ();
		ACM = ArenaCharacterSpawner.Player.GetComponent<ArenaCharacterManager> ();
		ACM.attackPlayer (spellType, Color.red);
	}
		
	void Start()
	{
		EventManager.OnArenaPlayerHit += takeDamage;
		EventManager.OnArenaDamageInfoTap += LightenArena;
	}

	void takeDamage(GameObject g)
	{
		g.GetComponent<ArenaCharacterManager> ().hit ();
		ArenaCamController.Instance.DarkenArena ();
		var DamageCanvasLocal = (GameObject)Instantiate (DamageCanvas);
		int a = Random.Range (0, 6);
		string s = "";
		if (a == 0)
			s = "HEX";
		else if (a == 1)
			s = "Hex";
		else if (a == 2)
			s = "SUN EATER";
		else if (a == 3)
			s = "CURSE";
		else if (a == 4)
			s = "JINX";
		else if (a == 5)
			s = "DUSK FALL";
		DamageCanvasLocal.GetComponentInChildren<Text>().text = g.name + " HAS BEEN HIT BY A <color=#8700FFFF>" + s + "</color> DOING " + Random.Range(4,15).ToString() + " DAMAGE TO HER";
		Damages.Add (DamageCanvasLocal);
	}

	void LightenArena()
	{
		if (Damages.Count <= 1) {
			ArenaCamController.Instance.LightenArena ();
		}
	}
		
}

