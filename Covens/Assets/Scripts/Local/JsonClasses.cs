using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class JsonClasses : MonoBehaviour
{

}

[Serializable]
public class Token
{
	public string displayName{ get; set; }
	public bool male{ get; set; }
	public float latitude{ get; set; }
	public float longitude{ get; set; }
	public int alignment{ get; set; }
	[NonSerialized] 
 	public GameObject Object;
	[NonSerialized] 
	public MarkerSpawner.MarkerType Type;
}

public class MarkerData
{	
	public string command { get; set; }
	public string instance { get; set; }
	public string target { get; set; }
	public bool dead { get; set; }
	public string type { get; set; }
	public Token token { get; set; }
}
	
[SerializeField]
public class MarkerDataDetail
{
	public string displayName{ get; set; }
	public string worldRank{ get; set; }
	public string covenStatus{ get; set; }
	public string favoriteSpell{ get; set; }
	public List<object> achievements { get; set; }
	public int energy{ get; set; }
	public int baseEnergy{ get; set; }
	public string dominion{ get; set; }
	public bool gender{ get; set; }
	public string coven{ get; set; }
	public int alignment{ get; set; }
	public int level{ get; set; }
	public int silver{ get; set; }
	public string description{ get; set; }
	public int summonOn{ get; set; }
	public int createdOn{ get; set; }
	public int expireOn{ get; set; }
	public string owner{ get; set; }
	public string ownerCoven{ get; set; }
	public int count{ get; set; }
	public List<object> conditions { get; set; }
}

public class Conditions
{
	public List<string> condition{ get; set; }
	public List<string> caster{ get; set; }
}

public class MarkerDetailContainer
{
	public MarkerDataDetail selection { get; set; }
	public bool immune { get; set; }
}

[SerializeField]
public class MapAPI
{
	public string characterName{ get; set; }
	public string target{ get; set; }
	public string type { get; set; }
	public bool physical{ get; set; }
	public float longitude{ get; set; }
	public float latitude{ get; set; }
}

#region Login

[Serializable]
public class PlayerLoginAPI
{
	public string username{ get; set; }
	public string password{ get; set; }
	public string game{ get; set; }
	public double lng{ get; set; }
	public double lat{ get; set; }
}

[Serializable]
public class PlayerLoginCallback
{
	public string token{ get; set; }
	public string wsToken{ get; set; }
	public Account account{ get; set; }
	public MarkerDataDetail character{ get; set; }
}

[Serializable]
public class Account
{
	public string username{ get; set; }
	public string email{ get; set; }
}



[Serializable]
public class PlayerResetCallback
{
	public string email{ get; set; }
}

[SerializeField]
public class PlayerPasswordCallback
{
	public string token{ get; set; }
}

[SerializeField]
public class PlayerResetAPI
{
	public string username{ get; set; }
	public string code{ get; set; }
	public string token{ get; set; }
	public string password{ get; set; }
}

#endregion

[Serializable]
public class MarkerAPI
{
	public List<MarkerData> tokens{ get; set; }
}
	


