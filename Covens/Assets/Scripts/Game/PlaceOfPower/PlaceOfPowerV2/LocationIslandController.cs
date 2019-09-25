using System.Threading.Tasks;
using System.Collections.Generic;
using Raincrow.DynamicPlacesOfPower;
using Raincrow.Maps;
using UnityEngine;
using Raincrow.GameEventResponses;

public class LocationIslandController : MonoBehaviour
{
    public static LocationIslandController instance { get; private set; }
    public static bool inLocation { get; private set; }
    public static List<Transform> unitPositions { get; private set; }
    public static Dictionary<int, LocationIsland> locationIslands { get; private set; }
    public static event System.Action<WitchToken> OnWitchEnter;
    public static event System.Action<WitchToken> OnWitchExit;

    public static event System.Action OnEnterLocation;
    public static event System.Action OnExitLocation;



    private static bool m_IsInBattle = false;

    private Vector2 m_MouseDownPosition;

    public static string popName = "";

    public static bool isGuardianActive { get; private set; }

    public static bool isInBattle
    {
        get
        {
            return m_IsInBattle;
        }
        private set { m_IsInBattle = value; }
    }
    public static LocationData locationData => m_LocationData;

    private static LocationData m_LocationData;

    [Header("Debug Parameters")]
    [SerializeField] private float distance = 50;

    [Header("Prefabs")]
    [SerializeField] private LineRenderer m_LinePrefab;

    [Header("Scripts")]
    [SerializeField] private PopCameraController popCameraController;
    [SerializeField] private LocationUnitSpawner locationUnitSpawner;


    private void Awake()
    {
        instance = this;
    }

    private static SpiritToken preInitializedSpirit = null;

    private void BattleBeginPOP(SpiritToken guardianSpirit)
    {
        if (!isInBattle)
        {
            // TODO CHANGE FOR RESUME BATTLE
            isGuardianActive = false;

            Debug.Log("[PLACE OF POWER] battle Starting");
            isInBattle = true;
            OnEnterLocation?.Invoke();
            MoveTokenHandlerPOP.OnMarkerMovePOP += instance.locationUnitSpawner.MoveMarker;
            AddSpiritHandlerPOP.OnSpiritAddPOP += instance.locationUnitSpawner.AddMarker;
            CreateIslands(m_LocationData);
            CreateTokens();
            if (guardianSpirit != null)
                AddGuardianSpirit(guardianSpirit);
            //   UpdateMarkers(false, true, true);
            LocationPOPInfo.Instance.Close();
            SetActiveIslands();
            instance.popCameraController.onUpdate += UpdateMarkers;

            PreActivateGuardian();

            if (popName != "")
                PlayerNotificationManager.Instance.ShowNotification($"The battle for {popName} has started!");
        }
        else
        {
            CreateTokens();
            if (guardianSpirit != null)
                locationUnitSpawner.AddMarker(guardianSpirit);
            SetActiveIslands();
        }
    }

    private void AddGuardianSpirit(SpiritToken spiritToken)
    {
        locationUnitSpawner.AddMarker(spiritToken);
    }

    private async void CreateTokens()
    {
        if (LocationUnitSpawner.Markers.Count > 0)
        {
            locationUnitSpawner.RemoveAllMarkers();
            await Task.Delay(1000);
        }
        foreach (var item in m_LocationData.tokens.Values())
        {
            if (item is WitchToken)
                locationUnitSpawner.AddMarker((WitchToken)item);
            else if (item is SpiritToken)
                locationUnitSpawner.AddMarker((SpiritToken)item);
        }
    }

    private static void HandleEnergyZero(string id, int energy)
    {
        if (id == PlayerDataManager.playerData.instance) //update the players energy
        {
            LocationPlayerAction.playerMarker.UpdateEnergy();
        }
        else if (LocationUnitSpawner.Markers.ContainsKey(id))
        {
            LocationUnitSpawner.Markers[id].UpdateEnergy();

            if (energy == 0)
                instance.locationUnitSpawner.RemoveMarker(id);
        }
    }

