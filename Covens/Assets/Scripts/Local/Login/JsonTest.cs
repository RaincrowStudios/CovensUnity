using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class JsonTest : MonoBehaviour {

	public string jsonText;
	// Use this for initialization
	void Start () {
		try{
		var d = JsonConvert.DeserializeObject<PlayerLoginCallback> (jsonText);
			print(d.token);
			print(d.account.username);
			print(d.character.dominion);
		}catch(Exception e) {
			print (e);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
