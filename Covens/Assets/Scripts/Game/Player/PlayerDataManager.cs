using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerManagerUI))]

public class PlayerDataManager : MonoBehaviour {

	
	public static MarkerDataDetail playerData; 
	public static Vector2 playerPos;
	public static float attackRadius = .5f;
	public static float DisplayRadius = .5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
