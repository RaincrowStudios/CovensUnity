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
              ChatManager.UnreadMessageCount(ChatCategory.NEWS) +
              ChatManager.UnreadMessageCount(ChatCategory.WORLD) +
              ChatManager.UnreadMessageCount(ChatCategory.COVEN) +
              ChatManager.UnreadMessageCount(ChatCategory.DOMINION);

        m_NewMessagesText.text = newMsgCount > 9 ? "9+" : newMsgCount.ToString();
        m_NewMessagesObj.SetActive(newMsgCount > 0);
    }

    private void OnReceiveMessage(ChatCategory category, ChatMessage message)
    {
        string channel;
        switch (category)
        {
            case ChatCategory.COVEN:    channel = PlayerDataManager.playerData.covenInfo.name;  break;
            case ChatCategory.WORLD:    channel = LocalizeLookUp.GetText("chat_world");         break;
            case ChatCategory.DOMINION: channel = PlayerDataManager.playerData.dominion;        break;
            case ChatCategory.NEWS:     channel = LocalizeLookUp.GetText("chat_news");          break;
            case ChatCategory.SUPPORT:  channel = LocalizeLookUp.GetText("generic_helpcrow");   break;
            default:                    channel = null; break;
        }

        string name;
        switch (message.type)
        {
            case MessageType.TEXT:
            case MessageType.LOCATION:
            case MessageType.IMAGE:
                name = message.player.name;
                break;
            case MessageType.BOSS:
            case MessageType.NPC:
                name = LocalizeLookUp.GetText(message.data.name + "_name");
                break;
            default: name = null;
                break;
        }

        string content;
        switch (message.type)
        {
            case MessageType.TEXT:      content = message.data.message; break;
            case MessageType.LOCATION:  content = LocalizeLookUp.GetText("chat_share_location"); break;
            case MessageType.IMAGE:     content = LocalizeLookUp.GetText("chat_share_image"); break;
            case MessageType.BOSS:
            case MessageType.NPC:       content = Raincrow.Chat.UI.UIChatNpc.GetLocalizedMessage(message); break;
            default:                    content = null; break;
        }

        string msg = "";

        if (channel != null)
            msg += $"[{channel}] ";

        if (name != null)
            msg += $"{name}: ";

        if (content != null)
            msg += content;

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
