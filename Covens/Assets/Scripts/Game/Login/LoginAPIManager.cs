using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

[RequireComponent(typeof(WebSocketClient))]
public class LoginAPIManager : MonoBehaviour
{
	public static bool loggedIn = false;
	static string token;
	public static string username;
	static string password;
	public static string loginToken;
	public static string wssToken;
	public static bool isNewAccount = true;

	public static void Login(string Username, string Password )
	{
		isNewAccount = false;
		var data = new {
			username = Username,
			password = Password,
			lng = OnlineMapsLocationService.instance.position.y,
			lat = OnlineMapsLocationService.instance.position.x, 
			UID = SystemInfo.deviceUniqueIdentifier
		};
		APIManager.Instance.Post ("login",JsonConvert.SerializeObject (data), LoginCallback, false, false);
	}

	static void LoginCallback(string result,int status)
	{
		if (status == 200) {
			TextEditor te = new TextEditor();
			te.content = new GUIContent( result);
			te.SelectAll();
			te.Copy();
			var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
			SetupConfig (data.config);
			loginToken = data.token;
			wssToken = data.wsToken;
			loggedIn = true;

		} else {
			DownloadAssetBundle.Instance.gameObject.SetActive (false);
			LoginUIManager.Instance.initiateLogin ();
			LoginUIManager.Instance.WrongPassword ();	
			print (status + "," + result);
		}
	}

	public static void WebSocketConnected( )
	{
		print ("WebSocketConnected");
		if (isNewAccount) {
			LoginUIManager.Instance.CreateAccountResponse (true, "");
		} else {
			GetCharacter ();
		}
	}

	static void SetupConfig(Config data)
	{
		PlayerDataManager.attackRadius = data.interactionRadius*.35f;
		PlayerDataManager.DisplayRadius = data.displayRadius;
		LocationUIManager.idleTimeOut = data.idleTimeLimit;
		foreach (var item in data.summoningMatrix) { 
			PlayerDataManager.SpiritToolsDict.Add (item.spirit, item.tool);
			PlayerDataManager.ToolsSpiritDict.Add (item.tool, item.spirit); 
		}
		WebSocketClient.Instance.InitiateWSSCOnnection ();
	}

	static void GetCharacter()
	{
		APIManager.Instance.GetData ("character/get",OnGetCharcterResponse);
	}

	static void OnGetCharcterResponse(string result, int response)
	{
		if (response == 200) {
			var data = JsonConvert.DeserializeObject<MarkerDataDetail> (result);

			PlayerDataManager.playerData = DictifyData (data);
			PlayerDataManager.currentDominion = data.dominion;
			LoginUIManager.Instance.CorrectPassword ();
			ChatConnectionManager.Instance.InitChat ();
			ApparelManager.instance.SetupApparel ();
			PushManager.InitPush ();
			GetKnownSpirits ();
			DownloadAssetBundle.Instance.gameObject.SetActive (false);

		} else {
			Debug.LogError (result);
		}
	}

	static void GetKnownSpirits(){
		APIManager.Instance.PostData ("/character/spirits/known", "null", ReceiveSpiritData, true);
	}

	static void ReceiveSpiritData(string response, int code)
	{
		if (code == 200) {
			var knownData = JsonConvert.DeserializeObject<List<SpiritData>>(response);
			foreach (var item in knownData) {
				PlayerDataManager.playerData.KnownSpiritsList.Add (item.id);
			}
			//			print (response + "Known Spirit Data Fetched");
		}
	}

	static MarkerDataDetail DictifyData(MarkerDataDetail data)
	{
		foreach (var item in data.ingredients.gems)
		{
			if (!DownloadedAssets.ingredientDictData.ContainsKey (item.id)) {
				print (item.id);
				continue;
			}
			item.name = DownloadedAssets.ingredientDictData [item.id].name;
			item.rarity = DownloadedAssets.ingredientDictData [item.id].rarity;
			data.ingredients.gemsDict[item.id] =  item;
		}
		foreach (var item in data.ingredients.tools)
		{
			if (!DownloadedAssets.ingredientDictData.ContainsKey (item.id)) {
				//				print (item.id);
				continue;
			}
			item.name = DownloadedAssets.ingredientDictData [item.id].name;
			item.rarity = DownloadedAssets.ingredientDictData [item.id].rarity;
			data.ingredients.toolsDict[item.id] =  item;
		}
		foreach (var item in data.ingredients.herbs)
		{
			if (!DownloadedAssets.ingredientDictData.ContainsKey (item.id)) {
				print (item.id);
				continue;
			}
			item.name = DownloadedAssets.ingredientDictData [item.id].name;
			item.rarity = DownloadedAssets.ingredientDictData [item.id].rarity;
			data.ingredients.herbsDict[item.id] =  item;
		}

		foreach (var item in data.spells) {
			item.school = DownloadedAssets.spellDictData [item.id].spellSchool;
			data.spellsDict.Add (item.id, item);
		}
		try{
			foreach (var item in data.cooldownList) {
				data.cooldownDict [item.instance] = item;
			}
		}catch{
			// nothing to cooldown
		}

		foreach (var item in data.inventory.cosmetics) {
			Utilities.SetCatagoryApparel (item);
		}

		foreach (var item in data.conditions) {
			data.conditionsDict.Add (item.instance, item);
			if (item.status == "silenced") {
				BanishManager.isSilenced = true;
				BanishManager.silenceTimeStamp = item.expiresOn;
			}
			if (item.status == "bound") {
				BanishManager.isBind = true;
				BanishManager.bindTimeStamp = item.expiresOn;
				BanishManager.Instance.BindLogin ();
			}
		}
		if (data.conditionsDict.Count == 0) { 
			ConditionsManager.Instance.SetupButton (false);
		} else {
			ConditionsManager.Instance.SetupButton (true);
		}

		return data; 
	}

