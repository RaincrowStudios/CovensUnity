using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class ChatUI : UIAnimationManager
{
	public static ChatUI Instance { get; set; }

	public Text worldButton;
	public Text newsButton;
	public Text dominionButton;
	public Text covenButton;
	public Text CovenUIText;

	public Text worldButtonNotification;
	public Text newsButtonNotification;
	public Text dominionButtonNotification;
	public Text covenButtonNotification;

	public List<GameObject> chatItems = new List<GameObject> ();

	public Sprite[] profilePics;
	public GameObject locationPrefab;
	public GameObject chatPrefab;
	public Transform container;
	public GameObject ChatParentObject;

	public GameObject Header;
	public static string playerName = "grim";

	public InputField inputMessage;
	public Button shareLocation;
	public Button sendButton;

	public Sprite[] chatHeads;

	int newsNoti,worldNoti,covenNoti,dominionNoti =0; 

	public static int currentCount=0;
	public static bool isWorld = true;

	public Animator anim;
	public enum ChatWindows
	{
		News,
		World,
		Covens,
		Dominion,
	};

	public ChatWindows ActiveWindow = ChatWindows.World;

	void Awake ()
	{
		Instance = this;
	}

	public void Init ()
	{
		SwitchWindow ("world");
	}

	public void initNotifications()
	{
		worldButtonNotification.text = covenButtonNotification.text  = newsButtonNotification.text = dominionButtonNotification.text = "";
		newsNoti = worldNoti = covenNoti = dominionNoti =  0;
	}

	public void addNotification( ChatData data)
	{
		var c = (Commands)Enum.Parse (typeof(Commands),data.CommandRaw);
		data.Command = c;
//		print ("Adding stuff");
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
		currentCount = 0;
		worldButton.transform.localScale = newsButton.transform.localScale = dominionButton.transform.localScale = covenButton.transform.localScale  = Vector3.one;
		worldButton.color = newsButton.color = dominionButton.color = covenButton.color = Utilities.Grey;

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
			isWorld = true;
			ActiveWindow = ChatWindows.World;
			populateChat (ChatConnectionManager.AllChat.WorldChat);
			worldButton.transform.localScale = Vector3.one * 1.2f;
			worldButton.color = Color.white;
			worldNoti = 0;
			worldButtonNotification.text = "";

		} else if (type == "coven") {
			isWorld = false;
			CovenUIText.gameObject.SetActive (true);
			if (PlayerDataManager.playerData.covenName != "") {
				CovenUIText.text = PlayerDataManager.playerData.covenName;
				populateChat (ChatConnectionManager.AllChat.CovenChat);

			} else {
				CovenUIText.text = "No Coven";
				clearChat ();
				inputMessage.interactable = false;
				sendButton.interactable = false;
				shareLocation.interactable = false;
			}
			ActiveWindow = ChatWindows.Covens;
			covenButton.transform.localScale = Vector3.one * 1.2f;
			covenButton.color = Color.white;
			covenNoti = 0;
			covenButtonNotification.text = "";

		} else {
			ActiveWindow = ChatWindows.Dominion;
			populateChat (ChatConnectionManager.AllChat.DominionChat);
			dominionButton.transform.localScale = Vector3.one * 1.2f;
			dominionButton.color = Color.white;
			dominionNoti = 0;
			dominionButtonNotification.text = "";
		} 
	}

	void populateChat (List<ChatData> CD)
	{
		clearChat ();
		if (CD == null)
			return;
		foreach (var item in CD) {
			
			AddItemHelper (item);
		}
	}

	public void AddItemHelper (ChatData CD)
	{
		#region newsScroll
		if (CD.Command == Commands.CovenMessage) {
			NewsScroll.Instance.ShowText ("(Coven) " + CD.Name + " : " + CD.Content,true);
		} else if(CD.Command == Commands.CovenLocation) {
			NewsScroll.Instance.ShowText ("(Coven) " + CD.Name + " shared location.",true);
		}else if (CD.Command == Commands.WorldMessage) {
			NewsScroll.Instance.ShowText ("(World) " + CD.Name + " : " + CD.Content,true);
		}else if(CD.Command == Commands.WorldLocation){
			NewsScroll.Instance.ShowText ("(World) " + CD.Name + " shared location.",true);
		}else if(CD.Command == Commands.NewsMessage){
			NewsScroll.Instance.ShowText ("(News) " +  CD.Content,true);
		}else if (CD.Command == Commands.DominionMessage) {
			NewsScroll.Instance.ShowText ("("+CD.Dominion+") " + CD.Name + " : " + CD.Content,true);
		} else  if (CD.Command == Commands.DominionLocation){
			NewsScroll.Instance.ShowText ("("+CD.Dominion+") " + CD.Name + " shared location.",true);
		}
		#endregion

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
		} else {
			if (CD.Command == Commands.DominionMessage || CD.Command == Commands.DominionLocation) {
				AddItem (CD);
			
			}  
		}
	}

	void AddItem (ChatData CD , bool isPVP = false)
	{
		GameObject chatObject = null;
		if (CD.Command == Commands.CovenMessage || CD.Command == Commands.NewsMessage || CD.Command == Commands.WorldMessage || CD.Command == Commands.DominionMessage) {
			chatObject = Utilities.InstantiateObject (chatPrefab, container);
			chatObject.GetComponent<ChatItemData> ().Setup (CD, false);
			currentCount++;
		} else {
			chatObject = Utilities.InstantiateObject (locationPrefab, container);
			chatObject.GetComponent<ChatItemData> ().Setup (CD, true);
		}
		chatItems.Add (chatObject); 
	}

	void clearChat ()
	{
		foreach (var item in chatItems) {
			Destroy (item);
		}
		chatItems.Clear ();
	}

	public void SendMessage ()
	{
		if (!inputMessage.text.IsNullOrWhiteSpace ()) {
			sendButton.interactable = false;
			shareLocation.interactable = false;

			ChatData CD = new ChatData ();
			CD.Avatar = 3;
			CD.Name = PlayerDataManager.playerData.displayName;
			CD.Content = inputMessage.text;
			CD.Degree = PlayerDataManager.playerData.degree;
			CD.Level = PlayerDataManager.playerData.level;
			if (ActiveWindow == ChatWindows.World) {
				CD.CommandRaw = Commands.WorldMessage.ToString ();
			} else if (ActiveWindow == ChatWindows.Covens) {
				CD.CommandRaw = Commands.CovenMessage.ToString ();
				CD.Coven = PlayerDataManager.playerData.covenName;
			} else if (ActiveWindow == ChatWindows.Dominion) {
				CD.CommandRaw = Commands.DominionMessage.ToString ();
				CD.Dominion = PlayerDataManager.currentDominion;
			} 
//			inputMessage.Select ();
			inputMessage.text = "";
			ChatConnectionManager.Instance.send (CD);
			StartCoroutine (ReEnableSendButton ()); 
		}
	}

	public void SendLocation ()
	{
		sendButton.interactable = false;
		shareLocation.interactable = false;
			ChatData CD = new ChatData ();
			CD.Name = PlayerDataManager.playerData.displayName;
			CD.Degree = PlayerDataManager.playerData.degree;
			CD.Level = PlayerDataManager.playerData.level;
			CD.Latitude = OnlineMaps.instance.position.y;
			CD.Longitude = OnlineMaps.instance.position.x;
			if (ActiveWindow == ChatWindows.World) {
				CD.CommandRaw = Commands.WorldLocation.ToString ();
			} else if (ActiveWindow == ChatWindows.Covens) {
				CD.CommandRaw = Commands.CovenLocation.ToString ();
			CD.Coven = PlayerDataManager.playerData.covenName;
			} else if (ActiveWindow == ChatWindows.Dominion) {
				CD.CommandRaw = Commands.DominionLocation.ToString ();
				CD.Dominion = PlayerDataManager.currentDominion;
			} 
//			inputMessage.Select ();
//			inputMessage.text = "";
			print (JsonConvert.SerializeObject (CD));
			ChatConnectionManager.Instance.send (CD);
			StartCoroutine (ReEnableSendButton ());
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

	public void ShowChat()
	{
		UIStateManager.Instance.CallWindowChanged(false);
		ChatParentObject.SetActive (true);
		anim.SetBool ("animate", true);
	}

	public void HideChat()
	{
		UIStateManager.Instance.CallWindowChanged(true);
		anim.SetBool ("animate", false);
	}


}

