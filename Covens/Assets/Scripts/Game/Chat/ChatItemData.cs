using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatItemData : MonoBehaviour
{
    public Text playerName;
    public Text degree;
    public Text content;
    public Text languageType;
    public Text timeStamp;
    public Image profilePic;
    public Image alignment;
    public Button translateButton;
    public Sprite[] chatHead;
    public int avatar;
    ChatData CD;
    public void Setup(ChatData data, bool isLocation)
    {
        CD = data;
        timeStamp.text = Utilities.EpocToDateTimeChat(data.TimeStamp);
        avatar = data.Avatar;
        profilePic.sprite = chatHead[data.Avatar];
        playerName.text = data.Name + "(level" + CD.Level.ToString() + ")";
        degree.text = Utilities.witchTypeControlSmallCaps(CD.Degree);
        if (data.Language != LoginAPIManager.systemLanguage)
        {
            translateButton.gameObject.SetActive(true);
            languageType.text = "( from " + data.Language + " )";
        }
        else
        {
            translateButton.gameObject.SetActive(false);
            languageType.text = "";
        }
        if (data.Degree > 0)
            alignment.color = Utilities.Orange;
        else if (data.Degree < 0)
            alignment.color = Utilities.Purple;
        else
            alignment.color = Utilities.Grey;
        if (!isLocation)
        {
            content.text = data.Content;
        }
        else
        {
            // add location logic
        }

    }

    void kill()
    {
        Destroy(gameObject);
    }

    public void TranslateText()
    {
        ChatUI.Instance.SendTranslate(content.text, OnTranslateComplete);
    }

    public void OnTranslateComplete(string text)
    {
        if (text == "null")
        {
            translateButton.interactable = false;
            translateButton.GetComponentInChildren<Text>().text = "Failed";
        }
        else
        {
            content.text = text;
            translateButton.gameObject.SetActive(false);
        }
    }

    public void MoveToPos()
    {
        if (PlayerDataManager.playerData.energy == 0)
            return;
        PlayerManager.Instance.Fly();
        OnlineMaps.instance.SetPosition(CD.Longitude, CD.Latitude);
        PlayerManager.inSpiritForm = false;
        PlayerManager.Instance.Fly();
        ChatUI.Instance.HideChat();
    }
}

