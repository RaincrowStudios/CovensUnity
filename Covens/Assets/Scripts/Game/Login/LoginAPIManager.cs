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

	#region Login

	public static void Login(string Username, string Password, Action<string, int> pOnResponse = null)
	{
		var data = new PlayerLoginAPI ();
		data.username = Username;
		data.password = Password;
		data.game = "covens";
		data.lat = 0;
		data.lng = 0;
        data.email = "tes1at@test.com";
        username = Username;
		data.UID = SystemInfo.deviceUniqueIdentifier;
        if (pOnResponse == null)
            pOnResponse = LoginCallback;
		APIManager.Instance.Post ("login",JsonConvert.SerializeObject (data), pOnResponse, false, false);
	}

	static void ContinueLogin (string result)
	{
		var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
		loginToken = data.token;
		wssToken = data.wsToken;
		data.character = DictifyData (data.character);
		print ("GENDER IS MALE : " + data.character.male);
		WebSocketClient.Instance.InitiateWSSCOnnection ();
		PlayerDataManager.playerData = data.character;
		PlayerDataManager.currentDominion = data.character.dominion;
		PlayerDataManager.attackRadius = data.config.interactionRadius*.35f;
		PlayerDataManager.DisplayRadius = data.config.displayRadius;
		LoginUIManager.Instance.CorrectPassword ();
		ChatConnectionManager.Instance.InitChat ();
		LocationUIManager.idleTimeOut = data.config.idleTimeLimit;
		PushManager.InitPush ();
		GetKnownSpirits ();
		LoadControllers();
		foreach (var item in data.config.summoningMatrix) {
			PlayerDataManager.SpiritToolsDict.Add (item.spirit, item.tool);
			PlayerDataManager.ToolsSpiritDict.Add (item.tool, item.spirit);
		}

	}

    static private void LoadControllers()
    {
        CovenController.Load();
        StoreController.Load();
        IAPController.Load();
        UIManager.Get<WardrobeView>().Setup();
    }


    static void Add (string id, string name, int cost , string desc, int degree) {
		SpellData sd = new SpellData ();
		sd.id = id;
		sd.displayName= name;
		sd.cost = cost;
		sd.description= desc;
		sd.school = degree;
		SpellCastAPI.spells[id] = sd;
	}

	static void LoginCallback(string result,int status)
	{
		if (status == 200) {
			print ("Logged In");
			print (result);
			try{
			ContinueLogin (result);
				loggedIn = true;
			}catch(Exception e) {
				Debug.LogError (e);
			}
	
		}
		else {
			LoginUIManager.Instance.WrongPassword ();	
			print (status + "," + result);
		}
	}

	#endregion

	#region CreateAccount
	public static void CreateAccount(string Username, string Password, string Email, Action<string, int> pOnResponse = null)
	{
		var data = new PlayerLoginAPI ();  
		data.username = Username; 
		data.password = Password;
		data.email = Email;
		data.game = "covens";  
		data.lat = 0;
		data.lng = 0; 
		username = Username;
		data.UID = SystemInfo.deviceUniqueIdentifier;

        if(pOnResponse == null)
            pOnResponse = CreateAccountCallback;
	
		APIManager.Instance.Put ("create-account",JsonConvert.SerializeObject (data), pOnResponse, false, false);  
	}

	static void CreateAccountCallback(string result,int status)
	{
		if (status == 200) {
			print ("Account Created");
			try{
				var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
				loginToken = data.token;
                wssToken = data.token;
                LoginUIManager.Instance.CreateAccountResponse (true,"");
			}catch(Exception e) {
				Debug.LogError (e);
				LoginUIManager.Instance.CreateAccountResponse (false, "Something went wrong.");
			}
		}
		else {
		//	LoginUIManager.Instance.WrongPassword ();	handle result
			if (status == 4103) {
				LoginUIManager.Instance.CreateAccountResponse (false, "Username is in already taken.");
			} else if (status == 4104) {
				LoginUIManager.Instance.CreateAccountResponse (false, "Username is invalid.");
			}
			else 	if (status == 4201) {
				LoginUIManager.Instance.CreateAccountResponse (false, "Session has expired.");
			}else {
				LoginUIManager.Instance.CreateAccountResponse (false, "Something went wrong. Error code : " + status);
			}
			print (status);
		}
	}

	public static void CreateCharacter(string Username, bool isMale, Action<string, int> pOnResponse = null)
	{
		var data = new PlayerCharacterCreateAPI ();  
		data.displayName = Username; 
		data.latitude = OnlineMapsLocationService.instance.position.y;
		data.longitude= OnlineMapsLocationService.instance.position.x; 
		username = Username;
        if (pOnResponse == null)
            pOnResponse = CreateCharacterCallback;
		print (token);
		APIManager.Instance.Put ("create-character",JsonConvert.SerializeObject (data), pOnResponse, true, false);  
	}

	static void CreateCharacterCallback(string result,int status)
	{
		if (status == 200) {
			print ("Character Created");
			print (result);
			try{
				ContinueLogin(result);
				loggedIn = true;
			}catch(Exception e) {
				Debug.LogError (e);
			}
		}
		else {
			//	LoginUIManager.Instance.WrongPassword ();	handle error
			print (status + " " + result);
		}
	}

    #endregion

    public static MarkerDataDetail DictifyData(MarkerDataDetail data)
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

	static void GetKnownSpirits(){
		APIManager.Instance.PostData ("/character/spirits/known", "null", ReceiveData, true);
	}

	static void ReceiveData(string response, int code)
	{
		if (code == 200) {
			var knownData = JsonConvert.DeserializeObject<List<SpiritData>>(response);
			foreach (var item in knownData) {
				PlayerDataManager.playerData.KnownSpiritsList.Add (item.id);
			}
//			print (response + "Known Spirit Data Fetched");
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
