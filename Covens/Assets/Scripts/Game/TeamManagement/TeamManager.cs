using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamManager : MonoBehaviour
{

//	public static void GetCharacterInvites(){
//		APIManager.Instance.GetData ("character/invite", (string s, int r) => {
//			
//		});
//	}

	public static void GetCharacterInvites(){
		APIManager.Instance.GetData ("character/invite", (string s, int r) => {

		});
	}
}

public enum TeamPrefabType
{
    Member, InviteRequest, Ally, UnAlly
}

public class TeamData
{
    public string username { get; set; }
    public int level { get; set; }
    public string covenName { get; set; }
    public string title { get; set; }
    public string status { get; set; }
    public TeamPrefabType type;
}