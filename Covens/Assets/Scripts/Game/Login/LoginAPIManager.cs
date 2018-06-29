using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
[RequireComponent(typeof(WebSocketClient))]
public class LoginAPIManager : MonoBehaviour
{

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
        if (pOnResponse == null)
            pOnResponse = LoginCallback;
		APIManager.Instance.Post ("login",JsonConvert.SerializeObject (data), pOnResponse, false, false);
	}

	static void ContinueLogin (string result)
	{
		var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
		loginToken = data.token;
		wssToken = data.wsToken;
		print (data.character.displayName);
		data.character = DictifyData (data.character);
		InitCondition (data);
		WebSocketClient.Instance.InitiateWSSCOnnection ();
		PlayerDataManager.playerData = data.character;
		PlayerDataManager.attackRadius = data.config.interactionRadius;
		PlayerDataManager.DisplayRadius = data.config.displayRadius;
		LoginUIManager.Instance.CorrectPassword ();
        try
        {
            ConditionsManager.Instance.Init();
        }catch(Exception e) { Debug.LogError("Error Here: " + e.Message); }
		foreach (var item in data.character.spellBook) {
			SpellCastAPI.spells.Add (item.id, item);
		}
        CovenController.LoadPlayerData();
		Add("spell_attack","Attack", 0,"",-1);
		Add("spell_ward","Ward", 0,"",1);
	}

	static void Add (string id, string name, int cost , string desc, int degree) {
		SpellData sd = new SpellData ();
		sd.id = id;
		sd.displayName= name;
		sd.cost = cost;
		sd.description= desc;
		sd.school = degree;
		SpellCastAPI.spells.Add (id, sd);
	}

	static void LoginCallback(string result,int status)
	{
		if (status == 200) {
			print ("Logged In");
			try{
			ContinueLogin (result);
			}catch(Exception e) {
				Debug.LogError (e);
			}
	
		}
		else {
			LoginUIManager.Instance.WrongPassword ();	
			print (status);
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
		data.latitude = 38.44;
		data.longitude= -78.8; 
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
			try{
				ContinueLogin(result);
			}catch(Exception e) {
				Debug.LogError (e);
			}
		}
		else {
			//	LoginUIManager.Instance.WrongPassword ();	handle error
			print (status);
		}
	}

    #endregion

    public static MarkerDataDetail DictifyData(MarkerDataDetail data)
    {
        foreach (var item in data.ingredients.gems)
        {
            if (!data.ingredients.gemsDict.ContainsKey(item.id))
                data.ingredients.gemsDict.Add(item.id, item);
        }
        foreach (var item in data.ingredients.tools)
        {
            if (!data.ingredients.toolsDict.ContainsKey(item.id))
                data.ingredients.toolsDict.Add(item.id, item);
        }
        foreach (var item in data.ingredients.herbs)
        {
            if (!data.ingredients.herbsDict.ContainsKey(item.id))
                data.ingredients.herbsDict.Add(item.id, item);
        }

        return data;
    }

	static void InitCondition(PlayerLoginCallback data)
	{
		foreach (var item in data.character.conditions) {
			item.displayName = "Hex";
			ConditionsManager.Conditions.Add (item.instance, item);
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
