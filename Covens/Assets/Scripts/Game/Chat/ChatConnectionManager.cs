using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class ChatConnectionManager : MonoBehaviour {

	public static ChatConnectionManager Instance { get; set;}
	public static ChatContainer AllChat;
	WebSocket serverChat;
	WebSocket serverCoven;
	WebSocket serverDominion;
	string address = "ws://104.196.14.209:80/";
	string addressHttp = "http://104.196.14.209";
	void Awake()
	{
		Instance = this;
	}

	public void SetDominion()
	{
		try{
		serverDominion.Close ();
		}catch{
		}
		print (PlayerDataManager.currentDominion);
		serverDominion = new WebSocket(new Uri(address+PlayerDataManager.currentDominion));
	}

	public void SetCoven()
	{
		try{
			serverCoven.Close ();
			AllChat.CovenChat.Clear();
		}catch{
		}
		if(PlayerDataManager.playerData.coven != "")
			serverCoven = new WebSocket(new Uri(address+PlayerDataManager.playerData.coven));
	}

	public void InitChat()
	{
		StartCoroutine (EstablishWSConnection ());
	}


	IEnumerator EstablishWSConnection ()
	{
		{
			using (WWW www = new WWW (addressHttp)) {
				yield return www;
				if (www.error == null) {
					print (www.text + "From Chat Web Socket HTTP");
					StartCoroutine ( StartChart ());
				}
				print (www.error);
			}
		}
	}


	IEnumerator StartChart () {

		serverChat = new WebSocket(new Uri(address+"Chat"));
		ChatData CD = new ChatData {
			Name = PlayerDataManager.playerData.displayName,
			Coven =  PlayerDataManager.playerData.coven,
			Dominion = PlayerDataManager.currentDominion,
			CommandRaw = Commands.Connected.ToString()
		};



		yield return StartCoroutine(serverChat.Connect());
		send (CD);
		SetCoven ();
		SetDominion ();
		if(PlayerDataManager.playerData.coven != "")
		yield return StartCoroutine(serverCoven.Connect());
		yield return StartCoroutine(serverDominion.Connect());

	

		while (true) {
			string reply = serverChat.RecvString ();
			if (reply != null) {
				print (reply );
				ProcessJsonString (reply);
			}
			if (serverChat.error != null) {
				Debug.LogError ("Error: " + serverChat.error);
				break;
			}

			if (PlayerDataManager.playerData.coven != "") {
				print (PlayerDataManager.playerData.coven + " " + PlayerDataManager.playerData.coven.Length);
				string replyc = serverCoven.RecvString ();
				if (replyc != null) {
					print (replyc + "Coven");
					ProcessJsonString (replyc);
				}
				if (serverCoven.error != null) {
					Debug.LogError ("Error: " + serverCoven.error);
					break;
				}
			}
			string replyd = serverDominion.RecvString ();
			if (replyd != null) {
				print (replyd + "dom");
				ProcessJsonString (replyd);
			}
			if (serverDominion.error != null) {
				Debug.LogError ("Error: " + serverDominion.error);
				break;
			}
			yield return 0;
		}
		serverChat.Close ();
	}
		
	public void send(ChatData data)
	{
		print ("Sending " + JsonConvert.SerializeObject (data));
		serverChat.Send (System.Text.Encoding.UTF8.GetBytes( JsonConvert.SerializeObject (data)));
	}

	public void ProcessJsonString(string rawData)
	{
		try {
			var Data = JsonConvert.DeserializeObject<ChatData>(rawData);
			ChatUI.Instance.AddItemHelper(Data);
			ChatUI.Instance.addNotification(Data);
			return;
		} catch (Exception ex) {
		}
		try {
			AllChat = JsonConvert.DeserializeObject<ChatContainer>(rawData);
			ChatUI.Instance.initNotifications();
			ChatUI.Instance.Init();
		} catch (Exception ex) {
			
		}
	}
}

public enum Commands
{
	Connected, WorldLocation, CovenLocation, WorldMessage, CovenMessage, NewsMessage, NewsLocation, DominionMessage, DominionLocation 
}

public class ChatData
{
	public string Name { get; set; }
	public string Content { get; set; }
	public string Receiver { get; set; }
	public string Dominion { get; set; }
	public int Level { get; set; }
	public string Coven { get; set; }
	public double TimeStamp { get; set; }
	public int Avatar { get; set; }
	public int Degree { get; set; }
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public string CommandRaw { get; set; }
	public Commands Command;


}

public class ChatContainer
{
	public string CommandRaw { get; set; }
	public List<ChatData> WorldChat { get; set; }
	public List<ChatData> CovenChat { get; set; }
	public List<ChatData> DominionChat { get; set; }
	public List<ChatData> News { get; set; }
}
