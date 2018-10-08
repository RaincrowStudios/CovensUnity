using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic; 

public class LocalizationManager : MonoBehaviour
{
	public static Dictionary<string,string> LocalizeDictionary = new Dictionary<string, string> (); 
	public static event Action OnChangeLanguage;

	class LocalizationData {
		public string id{ get; set;}
		public string value{ get; set;	}
	}
}