    public static void BattleStopPOP()
    {
        OnExitLocation?.Invoke();
        MoveTokenHandlerPOP.OnMarkerMovePOP -= instance.locationUnitSpawner.MoveMarker;
        AddSpiritHandlerPOP.OnSpiritAddPOP -= instance.locationUnitSpawner.AddMarker;
        instance.popCameraController.onUpdate -= UpdateMarkers;
        LocationBattleStart.OnLocationBattleStart -= instance.BattleBeginPOP;
        LocationBattleEnd.OnLocationBattleEnd -= BattleStopPOP;
        ExpireAstralHandler.OnExpireAstral -= LocationUnitSpawner.DisableCloaking;
        RespawnSpiritPOP.OnSpiritRewspawn -= instance.AddGuardianSpirit;
        OnMapEnergyChange.OnEnergyChange -= HandleEnergyZero;
        isInBattle = false;
        PlayerDataManager.playerData.insidePlaceOfPower = false;
    }

    private static void WitchJoined(WitchToken token)
    {
        Debug.Log(token.instance + " | ADDED ");
        if (!m_LocationData.tokens.ContainsKey1(token.popIndex) && !m_LocationData.tokens.ContainsKey2(token.instance))
        {
            m_LocationData.tokens.Add(token.popIndex, token.instance, token);
            m_LocationData.currentOccupants = m_LocationData.tokens.Count;
            OnWitchEnter?.Invoke(token);
        }
    }

    private static void WitchRemoved(RemoveTokenHandlerPOP.RemoveEventData removeData)
    {
        Debug.Log("Trying to Remove Token");
        if (m_LocationData.tokens.ContainsKey2(removeData.instance))
        {
            int popIndex = removeData.island * 3 + removeData.position;
            m_LocationData.currentOccupants--;
            OnWitchExit?.Invoke(m_LocationData.tokens[popIndex, removeData.instance] as WitchToken);
            m_LocationData.tokens.Remove(popIndex, removeData.instance);
            if (isInBattle)
            {
                Debug.Log("Removing Token");
                instance.locationUnitSpawner.RemoveMarker(removeData.instance);
            }
        }
    }

    private static bool PreActivateGuardian()
    {
        int witchCount = 0;
        foreach (var item in LocationUnitSpawner.Markers)
        {
            if (item.Value.Token.Type == MarkerManager.MarkerType.WITCH)
            {
                witchCount++;
            }
        }

        if (witchCount < 4)
        {
            foreach (var item in locationIslands)
            {
                item.Value.SetSpiritConnection(true);
            }
        }
        return witchCount < 4;
    }

    public static void ResumeBattle(string id)
    {
        LoadingOverlay.Show("Loading the place of power battle...");
        isInBattle = false;
        APIManager.Instance.Get($"place-of-power/{id}", "{}", (response, result) =>
        {
            if (result == 200)
            {
                Debug.Log(result);
                Debug.Log(response);
                response.CopyToClipboard();
                m_LocationData = LocationSlotParser.HandleResponse(response, true);
                LoadPOPManager.LoadScene(() =>
                  {
                      LoadingOverlay.Hide();
                      ExpireAstralHandler.OnExpireAstral += LocationUnitSpawner.DisableCloaking;
                      OnMapEnergyChange.OnPlayerDead += LoadPOPManager.UnloadScene;
                      OnMapEnergyChange.OnMarkerEnergyChange += LocationUnitSpawner.OnEnergyChange;
                      LocationBattleEnd.OnLocationBattleEnd += BattleStopPOP;
                      RespawnSpiritPOP.OnSpiritRewspawn += instance.AddGuardianSpirit;
                      RewardHandlerPOP.LocationReward += OnReward;
                      instance.BattleBeginPOP(m_LocationData.spirit);
                      OnMapEnergyChange.OnEnergyChange += HandleEnergyZero;

                      if (m_LocationData.spirit != null && m_LocationData.spirit.islands != null)
                      {
                          foreach (var item in m_LocationData.spirit.islands)
                          {
                              locationIslands[item].SetSpiritConnection(true);
                          }
                      }
                  });
            }
            else
            {
                Debug.Log(response);
                APIManager.Instance.Put($"place-of-power/leave", "{}", (s, r) =>
                {
                    if (r == 200)
                    {
                        PlayerDataManager.playerData.insidePlaceOfPower = false;
                    }
                });
                UIGlobalPopup.ShowError(() => { }, "Entering in pop failed.");
            }
        });
    }