	public static void CreateAccount(string Username, string Password, string Email)
	{
		isNewAccount = true;
		var data = new PlayerLoginAPI ();  
		data.username = Username; 
		data.password = Password;
		data.email = Email;
		data.game = "covens";  
		data.lat = OnlineMapsLocationService.instance.position.y;
		data.lng = OnlineMapsLocationService.instance.position.x; 
		username = Username;
		data.UID = SystemInfo.deviceUniqueIdentifier;

		APIManager.Instance.Put ("create-account",JsonConvert.SerializeObject (data), CreateAccountCallback, false, false);  
	}

	static void CreateAccountCallback(string result,int status)
	{
		if (status == 200) {
			print ("Account Created");
			print (result);
			var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
			loginToken = data.token;
			wssToken = data.wsToken;
			SetupConfig(data.config);
		} else {
			//	LoginUIManager.Instance.WrongPassword ();	handle result
			if (result == "4103") {
				LoginUIManager.Instance.CreateAccountResponse (false, "Username is in already taken.");
			} else if (result == "4104") {
				LoginUIManager.Instance.CreateAccountResponse (false, "Username is invalid.");
			}
			else 	if (result == "4201") {
				LoginUIManager.Instance.CreateAccountResponse (false, "Session has expired.");
			}else {
				LoginUIManager.Instance.CreateAccountResponse (false, "Something went wrong. Error code : " + result);
			}
		}
	}

	public static void CreateCharacter(string Username, bool isMale)
	{
		var data = new PlayerCharacterCreateAPI ();  
		data.displayName = Username; 
		data.latitude = OnlineMapsLocationService.instance.position.y;
		data.longitude= OnlineMapsLocationService.instance.position.x; 
		data.male = isMale;
		username = Username;
		APIManager.Instance.Put ("create-character",JsonConvert.SerializeObject (data), CreateCharacterCallback, true, false);  
	}

	static void CreateCharacterCallback(string result,int status)
	{
		if (status == 200) {
			var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
			loginToken = data.token;
			GetCharacter ();
		}
		else {
			print (status + " " + result);
		}
	}

	#region Password Reset

	public static void ResetPasswordRequest(string Username)
	{
		var data = new PlayerResetAPI ();
		data.username = Username;
		username = Username;
		Action<string,int> callback;
		callback = ResetPasswordRequestCallback;
		APIManager.Instance.Post ("request-reset",JsonConvert.SerializeObject (data), callback, false, false);
	}

	static void ResetPasswordRequestCallback(string result,int status)
	{
		if (status == 200) {
			string email = JsonConvert.DeserializeObject<PlayerResetCallback> (result).email; 
			LoginUIManager.Instance.EnterResetCode (email);	
		} else { 
			if (result == "4102") {
				LoginUIManager.Instance.EmailNull ();	
			} else if (result == "4101") {
				LoginUIManager.Instance.resetUserNull ();	
			}
		}
	}

	public static void SendResetCode(string code)
	{
		var data = new PlayerResetAPI ();
		data.code = code;
		data.username = username;

		Action<string,int> callback;
		callback = SendResetCodeCallback;
		APIManager.Instance.Post ("request-reset",JsonConvert.SerializeObject (data), callback, false, false);
	}

	static void SendResetCodeCallback(string result,int status)
	{
		if (status == 200) {
			token = JsonConvert.DeserializeObject<PlayerPasswordCallback> (result).token;
			print (token);
			LoginUIManager.Instance.FinishPasswordReset ();	
		}
		else {
			LoginUIManager.Instance.ResetCodeWrong ();	
		}
	}

	public static void SendNewPassword(string password)
	{
		var data = new PlayerResetAPI ();
		data.password = password;
		data.token = token;
		data.username = username;

		Action<string,int> callback;
		callback = SendResetCodeCallback;
		APIManager.Instance.Post ("reset-password",JsonConvert.SerializeObject (data), callback, false, false);
	}

	static void SendNewPasswordCallback(string result,int status)
	{
		if (status == 200) {
			LoginUIManager.Instance.PostPasswordReset (username,password);	
		}
		else {
			if (result == "4200") {
				LoginUIManager.Instance.PasswordTokenError ("Session has expired");
			}
		}
	}

	#endregion
}
