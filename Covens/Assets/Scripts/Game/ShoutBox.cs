using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;

public class ShoutBox : MonoBehaviour
{

    public Button shoutButton;
    public GameObject sendButton;
    public InputField inputField;

    bool show = false;
    public void OnShout()
    {
        if (!show)
        {
            //			shoutButton.gameObject.SetActive (false);
            inputField.gameObject.SetActive(true);
            sendButton.SetActive(true);
            //			shoutButton.interactable = false;
        }
        else
        {
            inputField.gameObject.SetActive(false);
            sendButton.SetActive(false);
        }

        show = !show;
    }

    public void OnSend()
    {
        inputField.gameObject.SetActive(false);
        sendButton.SetActive(false);

        string data = $"{{\"message\":\"{inputField.text}\"}}";

        APIManager.Instance.Post("character/shout", data, (response, result) =>
        {
            if (result == 200)
            {

            }
            else
            {
                Debug.LogError("shout error\n" + response);
            }
        });
        StartCoroutine(ReEnableSendButton());
    }

    IEnumerator ReEnableSendButton()
    {
        yield return new WaitForSeconds(5);
        shoutButton.interactable = true;
    }


}

