using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow;
using Raincrow.Maps;
using UnityEngine;

public class LoadPOPManager : MonoBehaviour
{

    private IMaps map;
    public GameObject[] MainUIDisable;
    void OnGUI()
    {
        if (GUI.Button(new Rect(200, 10, 90, 40), "Load Pop"))
        {
            if (map == null)
            {
                map = MapsAPI.Instance;
            }
            // foreach (var item in MainUIDisable)
            // {
            //     item.SetActive(false);
            // }
            // map.HideMap(true);
            // var k = new { id = "5d49893f1df43058b9f42a93" };
            APIManager.Instance.Get("place-of-power/view/5d4c830601f40c2379dfefd2", (response, result) =>
            {
                Debug.Log(response);
                if (result == 200)
                {
                    var popInfo = LocationPOPInfo.Instance;
                    popInfo.Show(JsonConvert.DeserializeObject<LocationViewData>(response));
                }
                else
                {
                    Debug.Log(result);
                }

            });

            // SceneManager.LoadSceneAsync(SceneManager.Scene.PLACE_OF_POWER, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            // {
            //     LocationIslandController.instance.Initiate();
            // });
        }
    }
}
