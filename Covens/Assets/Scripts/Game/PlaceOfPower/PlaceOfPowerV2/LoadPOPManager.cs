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
    bool isViewVisible = false;
    static bool sceneLoaded = false;
    private static int previousEnergy = 0;
    private static string previousState = "";
    void Awake()
    {
        Instance = this;
    }
    void OnGUI()
    {
        if (!isViewVisible && GUI.Button(new Rect(320, 10, 80, 40), "View POP"))
        {
            if (map == null)
            {
                map = MapsAPI.Instance;
            }
            LocationIslandController.ExitPOP(() =>
            {
                APIManager.Instance.Get("place-of-power/view/5d56ebd18aca90617dc3b5fa", (response, result) =>
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
                              isViewVisible = true;
                          }
                          else
                          {
                              Debug.Log(result);
                          }

                      });
            });
        }



        if (sceneLoaded && !LocationIslandController.isInBattle && GUI.Button(new Rect(365, 10, 160, 40), "Start POP Battle"))
        {
            if (map == null)
            {
                map = MapsAPI.Instance;
            }
            APIManager.Instance.Post("place-of-power/start/5d56ebd18aca90617dc3b5fa", "{}", (response, result) =>
             {
                 Debug.Log(response);
             });
        }
    }


    public static void LoadScene(System.Action onComplete)
    {
        previousEnergy = PlayerDataManager.playerData.energy;
        previousState = PlayerDataManager.playerData.state;

        foreach (var item in Instance.MainUIDisable)
        {
            item.SetActive(false);
        }
        Instance.map.HideMap(true);
        SceneManager.LoadSceneAsync(SceneManager.Scene.PLACE_OF_POWER, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
        {
            sceneLoaded = true;
            onComplete();
        });
    }

    public static void UnloadScene()
    {

        foreach (var item in Instance.MainUIDisable)
        {
            item.SetActive(true);
        }
        Instance.map.HideMap(false);
        LocationIslandController.BattleStopPOP();
        LocationUnitSpawner.UnloadScene();
        SceneManager.UnloadScene(SceneManager.Scene.PLACE_OF_POWER, null, () =>
       {
           sceneLoaded = false;
           var t = LocationExitInfo.Instance;
           UIQuickCast.Close();
           t.ShowUI();
           PlayerDataManager.playerData.energy = previousEnergy;
           PlayerDataManager.playerData.state = previousState;
           //onComplete();
       });
        OnMapEnergyChange.OnPlayerDead -= LoadPOPManager.UnloadScene;
        OnMapEnergyChange.OnMarkerEnergyChange -= LocationUnitSpawner.OnEnergyChange;
    }

    private static double GetFakeTime()
    {
        System.DateTime dtDateTime = System.DateTime.UtcNow;
        dtDateTime = dtDateTime.AddSeconds(30);
        return ((System.DateTimeOffset)dtDateTime).ToUnixTimeMilliseconds();
    }

    public static void HandleQuickCastOpen(System.Action OnOpen)
    {
        UIQuickCast.Open(() =>
        {
            if (LocationIslandController.isInBattle)
            {
                OnOpen();
            }
        });

    }
}