    private static void OnReward(RewardHandlerPOP.RewardPOPData rewardData)
    {
        Debug.Log("REWARD!!!");
        LocationRewardInfo.Instance.Setup(popName, rewardData, () =>
        {
            PlayerDataManager.playerData.insidePlaceOfPower = false;
            LoadPOPManager.UnloadSceneReward();
        });
        RewardHandlerPOP.LocationReward -= OnReward;
    }

    public static void EnterPOP(string id, System.Action<LocationData> OnComplete)
    {
        APIManager.Instance.Put($"place-of-power/enter/{id}", "{}", async (response, result) =>
          {
              Debug.Log(result);
              Debug.Log(response);
              AddWitchHandlerPOP.OnWitchAddPOP += WitchJoined;
              RemoveTokenHandlerPOP.OnRemoveTokenPOP += WitchRemoved;
              preInitializedSpirit = null;
              if (result == 200)
              {
                  MarkerSpawner.ClearImmunities();
                  LocationBattleStart.OnLocationBattleStart += (s) =>
                  {
                      Debug.Log("BATTLE STARTING");
                      preInitializedSpirit = s;
                  };
                  m_LocationData = LocationSlotParser.HandleResponse(response);
                  OnComplete(locationData);
                  await Task.Delay(2200);
                  LoadPOPManager.LoadScene(() =>
                  {

                      RespawnSpiritPOP.OnSpiritRewspawn += instance.AddGuardianSpirit;
                      ExpireAstralHandler.OnExpireAstral += LocationUnitSpawner.DisableCloaking;
                      OnMapEnergyChange.OnPlayerDead += LoadPOPManager.UnloadScene;
                      OnMapEnergyChange.OnMarkerEnergyChange += LocationUnitSpawner.OnEnergyChange;
                      LocationBattleEnd.OnLocationBattleEnd += BattleStopPOP;
                      RewardHandlerPOP.LocationReward += OnReward;
                      OnMapEnergyChange.OnEnergyChange += HandleEnergyZero;

                      if (preInitializedSpirit != null)
                      {
                          Debug.Log("PRE INTIALIZED");
                          instance.BattleBeginPOP(preInitializedSpirit);
                      }
                      LocationBattleStart.OnLocationBattleStart += instance.BattleBeginPOP;


                  });
              }
              else
              {
                  OnComplete(null);
              }
          });
    }

    public static void ExitPOP(System.Action OnComplete)
    {
        AddWitchHandlerPOP.OnWitchAddPOP -= WitchJoined;
        RemoveTokenHandlerPOP.OnRemoveTokenPOP -= WitchRemoved;
        APIManager.Instance.Put($"place-of-power/leave", "{}", (response, result) =>
          {
              isInBattle = false;
              OnComplete();
          });
    }

