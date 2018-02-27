using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {

	public bool isOff = false;

	public static List<GameObject> Team1  = new List<GameObject>();
	public static List<GameObject> Team2  = new List<GameObject>();
	public static List<GameObject> Team3  = new List<GameObject>();

	public Color col1;
	public Color col2;
	public Color col3;
	public Color col4;
	public float timeScaleVal = .3f;
	int currentTeam = 1;

	void Start () {
		Time.timeScale = timeScaleVal;
		if(isOff)
		StartCoroutine (Attack ());
	}

	IEnumerator Attack()
	{
		yield return new WaitForSeconds (0);
		if (currentTeam == 1) {
			foreach (var item in Team1) {
				int x = 4;
				if (x < 3) {
					item.GetComponent<ArenaCharacterManager> ().targetPlayer = Team2 [x].transform;
				} else {
					item.GetComponent<ArenaCharacterManager> ().targetPlayer = Team3 [x-3].transform;
				}
				StartCoroutine(LaunchAttack(item.GetComponent<ArenaCharacterManager>(),GetSpell()));
				yield return new WaitForSeconds (Random.Range(0.0f,2.0f));
			}
		}
			
		if (currentTeam == 2) {
			foreach (var item in Team2) {
				int x = 4;
				if (x < 3) {
					item.GetComponent<ArenaCharacterManager> ().targetPlayer = Team1 [x].transform;
				} else {
					item.GetComponent<ArenaCharacterManager> ().targetPlayer = Team3 [x-3].transform;
				}
				StartCoroutine(LaunchAttack(item.GetComponent<ArenaCharacterManager>(),GetSpell()));
				yield return new WaitForSeconds (Random.Range(0.0f,2.0f));
			}
		}

		if (currentTeam == 3) {
			foreach (var item in Team3) {
				int x = Random.Range (0, 6);
				if (x < 3) {
					item.GetComponent<ArenaCharacterManager> ().targetPlayer = Team2 [x].transform;
				} else {
					item.GetComponent<ArenaCharacterManager> ().targetPlayer = Team1 [x-3].transform;
				}
				StartCoroutine(LaunchAttack(item.GetComponent<ArenaCharacterManager>(),GetSpell()));
				yield return new WaitForSeconds (Random.Range(0.0f,2.0f));
			}
		}

		if (currentTeam < 3)
			currentTeam++;
		else
			currentTeam = 1;

		yield return new WaitForSeconds (Random.Range(4f,5f));

		StartCoroutine (Attack ());
	}

	IEnumerator LaunchAttack(ArenaCharacterManager AM, ArenaCharacterManager.SpellType sp)
	{
		AM.active ();
		AM.lookAtPlayer ();
		yield return new WaitForSeconds (Random.Range(2.0f,4.0f));
		AM.attackPlayer (sp,GetColor());
	}

	void Update () {
		
	}

	ArenaCharacterManager.SpellType GetSpell()
	{

//		return ArenaCharacterManager.SpellType.curse;


		int x = Random.Range (0, 4);
		if (x == 0)
			return ArenaCharacterManager.SpellType.bless;
		else if(x == 1)
			return ArenaCharacterManager.SpellType.curse;
		else if(x == 2)
			return ArenaCharacterManager.SpellType.jinx;
		else 
			return ArenaCharacterManager.SpellType.jinx;
	}

	Color GetColor()
	{
		int x = Random.Range (0, 4);
		if (x == 0)
			return col1;
		else if(x == 1)
			return col2;
		else if(x == 2)
			return col3;
		else 
			return col4;
	}
}
