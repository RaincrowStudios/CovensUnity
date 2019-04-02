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
        //		shoutButton.gameObject.SetActive (true);
        inputField.gameObject.SetActive(false);
        sendButton.SetActive(false);
        var data = new { shout = inputField.text };
        APIManager.Instance.PostData("/map/shout", JsonConvert.SerializeObject(data), ReceiveData);
        StartCoroutine(ReEnableSendButton());
    }

    public void ReceiveData(string response, int code)
    {
        if (code == 200)
        {
            Debug.Log("success");
        }
    }

    IEnumerator ReEnableSendButton()
    {
        yield return new WaitForSeconds(5);
        shoutButton.interactable = true;
    }


}

