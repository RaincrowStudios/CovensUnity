using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using Raincrow.FTF;
using TMPro;

public class ShoutBox : MonoBehaviour
{
    public Button shoutButton;
    public Button sendButton;
    public TMP_InputField inputField;

    private bool m_IsShowing = false;

    private void Awake()
    {
        shoutButton.onClick.AddListener(OnClickShout);
        sendButton.onClick.AddListener(OnClickSend);

        inputField.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
        shoutButton.gameObject.SetActive(true);
    }

    public void OnClickShout()
    {
        if (FirstTapManager.IsFirstTime("shout"))
        {
            FirstTapManager.Show("shout", OnClickShout);
            return;
        }

        if (!m_IsShowing)
        {
            inputField.gameObject.SetActive(true);
            sendButton.gameObject.SetActive(true);
        }
        else
        {
            inputField.gameObject.SetActive(false);
            sendButton.gameObject.SetActive(false);
        }

        m_IsShowing = !m_IsShowing;
        inputField.text = "";
    }

    public void OnClickSend()
    {
        inputField.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
        m_IsShowing = false;

        string message = inputField.text;
        string data = $"{{\"message\":\"{message}\"}}";
        inputField.text = "";

        APIManager.Instance.Post("character/shout", data, (response, result) =>
        {
            if (result == 200)
            {
                ShoutHandler.SpawnShoutbox(PlayerDataManager.playerData.instance, message);
            }
            else
            {
                Debug.LogError("shout error\n" + response);
            }
        });

        shoutButton.interactable = false;
        StartCoroutine(ReEnableSendButton());
    }

    IEnumerator ReEnableSendButton()
    {
        yield return new WaitForSeconds(2);
        shoutButton.interactable = true;
    }
}

