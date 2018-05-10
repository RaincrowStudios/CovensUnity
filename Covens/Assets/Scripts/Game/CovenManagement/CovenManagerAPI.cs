using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class CovenManagerAPI : MonoBehaviour {
	//
	public static void GetCovenData( )
	{
		var data = new CovenDataAPI ();
		//after login all player attributes are stored in PlayerDataManager > playerdata
		data.instanceID = PlayerDataManager.playerData.instance;
		//define action for the http response
		Action<string,int> callback;
		//assign the method to the action
		callback = CovenCallBack;
		//sends a get requests and adds a token. change the end point to whatever sean tells you
		APIManager.Instance.PostCoven ("coven/getPlayers", JsonConvert.SerializeObject(data), callback);
	}

	static void CovenCallBack (string result, int response)
	{
		//200 - success
		if (response == 200) {
			try{
				//parse the json data
				CovenData data = JsonConvert.DeserializeObject<CovenData>(result);
				// handle coven data;
			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}
}

