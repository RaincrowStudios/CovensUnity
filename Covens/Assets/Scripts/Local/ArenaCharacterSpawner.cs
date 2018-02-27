using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaCharacterSpawner : MonoBehaviour {

	public static List<string> charNames = new List<string>(){"WIHNHILDA", "AILEEN", "ROWENA", "ADONIA","GEWNDOLYN","HYACINTH", "BELLADONNA","LORELEI","RAVEN" };

	public static ArenaCharacterSpawner Instance { get; set;}
	public Transform playerSpawn;
	public Transform [] yourCovenSpawn;
	public Transform [] enemyCovenSpawnA;
	public Transform [] enemyCovenSpawnB;
	public static GameObject Player;
	public static List<GameObject> CovenA = new List<GameObject>();
	public static List<GameObject> CovenB = new List<GameObject>();
	public static List<GameObject> YourCoven = new List<GameObject>();
	public GameObject[] character;
	public static Transform SelectedPlayer;
	// Use this for initialization
	int nameCount = 0;
	void Awake()
	{
		Instance = this;

	}

	void Update()
	{
		if (Input.GetMouseButtonUp (0)) {
			RaycastHit hit = new RaycastHit ();
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 5000)) {
				if (hit.transform.tag == "ArenaEnemy") {
					print ("Found Enemy");
					Player.GetComponent<ArenaCharacterManager> ().targetPlayer = hit.transform;
					Player.GetComponent<ArenaCharacterManager> ().lookAtPlayer ();
					ArenaCamController.Instance.SetZoomToPlayer ();
				} else if(hit.transform.tag == "ArenaFriend"){
					print ("Found Friend");
				} else if(hit.transform.tag == "ArenaPlayer"){
					print ("Found Player");
				} 
			}
		}
	}
		

	void Start () {
		try{
		Player = (GameObject) Instantiate (character[Random.Range(0,character.Length)], playerSpawn);
		Player.name = charNames [nameCount];
		nameCount++;
		Player.tag = "ArenaPlayer";
		}catch(System.Exception e){
			print (e);
		}
		foreach (var item in yourCovenSpawn) {
			var g = (GameObject) Instantiate (character[Random.Range(0,character.Length)], item);
			g.name = charNames [nameCount];

			nameCount++;
			g.tag = "ArenaFriend";
			YourCoven.Add (g);
		}

		foreach (var item in enemyCovenSpawnA) {
			var g = (GameObject) Instantiate (character[Random.Range(0,character.Length)], item);
			g.name = charNames [nameCount];
			nameCount++;
			var a = g.transform.GetChild (3);
			var info = a.GetComponentsInChildren<Text> ();
			info [0].text = g.name;
			info [1].text = "Level " + Random.Range (4, 8);
			info [2].text =  Random.Range (40, 80).ToString();
			g.tag = "ArenaEnemy";
			CovenA.Add (g);
		}
		int k = 0;
		foreach (var item in enemyCovenSpawnB) {
			GameObject g;
			if(k==1)
				g = (GameObject) Instantiate (character[3], item);
			else
			 g = (GameObject) Instantiate (character[Random.Range(0,character.Length)], item);
			g.name = charNames [nameCount];
			var a = g.transform.GetChild (3);
			var info = a.GetComponentsInChildren<Text> ();
			info [0].text = g.name;
			info [1].text = "Level " + Random.Range (4, 8);
			info [2].text = Random.Range (40, 80).ToString();
			nameCount++;
			g.tag = "ArenaEnemy";
			CovenB.Add (g);
			k++;
		}

		AttackManager.Team1 = YourCoven;
		AttackManager.Team2 = CovenA;
		AttackManager.Team3 = CovenB;
		AttackManager.Team1.Add (Player);
	}
	
}
