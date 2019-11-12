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

        inputField.onSubmit.AddListener(InputField_OnSubmit);
        inputField.text = "";
    }

    private void InputField_OnSubmit(string msg)
    {
        OnClickSend();
    }

    private void Open()
    {
        if (m_IsShowing)
            return;

        inputField.gameObject.SetActive(true);
        sendButton.gameObject.SetActive(true);
        m_IsShowing = true;

        BackButtonListener.AddCloseAction(OnPressBack);
    }

    private void Close()
    {
        if (m_IsShowing == false)
            return;

        inputField.text = "";

        inputField.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
        m_IsShowing = false;

        BackButtonListener.RemoveCloseAction();
    }

    public void OnClickShout()
    {
        if (FirstTapManager.IsFirstTime("shout"))
        {
            FirstTapManager.Show("shout", OnClickShout);
            return;
        }

        if (m_IsShowing)
            Close();
        else
            Open();
    }

    public void OnClickSend()
    {
        string message = inputField.text;

        if (string.IsNullOrWhiteSpace(message) == false)
        {
            string data = $"{{\"message\":\"{message}\"}}";
            
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

        Close();
    }

    IEnumerator ReEnableSendButton()
    {
        yield return new WaitForSeconds(2);
        shoutButton.interactable = true;
    }

    private void OnPressBack()
    {
        Close();
    }
}

