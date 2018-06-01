using UnityEngine;
using System;
using System.Collections;
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

	public static void Login(string Username, string Password)
	{
		var data = new PlayerLoginAPI ();
		data.username = Username;
		data.password = Password;
		data.game = "covens";
		data.lat = 0;
		data.lng = 0;
		username = Username;
		Action<string,int> callback;
		callback = LoginCallback;
		APIManager.Instance.Post ("login",JsonConvert.SerializeObject (data), callback);
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
		ConditionsManager.Instance.Init ();
		foreach (var item in data.character.spellBook) {
			SpellCastAPI.spells.Add (item.id, item);
		}
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
	public static void CreateAccount(string Username, string Password, string Email)
	{
		var data = new PlayerLoginAPI ();  
		data.username = Username; 
		data.password = Password;
		data.email = Email;
		data.game = "covens";  
		data.lat = 0;
		data.lng = 0; 
		username = Username;
		Action<string,int> callback;
		callback = CreateAccountCallback;
		APIManager.Instance.Put ("create-account",JsonConvert.SerializeObject (data), callback);  
	}

	static void CreateAccountCallback(string result,int status)
	{
		if (status == 200) {
			print ("Account Created");
			try{
				var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
				loginToken = data.token;
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

	public static void CreateCharacter(string Username, bool isMale)
	{
		var data = new PlayerCharacterCreateAPI ();  
		data.displayName = Username; 
		data.latitude = 38.44;
		data.longitude= -78.8; 
		username = Username; 
		Action<string,int> callback;
		callback = CreateCharacterCallback;
		APIManager.Instance.Put ("create-character",JsonConvert.SerializeObject (data), callback,true);  
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
		foreach (var item in data.ingredients.gems) {
			data.ingredients.gemsDict.Add(item.displayName,item);
		}
		foreach (var item in data.ingredients.tools) {
			data.ingredients.toolsDict.Add(item.displayName,item);
		}
		foreach (var item in data.ingredients.herbs) {
			data.ingredients.herbsDict.Add(item.displayName,item);
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
		APIManager.Instance.Post ("request-reset",JsonConvert.SerializeObject (data), callback);
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
		APIManager.Instance.Post ("request-reset",JsonConvert.SerializeObject (data), callback);
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
		APIManager.Instance.Post ("reset-password",JsonConvert.SerializeObject (data), callback);
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
