using Raincrow.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewsScroll : MonoBehaviour
{
	public static NewsScroll Instance { get; set;}

	public Text Previous;
	public Text New;
	public GameObject scrollContainer;
	public Image IconOld;
	public Image IconNew;

	public Sprite InfoIcon;
	public Sprite ChatIcon;

    [SerializeField] private GameObject m_NewMessagesObj;
    [SerializeField] private TextMeshProUGUI m_NewMessagesText;

	string curText ="";
	Sprite curSp;

	void Awake()
	{
		Instance = this;
		Previous.text = "";
		New.text = "";

        m_NewMessagesText.text = "";
        m_NewMessagesObj.SetActive(false);

        ChatManager.OnReceiveMessage += OnReceiveMessage;
        ChatManager.OnResetNewMsgCount += UpdateNewMsgCount;
	}

    private void OnDestroy()
    {
        ChatManager.OnReceiveMessage -= OnReceiveMessage;
        ChatManager.OnResetNewMsgCount -= UpdateNewMsgCount;
    }

    private void UpdateNewMsgCount()
    {
        int newMsgCount =
              ChatManager.NewMessagesCount(ChatCategory.NEWS) +
              ChatManager.NewMessagesCount(ChatCategory.WORLD) +
              ChatManager.NewMessagesCount(ChatCategory.COVEN) +
              ChatManager.NewMessagesCount(ChatCategory.DOMINION);

        m_NewMessagesText.text = newMsgCount > 9 ? "9+" : newMsgCount.ToString();
        m_NewMessagesObj.SetActive(newMsgCount > 0);
    }

    private void OnReceiveMessage(ChatCategory category, ChatMessage message)
    {
        string msg = "(";
        switch (category)
        {
            case ChatCategory.COVEN: msg += PlayerDataManager.playerData.covenInfo.name; break;
            case ChatCategory.WORLD: msg += LocalizeLookUp.GetText("chat_world"); break;
            case ChatCategory.DOMINION: msg += PlayerDataManager.playerData.dominion; break;
            case ChatCategory.NEWS: msg += LocalizeLookUp.GetText("chat_news"); break;
        }
        msg += ") " + message.player.name + ": ";

        switch(message.type)
        {
            case MessageType.TEXT: msg += message.data.message; break;
            case MessageType.LOCATION: msg += LocalizeLookUp.GetText("chat_share_location"); break;
            case MessageType.IMAGE: msg += LocalizeLookUp.GetText("chat_share_image"); break;
        }

        ShowText(msg, true);

        UpdateNewMsgCount();
    }

    public void ShowText(string text, bool isChat = false)
	{
		this.CancelInvoke ();
		Animate ();
		New.text = text;
		curText = text;
		if (!isChat) {
			IconNew.sprite = InfoIcon;
			curSp = InfoIcon; 
		} else {
			IconNew.sprite = ChatIcon;
			curSp = ChatIcon; 
		}
		Invoke ("AddOldText", .28f);
	}

	void AddOldText()
	{
		Previous.text = curText;
		IconOld.sprite = curSp; 
	}

	void Animate()
	{
		scrollContainer.SetActive (false);
		scrollContainer.SetActive (true);
	}
}
