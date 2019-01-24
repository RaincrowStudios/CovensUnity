using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMainScene : MonoBehaviour
{
    private void Start()
    {
        LoginAPIManager.sceneLoaded = true;

        if (!LoginAPIManager.loggedIn)
        {
            MapsAPI.Instance.transform.GetComponent<MeshRenderer>().enabled = false;
            LoginUIManager.Instance.ShowHome();
        }
        else
        {
            if (!LoginAPIManager.hasCharacter)
            {
                LoginUIManager.Instance.ShowCreateCharacter();
            }
            else
            {
                LoginAPIManager.InitiliazingPostLogin();

                if (PlayerDataManager.playerData.energy == 0)
                {
                    DeathState.Instance.ShowDeath();
                }
                Invoke("enableSockets", 2f);
            }
        }
    }

    void enableSockets()
    {
        WebSocketClient.websocketReady = true;
    }
}
