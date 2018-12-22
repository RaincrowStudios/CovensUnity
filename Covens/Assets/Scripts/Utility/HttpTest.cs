using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json;

public class HttpTest : MonoBehaviour
{

    // Use this for initialization


    IEnumerator CreateAccount()
    {

        var data = new PlayerLoginAPI();
        data.username = UnityEngine.Random.Range(999999, 9999999999).ToString();
        data.password = "1223";
        data.email = data.username + "@gmail.com";
        data.game = "covens";
        data.language = Application.systemLanguage.ToString();
        data.latitude = 0;
        data.longitude = 0;
        data.UID = SystemInfo.deviceUniqueIdentifier + data.username;


        UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddress + "raincrow/create-account", JsonConvert.SerializeObject(data));
        www.SetRequestHeader("Content-Type", "application/json");
        print("Sending Data : " + data);
        print(Constants.hostAddress + "create-account");

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error + www.responseCode.ToString());
        }
        else
        {
            print(www.GetRequestHeader("HTTP-date"));
            print(www.responseCode.ToString());
            print("Received response : " + www.downloadHandler.text);
        }

    }



}

