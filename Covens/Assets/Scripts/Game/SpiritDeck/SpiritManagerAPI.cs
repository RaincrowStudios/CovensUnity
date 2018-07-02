using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;

public class SpiritManagerAPI : MonoBehaviour
{

	public static void GetData()
	{
		Action<string,int > callback;
		callback = ResponseCallBack;
		APIManager.Instance.PostData ("character/portals/active", "null", callback);
	}

	static void ResponseCallBack (string result, int response)
	{
		print (result);
	}
}

