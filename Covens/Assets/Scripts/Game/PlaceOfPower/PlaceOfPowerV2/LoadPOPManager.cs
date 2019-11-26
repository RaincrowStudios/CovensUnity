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
        Debug.Log("LOADING POP MANAGER");
    }
    void OnGUI()
    {
        // if (!isViewVisible && GUI.Button(new Rect(320, 10, 80, 40), "View POP"))
        // {
        //     NewMethod();
        // }



        // if (sceneLoaded && !LocationIslandController.isInBattle && GUI.Button(new Rect(365, 10, 160, 40), "Start POP Battle"))
        // {
        //     if (map == null)
        //     {
        //         map = MapsAPI.Instance;
        //     }
        //     APIManager.Instance.Post("place-of-power/start/5d56ebd18aca90617dc3b5fa", "{}", (response, result) =>
        //      {
        //          Debug.Log(response);
        //      });
        // }
    }

    public static void EnterPOP(string id, System.Action onLoad = null)
    {
        Debug.Log(id);
        if (Instance.map == null)
        {
            Instance.map = MapsAPI.Instance;
        }
        LoadingOverlay.Show();

        APIManager.Instance.Get("place-of-power/view/" + id, (response, result) =>
          {
              Debug.Log(response);
              if (result == 200)
              {
                  LoadingOverlay.Hide();
                  var popInfo = LocationPOPInfo.Instance;
                  var data = JsonConvert.DeserializeObject<LocationViewData>(response);
                  popInfo.Show(data);
                  Instance.isViewVisible = true;
              }
              else
              {
                  Debug.Log(result);
              }
              onLoad?.Invoke();
          });

    }

    public static void LoadScene(System.Action onComplete)
    {
        Debug.Log("LOADING POP SCENE");

        previousEnergy = PlayerDataManager.playerData.energy;
        previousState = PlayerDataManager.playerData.state;

        foreach (var item in Instance.MainUIDisable)
        {
            item.SetActive(false);
        }
        if (Instance.map == null)
        {
            Instance.map = MapsAPI.Instance;
        }
        Instance.map.HideMap(true);
        SceneManager.LoadSceneAsync(SceneManager.Scene.PLACE_OF_POWER, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
        {
            sceneLoaded = true;
            onComplete();
        });
        OnMapEnergyChange.OnPlayerDead += LoadPOPManager.UnloadScene;
        OnMapEnergyChange.OnMarkerEnergyChange += LocationUnitSpawner.OnEnergyChange;
    }

    public static void UnloadScene()
    {
        HandleUnload();
        SceneManager.UnloadScene(SceneManager.Scene.PLACE_OF_POWER, null, () =>
        {
            sceneLoaded = false;
            var t = LocationExitInfo.Instance;
            // OnMapEnergyChange.ForceEvent(PlayerManager.marker, (int)(PlayerDataManager.playerData.baseEnergy * .25f));
            // PlayerManagerUI.Instance.UpdateEnergy();
            LocationIslandController.MainSceneLoaded();
            //  SpellcastingFX.ResumeTweening();
            if (UIQuickCast.IsOpen)
                UIQuickCast.Close();
            t.ShowUI();
            LoginAPIManager.GetCharacter((s, r) =>
          {
              LoadingOverlay.Hide();
          });

        });
        OnMapEnergyChange.OnPlayerDead -= LoadPOPManager.UnloadScene;
        OnMapEnergyChange.OnMarkerEnergyChange -= LocationUnitSpawner.OnEnergyChange;
    }

    private static void HandleUnload()
    {
        LoadingOverlay.Show("Loading Map...");
        foreach (var item in Instance.MainUIDisable)
        {
            item.SetActive(true);
        }
        if (UIWaitingCastResult.isOpen)
            UIWaitingCastResult.Instance.Close();
        if (UIPlayerInfo.IsShowing)
            UIPlayerInfo.SetVisibility(false);
        if (UISpiritInfo.IsShowing)
            UISpiritInfo.SetVisibility(false);
        Instance.map.HideMap(false);
        LocationIslandController.BattleStopPOP();
        LocationUnitSpawner.UnloadScene();
        MarkerManagerAPI.GetMarkers();

    }

    public static void UnloadSceneReward()
    {
        HandleUnload();
        SceneManager.UnloadScene(SceneManager.Scene.PLACE_OF_POWER, null, () =>
        {
            LoginAPIManager.GetCharacter((s, r) =>
            {
                LoadingOverlay.Hide();

            });
            sceneLoaded = false;
            UIQuickCast.Close();
            OnMapEnergyChange.OnPlayerDead -= LoadPOPManager.UnloadScene;
            OnMapEnergyChange.OnMarkerEnergyChange -= LocationUnitSpawner.OnEnergyChange;
        });
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
