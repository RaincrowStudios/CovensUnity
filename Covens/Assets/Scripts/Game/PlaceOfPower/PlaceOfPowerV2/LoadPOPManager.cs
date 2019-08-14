using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow;
using Raincrow.Maps;
using UnityEngine;

public class LoadPOPManager : MonoBehaviour
{
    private static LoadPOPManager Instance { get; set; }
    private IMaps map;
    public GameObject[] MainUIDisable;
    void Awake()
    {
        Instance = this;
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(320, 10, 40, 40), "View"))
        {
            if (map == null)
            {
                map = MapsAPI.Instance;
            }
            APIManager.Instance.Get("place-of-power/view/5d54431515c8ee22cbd6b991", (response, result) =>
            {
                Debug.Log(response);
                if (result == 200)
                {
                    var popInfo = LocationPOPInfo.Instance;
                    var data = JsonConvert.DeserializeObject<LocationViewData>(response);
                    data.battleBeginsOn = GetFakeTime();
                    Debug.Log(GetFakeTime());
                    Debug.Log(data.battleBeginsOn);
                    popInfo.Show(data);
                }
                else
                {
                    Debug.Log(result);
                }

            });
        }

        if (GUI.Button(new Rect(275, 10, 40, 40), "Exit"))
        {
            LocationIslandController.ExitPOP();
        }

        if (GUI.Button(new Rect(365, 10, 40, 40), "Start"))
        {
            if (map == null)
            {
                map = MapsAPI.Instance;
            }
            APIManager.Instance.Post("place-of-power/start/5d54431515c8ee22cbd6b991", "{}", (response, result) =>
             {
                 Debug.Log(response);
             });
        }
    }


    public static void LoadScene(System.Action onComplete)
    {
        foreach (var item in Instance.MainUIDisable)
        {
            item.SetActive(false);
        }
        Instance.map.HideMap(true);
        SceneManager.LoadSceneAsync(SceneManager.Scene.PLACE_OF_POWER, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, onComplete);
    }

    private static double GetFakeTime()
    {
        System.DateTime dtDateTime = System.DateTime.UtcNow;
        dtDateTime = dtDateTime.AddSeconds(30);
        return ((System.DateTimeOffset)dtDateTime).ToUnixTimeMilliseconds();
    }
}
