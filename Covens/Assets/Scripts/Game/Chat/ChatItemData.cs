﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

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
    public Button playerDetail;
    ChatData CD;

    public void Setup(ChatData data, bool isLocation)
    {
        CD = data;

        timeStamp.text = Utilities.EpocToDateTimeChat(data.TimeStamp);
        playerDetail.onClick.AddListener(OnSelectPlayer);
        //if is player
        if (data.Avatar >= 0)
        {
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
        if (PlayerDataManager.playerData.energy == 0)
            return;
        PlayerManager.Instance.Fly();
        MapsAPI.Instance.SetPosition(CD.Longitude, CD.Latitude);
        PlayerManager.inSpiritForm = false;
        PlayerManager.Instance.Fly();
        ChatUI.Instance.HideChat();
    }

    void OnSelectPlayer()
    {
        ChatUI.Instance.GetPlayerDetails(CD.Name);
    }
}