    private void CreateIslands(LocationData locationData)
    {

        float angleStep = 360 / locationData.islands;
        float previousAngle = 0;
        unitPositions = new List<Transform>();
        locationIslands = new Dictionary<int, LocationIsland>();
        for (int i = 0; i < locationData.islands; i++)
        {
            LocationIsland island = SpawnIsland(previousAngle);
            unitPositions.AddRange(island.Setup(distance, i));
            previousAngle += angleStep;
            island.gameObject.name = i.ToString();
            locationIslands.Add(i, island);
        }
        m_LinePrefab.positionCount = locationData.islands;

        LeanTween.value(0, 1, 1).setOnUpdate((float value) =>
        {
            transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 30, value));
            for (int i = 0; i < transform.childCount; i++)
            {
                m_LinePrefab.SetPosition(i, transform.GetChild(i).GetChild(0).position);
            }
            UpdateMarkers(true, true, true);
        }).setEase(LeanTweenType.easeInOutQuad);
    }

    private LocationIsland SpawnIsland(float previousAngle)
    {
        LocationIsland island = Instantiate(Resources.Load<LocationIsland>("LocationIsland"));
        island.transform.parent = transform;
        island.transform.position = Vector3.zero;
        island.transform.Rotate(0, previousAngle, 0);
        return island;
    }

    private static void UpdateMarkers(bool arg1, bool arg2, bool arg3)
    {
        foreach (var item in LocationUnitSpawner.Markers)
        {
            // Debug.Log(instance.popCameraController.camera.transform.rotation);
            item.Value.AvatarTransform.rotation = instance.popCameraController.camera.transform.rotation;
        }
    }

    public static void UpdateMarker(IMarker m)
    {
        // Debug.Log("fixing orientation");
        // Debug.Log(instance.popCameraController.camera.transform.rotation);
        // Debug.Log(instance.popCameraController.camera.transform.localEulerAngles);
        m.AvatarTransform.rotation = instance.popCameraController.camera.transform.rotation;
    }

    public static void SetActiveIslands()
    {
        HashSet<int> activePositions = new HashSet<int>();
        activePositions.Add(LocationPlayerAction.getCurrentIsland);
        activePositions.Add(LocationPlayerAction.getCurrentIsland - 1 >= 0 ? LocationPlayerAction.getCurrentIsland - 1 : locationIslands.Count - 1);
        activePositions.Add(LocationPlayerAction.getCurrentIsland + 1 < locationIslands.Count ? LocationPlayerAction.getCurrentIsland + 1 : 0);

        foreach (var item in locationIslands)
        {
            if (activePositions.Contains(item.Key))
            {
                item.Value.ActivateIsland(true);
            }
            else
            {
                item.Value.ActivateIsland(false);
            }
        }
    }

    public static void ActivateSpiritConnection(string id)
    {
        int island = LocationUnitSpawner.GetIsland(id);
        if (locationIslands.ContainsKey(island))
        {
            if (!locationIslands[island].IsConnected)
            {
                locationIslands[island].SetSpiritConnection(true);
                if (!isGuardianActive)
                {
                    PlayerNotificationManager.Instance.ShowNotification("The Guardian Spirit has awakened!");
                    isGuardianActive = true;
                }
                locationIslands[island].SetSpiritConnection(true);
            }
        }
    }

    public static void moveCamera(Vector3 position)
    {
        instance.popCameraController.MoveCamera(position, 1.2f);
    }

    private void Update()
    {
        if (LocationPlayerAction.isCloaked) return;
        bool inputUp = false;
        bool inputDown = false;

        if (Application.isEditor)
        {
            inputDown = Input.GetMouseButtonDown(0);
            inputUp = Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
        else
        {
            inputDown = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId);
            inputUp = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended;
        }

        if (inputDown)
        {
            m_MouseDownPosition = Input.mousePosition;
            return;
        }
        else if (inputUp)
        {
            if (Vector2.Distance(m_MouseDownPosition, Input.mousePosition) > 15)
            {
                m_MouseDownPosition = new Vector2(-Screen.width, -Screen.height);
                return;
            }
            m_MouseDownPosition = new Vector2(-Screen.width, -Screen.height);

            Camera cam = popCameraController.camera;

            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << 20))
            {
                IMarker marker = hit.transform.GetComponentInParent<IMarker>();
                if (marker != null)
                {
                    marker.OnClick?.Invoke(marker);
                    return;
                }
                LocationPosition LP = hit.transform.GetComponent<LocationPosition>();
                if (LP != null)
                {
                    if (unitPositions[LP.popIndex].childCount < 2)
                    {
                        LP.OnClick();
                    }
                }

            }
        }
    }
}

public class LocationData
{
    public string _id { get; set; }
    public int maxSlots { get; set; }
    public int currentOccupants { get; set; }
    public bool isInBattle { get; set; }
    public int islands
    {
        get
        {
            return maxSlots / 3;
        }
    }
    public MultiKeyDictionary<int, string, object> tokens = new MultiKeyDictionary<int, string, object>();
    public SpiritToken spirit { get; set; }
}

public class LocationViewData
{
    public string _id { get; set; }
    public string name { get; set; }
    public double battleFinishedOn { get; set; }
    public double coolDownEndOn
    {
        get
        {
            return battleFinishedOn + (coolDownTimeWindow * 1000);
        }
        set { }
    } // if pop is under cooldown
    public double battleBeginsOn { get; set; } // when did/will the battle begin
    public bool isActive { get; set; }     // on going battle {cooldownEnd:0} {battleBegin:934099}
    public bool isOpen { get; set; }     // accepting players
    public int tier { get; set; }
    public int openTimeWindow { get; set; }
    public int coolDownTimeWindow { get; set; }
}

public class LocationWitchToken
{
    public WitchToken character;
    public int island { get; set; }
    public int position { get; set; }
}

public class LocationSpiritToken
{
    public SpiritToken spirit;
    public int island { get; set; }
    public int position { get; set; }
}