using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


public class ApparelManager : MonoBehaviour
{
	public static ApparelManager instance { get; set;}
	[HideInInspector]
	public ApparelView ActiveViewPlayer;

	public ApparelView male;
	public ApparelView female;

	void Awake()
	{
		instance = this;
	}

	public void SetupApparel()
	{
		if (PlayerDataManager.playerData.male) {
			female.gameObject.SetActive (false);
			male.gameObject.SetActive (true);
			male.InitializeChar (PlayerDataManager.playerData.equipped);
			ActiveViewPlayer = male;
		} else {
			female.gameObject.SetActive (true);
			male.gameObject.SetActive (false);
			female.InitializeChar (PlayerDataManager.playerData.equipped);
			ActiveViewPlayer = female;
		}
	}
}

public class ApparelData
{
	public string id { get; set; }
	[JsonIgnore]
	public string storeCatagory { get; set; }

	public string iconId{ get; set; }
	[JsonIgnore]
	public string catagory{ get; set; }

	public Assets assets {get; set;}

	public string position { get; set; }
	
	public List<string> conflicts { get; set; }

	public bool owned { get; set; }

	public int gold { get; set; }

	public int silver { get; set; }

	[JsonIgnore]
	public ApparelButtonData buttonData;

	[JsonIgnore]
	public bool isNew;

	[JsonIgnore]
	public bool isEquippedNew;
	 
	[JsonIgnore]
	public ApparelType apparelType;
}

public class Assets{
	
	public List<string> shadow { get; set; }

	public List<string> white { get; set; }

	public List<string> grey { get; set; }

	public List<string> baseAsset { get; set; }
}

public enum ApparelType{
	Base,Grey,White,Shadow
}

public class EquippedApparel{
	
	public string position { get; set;} 

	public string id { get; set;}

	public List<string> assets { get; set;}

}

