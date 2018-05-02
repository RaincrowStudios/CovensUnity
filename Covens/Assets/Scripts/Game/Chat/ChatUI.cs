using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class ChatUI : MonoBehaviour
{
	public static ChatUI Instance { get; set; }

	public Text worldButton;
	public Text newsButton;
	public Text dominionButton;
	public Text covenButton;
	public Text whisperButton;
	public Text CovenUIText;

	public Text worldButtonNotification;
	public Text newsButtonNotification;
	public Text dominionButtonNotification;
	public Text covenButtonNotification;
	public Text whisperButtonNotification;
	public Dictionary<string,GameObject> pvpChatItems = new Dictionary<string, GameObject>();
	public List<GameObject> chatItems = new List<GameObject> ();
	public Sprite[] profilePics;
	public GameObject locationPrefab;
	public GameObject chatPrefab;
	public Transform container;
	public GameObject ChatParentObject;

	public GameObject Header;
	public GameObject PvPHeader;
	public Text PvPName;
	public Text PvPDegree;
	public static string selectedPvPPlayer = null;
	HashSet<string> pvpPlayerNames = new HashSet<string> ();
	public static string playerName = "grim";

	public InputField inputMessage;
	public Button shareLocation;
	public Button sendButton;

	int newsNoti,worldNoti,covenNoti,dominionNoti, whisperNoti =0; 

	public enum ChatWindows
	{
		News,
		World,
		Covens,
		Dominion,
		Whispers}

	;

	public ChatWindows ActiveWindow = ChatWindows.World;

	void Awake ()
	{
		Instance = this;
	}

	public void Init ()
	{
		ChatParentObject.SetActive (true);
		SwitchWindow ("world");
	}

	public void initNotifications()
	{
		worldButtonNotification.text = covenButtonNotification.text = whisperButtonNotification.text = newsButtonNotification.text = dominionButtonNotification.text = "";
		newsNoti = worldNoti = covenNoti = dominionNoti = whisperNoti = 0;
	}

	public void addNotification( ChatData data)
	{
		var c = (Commands)Enum.Parse (typeof(Commands),data.CommandRaw);
		data.Command = c;
		print ("Adding stuff");
		if (c == Commands.CovenLocation || c == Commands.CovenMessage) {
			if (ActiveWindow != ChatWindows.Covens) {
				covenNoti++;
				covenButtonNotification.text = covenNoti.ToString ();
			}
			ChatConnectionManager.AllChat.CovenChat.Add (data);

		} 

		else 	if (c == Commands.WorldMessage || c == Commands.WorldLocation) {
			if (ActiveWindow != ChatWindows.World) {
				worldNoti++;
				worldButtonNotification.text = worldNoti.ToString ();
			}
			ChatConnectionManager.AllChat.WorldChat.Add (data);
		} 

		else 	if (c == Commands.WhisperMessage || c == Commands.WhisperLocation) {
			if (ActiveWindow != ChatWindows.Whispers) {
				whisperNoti++;
				whisperButtonNotification.text = whisperNoti.ToString ();
			}
			ChatConnectionManager.AllChat.PvpChat.Add (data);
		}

		else 	if (c == Commands.DominionMessage || c == Commands.DominionLocation) {
			if (ActiveWindow != ChatWindows.Dominion) {
				dominionNoti++;
				dominionButtonNotification.text = dominionNoti.ToString ();
			}
			ChatConnectionManager.AllChat.DominionChat.Add (data);
		}

		else 	if (c == Commands.NewsMessage || c == Commands.NewsLocation) {
			if (ActiveWindow != ChatWindows.News) {
				newsNoti++;
				newsButtonNotification.text = newsNoti.ToString ();
			}
			ChatConnectionManager.AllChat.News.Add (data);
		}
	}

	public void SwitchWindow (string type) 
	{
		worldButton.transform.localScale = newsButton.transform.localScale = dominionButton.transform.localScale = covenButton.transform.localScale = whisperButton.transform.localScale = Vector3.one;
		worldButton.color = newsButton.color = dominionButton.color = covenButton.color = whisperButton.color = Utilities.Grey;

	

		CovenUIText.gameObject.SetActive (false);
		inputMessage.interactable = true;
		sendButton.interactable = true;
		shareLocation.interactable = true;
		if (type == "news") {
			ActiveWindow = ChatWindows.News;
			populateChat (ChatConnectionManager.AllChat.News);
			newsButton.transform.localScale = Vector3.one * 1.2f;
			newsButton.color = Color.white;
			inputMessage.interactable = false;
			sendButton.interactable = false;
			shareLocation.interactable = false;
			newsNoti = 0;
			newsButtonNotification.text = "";
		} else if (type == "world") {
			ActiveWindow = ChatWindows.World;
			populateChat (ChatConnectionManager.AllChat.WorldChat);
			worldButton.transform.localScale = Vector3.one * 1.2f;
			worldButton.color = Color.white;
			worldNoti = 0;
			worldButtonNotification.text = "";
		} else if (type == "coven") {
			CovenUIText.gameObject.SetActive (true);
			CovenUIText.text = ChatConnectionManager.coven;
			ActiveWindow = ChatWindows.Covens;
			populateChat (ChatConnectionManager.AllChat.CovenChat);
			covenButton.transform.localScale = Vector3.one * 1.2f;
			covenButton.color = Color.white;
			covenNoti = 0;
			covenButtonNotification.text = "";
		} else if (type == "dominion") {
			ActiveWindow = ChatWindows.Dominion;
			populateChat (ChatConnectionManager.AllChat.DominionChat);
			dominionButton.transform.localScale = Vector3.one * 1.2f;
			dominionButton.color = Color.white;
			dominionNoti = 0;
			dominionButtonNotification.text = "";
		} else {
			ActiveWindow = ChatWindows.Whispers;
			populateChat (ChatConnectionManager.AllChat.PvpChat);
			whisperButton.transform.localScale = Vector3.one * 1.2f;
			whisperButton.color = Color.white;
			inputMessage.interactable = false;
			sendButton.interactable = false;
			shareLocation.interactable = false;
			whisperNoti = 0;
			whisperButtonNotification.text = "";
		}
	}

	void populateChat (List<ChatData> CD)
	{
		clearChat ();

		foreach (var item in CD) {
			AddItemHelper (item);
		}
	}

	public void AddItemHelper (ChatData CD)
	{
		CD.Command = (Commands)Enum.Parse (typeof(Commands), CD.CommandRaw);
		if (ActiveWindow == ChatWindows.Covens) {
			if (CD.Command == Commands.CovenMessage || CD.Command == Commands.CovenLocation) {
				AddItem (CD);
			}  
		} else if (ActiveWindow == ChatWindows.World) {
			if (CD.Command == Commands.WorldMessage || CD.Command == Commands.WorldLocation) {
				AddItem (CD);
			}  
		} else if (ActiveWindow == ChatWindows.News) {
			if (CD.Command == Commands.NewsMessage || CD.Command == Commands.NewsLocation) {
				AddItem (CD);
			}  
		} else if (ActiveWindow == ChatWindows.Dominion) {
			if (CD.Command == Commands.DominionMessage || CD.Command == Commands.DominionMessage) {
				AddItem (CD);
			}  
		} else {
			if (selectedPvPPlayer == null) {
				if (CD.Command == Commands.WhisperMessage || CD.Command == Commands.WhisperLocation) {

					if (CD.Name != playerName)
						CD.WhisperUIPlayer = CD.Name;

					if (CD.Receiver != playerName)
						CD.WhisperUIPlayer = CD.Receiver;

					if (CD.WhisperUIPlayer != null) {
						if (!pvpPlayerNames.Contains (CD.WhisperUIPlayer)) {
							AddItem (CD, true);
							pvpPlayerNames.Add (CD.WhisperUIPlayer);
						} else {
							if (pvpChatItems.ContainsKey (CD.WhisperUIPlayer))
								pvpChatItems [CD.WhisperUIPlayer].GetComponent<ChatItemData> ().content.text = CD.Content;
						}
					}

				} 
			} else {
				AddItem (CD);
			}
			
		}
	}

	void AddItem (ChatData CD , bool isPVP = false)
	{
		GameObject chatObject = null;
		if (CD.Command == Commands.CovenMessage || CD.Command == Commands.NewsMessage || CD.Command == Commands.WorldMessage || CD.Command == Commands.DominionMessage || CD.Command == Commands.WhisperMessage) {
			chatObject = Utilities.InstantiateObject (chatPrefab, container);
			chatObject.GetComponent<ChatItemData> ().Setup (CD, false);
		} else {
			chatObject = Utilities.InstantiateObject (locationPrefab, container);
			chatObject.GetComponent<ChatItemData> ().Setup (CD, true);
		}
		if (isPVP) {
			pvpChatItems.Add (CD.WhisperUIPlayer, chatObject);

		}
		chatItems.Add (chatObject);
	}

	void clearChat ()
	{
		pvpPlayerNames.Clear ();
		pvpChatItems.Clear ();	
		foreach (var item in chatItems) {
			Destroy (item);
		}
		chatItems.Clear ();
	}

	public void ShowPvP (ChatData CD)
	{
		clearChat ();
		inputMessage.interactable = true;
		sendButton.interactable = true;
		shareLocation.interactable = true;
		if (CD.Name != playerName)
			selectedPvPPlayer = CD.Name;
		else
			selectedPvPPlayer = CD.Receiver;
		
		PvPHeader.SetActive (true);
		Header.SetActive (false);
		PvPName.text = CD.Name + " (Level " + CD.Level.ToString () + ")";
		PvPDegree.text = Utilities.witchTypeControl (CD.Degree); 
		foreach (var item in ChatConnectionManager.AllChat.PvpChat) {

			if (item.Name == selectedPvPPlayer || item.Receiver == selectedPvPPlayer) {
				AddItemHelper (item);
			}
		
		}
	}

	public void hidePvP ()
	{
		selectedPvPPlayer = null;
		PvPHeader.SetActive (false);
		Header.SetActive (true);
		SwitchWindow ("whispers");
	}

	public void SendMessage ()
	{
		if (!inputMessage.text.IsNullOrWhiteSpace ()) {
			sendButton.interactable = false;
			shareLocation.interactable = false;

			ChatData CD = new ChatData ();
			CD.Avatar = 3;
			CD.Name = playerName;
			CD.Content = inputMessage.text;
			CD.Degree = -2;
			CD.Level = 6;
			if (ActiveWindow == ChatWindows.World) {
				CD.CommandRaw = Commands.WorldMessage.ToString ();
			} else if (ActiveWindow == ChatWindows.Covens) {
				CD.CommandRaw = Commands.CovenMessage.ToString ();
				CD.Coven = ChatConnectionManager.coven;
			} else if (ActiveWindow == ChatWindows.Dominion) {
				CD.CommandRaw = Commands.DominionMessage.ToString ();
				CD.Dominion = ChatConnectionManager.dominion;
			} else if (ActiveWindow == ChatWindows.Whispers) {
				CD.Receiver = selectedPvPPlayer;
				CD.CommandRaw = Commands.WhisperMessage.ToString ();
			}
			inputMessage.Select ();
			inputMessage.text = "";
			print ("send");
			ChatConnectionManager.Instance.send (CD);
			StartCoroutine (ReEnableSendButton ());
		}
	}

	IEnumerator ReEnableSendButton ()
	{
		yield return new WaitForSeconds (1.5f);
		sendButton.interactable = true;
		shareLocation.interactable = true;
	}

	void Update ()
	{
		if (Input.GetKeyUp (KeyCode.Return) && inputMessage.IsInteractable ()) {
			SendMessage ();
		}
	}
}

