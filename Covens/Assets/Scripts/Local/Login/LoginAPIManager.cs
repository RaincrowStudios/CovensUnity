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

	static void LoginCallback(string result,int status)
	{
		if (status == 200) {
			print ("Logged In");
			try{
			var data = JsonConvert.DeserializeObject<PlayerLoginCallback> (result);
				loginToken = data.token;
				wssToken = data.wsToken;
				print(data.character.displayName);
				data.character = DictifyData(data.character);

				WebSocketClient.Instance.InitiateWSSCOnnection();
				PlayerDataManager.playerData = data.character;
				LoginUIManager.Instance.CorrectPassword ();	

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

	public static MarkerDataDetail DictifyData(MarkerDataDetail data)
	{
		foreach (var item in data.inventory.gems) {
			data.inventory.gemsDict.Add(item.displayName,item);
		}
		foreach (var item in data.inventory.tools) {
			data.inventory.toolsDict.Add(item.displayName,item);
		}
		foreach (var item in data.inventory.herbs) {
			data.inventory.herbsDict.Add(item.displayName,item);
		}
		return data;
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
