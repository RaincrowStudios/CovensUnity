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
    private static int m_UnloadTweenId;

    void Awake()
    {
        Instance = this;
        Debug.Log("LOADING POP MANAGER");
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
        LeanTween.cancel(m_UnloadTweenId);

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
            RemoveTokenHandlerPOP.OnRemoveTokenPOP += OnPlayerDead;
            OnMapEnergyChange.OnMarkerEnergyChange += LocationUnitSpawner.OnEnergyChange;
            ExpireAstralHandler.OnExpireAstral += LocationUnitSpawner.DisableCloaking;
            onComplete();
        });
    }

    private static async void OnPlayerDead(RemoveTokenHandlerPOP.RemoveEventData data)
    {
        await System.Threading.Tasks.Task.Delay(1000);

        if (PlayerDataManager.playerData.instance != data.instance)
            return;

        LocationExitInfo.Instance.ShowUI(UnloadScene);
    }

    public static void UnloadScene()
    {
        PlayerDataManager.playerData.insidePlaceOfPower = false;
        LocationIslandController.isInBattle = false;

        RemoveTokenHandlerPOP.OnRemoveTokenPOP -= OnPlayerDead;
        OnMapEnergyChange.OnMarkerEnergyChange -= LocationUnitSpawner.OnEnergyChange;
        ExpireAstralHandler.OnExpireAstral -= LocationUnitSpawner.DisableCloaking;

        foreach (var item in Instance.MainUIDisable)
            item.SetActive(true);
        if (UIWaitingCastResult.isOpen)
            UIWaitingCastResult.Instance.Close();
        //if (UIPlayerInfo.IsShowing)
        //    UIPlayerInfo.SetVisibility(false);
        //if (UISpiritInfo.IsShowing)
        //    UISpiritInfo.SetVisibility(false);
        LocationIslandController.BattleStopPOP();
        LocationUnitSpawner.UnloadScene();

        Instance.map.HideMap(false);

        LoadingOverlay.Show("Loading Map...");
        SceneManager.UnloadScene(SceneManager.Scene.PLACE_OF_POWER, null, () =>
        {
            if (UIQuickCast.IsOpen)
                UIQuickCast.Close();

            LoginAPIManager.GetCharacter((s, r) =>
            {
                PlayerManager.witchMarker.UpdateEnergy(0);
                PlayerManagerUI.Instance.UpdateEnergy();
                MarkerManagerAPI.GetMarkers(true, true, () => LoadingOverlay.Hide(), true);
            });
        });        
    }

    public static void HandleQuickCastOpen(System.Action OnOpen)
    {
        OnOpen?.Invoke();
    }
}
