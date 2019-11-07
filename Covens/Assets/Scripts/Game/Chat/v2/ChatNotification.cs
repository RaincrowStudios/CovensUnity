using Raincrow.Chat;
using TMPro;
using UnityEngine;

public class ChatNotification : MonoBehaviour
{
    public static ChatNotification Instance { get; set; }
    private int messageCount;
    public GameObject notifcationContainer;
    public TextMeshProUGUI notification;
    public GameObject msgContainer;
    public TextMeshProUGUI oldMsg;
    public TextMeshProUGUI newMsg;



    void Awake()
    {
        Instance = this;
        messageCount = 0;
        notifcationContainer.SetActive(false);
    }

    public void OnMessage(ChatMessage msg, ChatCategory chatCategory)
    {
        messageCount++;
        if (!notifcationContainer.activeInHierarchy)
            notifcationContainer.SetActive(true);
        notification.text = messageCount > 9 ? "9+" : messageCount.ToString();
        msgContainer.SetActive(false);
        oldMsg.text = newMsg.text;

        if (msg.type == MessageType.TEXT)
        {
            oldMsg.text = $"{GetType(chatCategory)} {msg.player.name}: {msg.data.message}";
        }
        else if (msg.type == MessageType.LOCATION)
        {
            oldMsg.text = $"{GetType(chatCategory)} {msg.player.name}: {LocalizeLookUp.GetText("chat_share_location")}";
        }
        else if (msg.type == MessageType.IMAGE)
        {
            oldMsg.text = $"{GetType(chatCategory)} {msg.player.name}: Shared an image.";
        }

        msgContainer.SetActive(false);
    }

    private string GetType(ChatCategory chatCategory = ChatCategory.WORLD)
    {
        if (chatCategory == ChatCategory.DOMINION)
            return "(" + LocalizeLookUp.GetText("chat_dominion") + ")";
        else if (chatCategory == ChatCategory.COVEN)
            return "(" + LocalizeLookUp.GetText("chat_coven") + ")";
        else if (chatCategory == ChatCategory.SUPPORT)
            return "(HelpCrow)";
        else
            return "(" + LocalizeLookUp.GetText("chat_world") + ")";
    }

    public void SetStartMessage(ChatMessage msg)
    {
        if (msg.type == MessageType.TEXT)
        {
            oldMsg.text = $"{GetType()} {msg.player.name}: {msg.data.message}";
        }
        else if (msg.type == MessageType.LOCATION)
        {
            oldMsg.text = $"{GetType()} {msg.player.name}: {LocalizeLookUp.GetText("chat_share_location")}";
        }
        else if (msg.type == MessageType.IMAGE)
        {
            oldMsg.text = $"{GetType()} {msg.player.name}: Shared an image.";
        }
    }

    public void ClearNotification()
    {
        messageCount = 0;
        notifcationContainer.SetActive(false);
    }
}