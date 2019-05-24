using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;


public class ChatItemData : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI degree;
    public TextMeshProUGUI content;
    public TextMeshProUGUI languageType;
    public TextMeshProUGUI timeStamp;
    public Image profilePic;
    public Image alignment;
    public Button translateButton;
    public Sprite[] chatHead;
    public int avatar;
    public Button playerDetail;
    ChatData CD;

    public void Setup(ChatData data, bool isLocation)
    {
        CD = data;
        timeStamp.text = Utilities.EpocToDateTimeChat(data.TimeStamp);

        if (data.Command == Commands.HelpCrowMessage)
        {
            content.text = data.Content;
            return;
        }
        playerDetail.onClick.AddListener(OnSelectPlayer);
        //if is player
        if (data.Avatar >= 0)
        {
            if (data.CommandRaw.Contains("News"))
                playerName.text = data.Title;
            else
                playerName.text = data.Name + "(level" + CD.Level.ToString() + ")";
            avatar = data.Avatar;
            profilePic.sprite = chatHead[data.Avatar];
            degree.text = Utilities.witchTypeControlSmallCaps(CD.Degree);

            if (data.Degree > 0)
                alignment.color = Utilities.Orange;
            else if (data.Degree < 0)
                alignment.color = Utilities.Purple;
            else
                alignment.color = Utilities.Grey;
        }
        else //generic chat items
        {
            if (string.IsNullOrEmpty(data.Name))
            {
                playerName.gameObject.SetActive(false);
            }
            else
            {
                playerName.text = data.Name;
            }

            degree.gameObject.SetActive(false);
            alignment.gameObject.SetActive(false);
        }

        if (!isLocation)
        {
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
        }

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
        PlayerManager.Instance.FlyTo(CD.Longitude, CD.Latitude);
        ChatUI.Instance.HideChat();

        //if (PlayerDataManager.playerData.energy == 0)
        //    return;

        //Vector2 p = new Vector2((float)CD.Longitude, (float)CD.Latitude);
        //Vector2 playerPos = PlayerManager.marker.coords;

        //if (MapsAPI.Instance.DistanceBetweenPointsD(p, playerPos) > 0.05f)
        //{
        //    MapsAPI.Instance.SetPosition();
        //    MarkerManagerAPI.GetMarkers(false, true, null, true);
        //    ChatUI.Instance.HideChat();
        //}
        //else
        //{
        //    ChatUI.Instance.HideChat();
        //    Vector3 worldPos = MapsAPI.Instance.GetWorldPosition(CD.Longitude, CD.Latitude);
        //    MapCameraUtils.SetPosition(worldPos, 1f, true);
        //}
    }

    void OnSelectPlayer()
    {
        ChatUI.Instance.GetPlayerDetails(CD.Name);
    }
}

