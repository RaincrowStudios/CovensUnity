using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class ChatConnectionManager : MonoBehaviour {

	public static ChatConnectionManager Instance { get; set;}

	public static string coven = "risers";
	public static string dominion = "Virginia";
	public static ChatContainer AllChat;
	WebSocket serverChat;
	WebSocket serverCoven;
	WebSocket serverDominion;

	void Awake()
	{
		Instance = this;
	}

	IEnumerator Start () {

		serverChat = new WebSocket(new Uri("ws://localhost:1000/Chat"));
		serverCoven = new WebSocket(new Uri("ws://localhost:1000/risers"));
		serverDominion = new WebSocket(new Uri("ws://localhost:1000/Virginia"));

		yield return StartCoroutine(serverChat.Connect());
		yield return StartCoroutine(serverCoven.Connect());
		yield return StartCoroutine(serverDominion.Connect());


		ChatData CD = new ChatData {
			Name = "grim",
			Coven = coven,
			Dominion = dominion,
			CommandRaw = Commands.Connected.ToString()
		};

		send (CD);

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

			string replyc = serverCoven.RecvString ();
			if (replyc != null) {
				print (replyc + "Coven");
				ProcessJsonString (replyc);
			}
			if (serverCoven.error != null) {
				Debug.LogError ("Error: " + serverCoven.error);
				break;
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
		serverChat.Send (System.Text.Encoding.UTF8.GetBytes( JsonConvert.SerializeObject (data)));
	}

	public void ProcessJsonString(string rawData)
	{
		try {
			var Data = JsonConvert.DeserializeObject<ChatData>(rawData);
			ChatUI.Instance.AddItemHelper(Data);
			ChatUI.Instance.addNotification(Data);
			print("Yo!");
			return;
		} catch (Exception ex) {
		}
		try {
			AllChat = JsonConvert.DeserializeObject<ChatContainer>(rawData);
			ChatUI.Instance.initNotifications();
			ChatUI.Instance.Init();
			print("Y2o!");

		} catch (Exception ex) {
			
		}
	}
}

public enum Commands
{
	Connected, WorldLocation, CovenLocation, WorldMessage, CovenMessage, NewsMessage, NewsLocation, WhisperMessage, WhisperLocation, DominionMessage, DominionLocation 
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
	public string WhisperUIPlayer ;

}

public class ChatContainer
{
	public string CommandRaw { get; set; }
	public List<ChatData> PvpChat { get; set; }
	public List<ChatData> WorldChat { get; set; }
	public List<ChatData> CovenChat { get; set; }
	public List<ChatData> DominionChat { get; set; }
	public List<ChatData> News { get; set; }
}
