using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SpellCastAPI : MonoBehaviour
{

	public static void CastSummon( )
	{
		var data = new MapAPI ();
		data.characterName = PlayerDataManager.playerData.displayName; 
		Action<string,int> callback;
		callback = GetMarkersCallback;
		APIManager.Instance.PostCoven ("map/portal", JsonConvert.SerializeObject (data), callback);
	}

	static void GetMarkersCallback (string result, int response)
	{
		if (response == 200) {
			try{

			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}

}

