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
		APIManager.Instance.PostCoven ("coven/display", JsonConvert.SerializeObject(data), callback);
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



/*
covens/coven/display --> req: {covenName: str} --> res: {coven info}

covens/coven/ally --> req: {covenName: str} --> res: 200

covens/coven/unally --> req: {covenName: str} --> res: 200

covens/coven/create --> req: {covenName: str} --> res: 200

covens/coven/disband --> req: {} --> res: 200

covens/coven/request --> req: {covenName: str} --> res: 200 | WSS --> command: coven_member_request

covens/coven/invite --> req: {invitedId: str || invitedName: str} --> res: 200 | WSS --> inviteToken

covens/coven/join --> req: {inviteToken: str} --> res: 200 | WSS --> command: coven_member_join

covens/coven/leave -->req:  {} --> res: 200 | WSS --> command: coven_member_leave

covens/coven/kick --> req: {memberId: str || memberName: str} --> res: 200 | WSS --> command: coven_member_kick

covens/coven/title --> req: {title: str, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_title, title: str

covens/coven/promote --> req: {rank: int, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_promote, rank: int

covens/coven/location --> req: {memberId || memberName} --> res: {latitude: float, longitude: float} --> covens/map/move --> req: {physical: bool, latitude: float, longitude: float}
*/
