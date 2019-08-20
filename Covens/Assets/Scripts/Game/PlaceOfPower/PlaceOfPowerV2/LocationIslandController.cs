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
    private static bool m_IsInBattle = false;
    private Vector2 m_MouseDownPosition;

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

    private void BattleBeginPOP(SpiritToken guardianSpirit)
    {
        isInBattle = true;
        MoveTokenHandlerPOP.OnMarkerMovePOP += instance.locationUnitSpawner.MoveMarker;
        AddSpiritHandlerPOP.OnSpiritAddPOP += instance.locationUnitSpawner.AddMarker;
        CreateIslands(m_LocationData);
        CreateTokens();
        locationUnitSpawner.AddMarker(guardianSpirit);
        UpdateMarkers(false, true, true);
        LocationPOPInfo.Instance.Close();
        SetActiveIslands();
        instance.popCameraController.onUpdate += UpdateMarkers;
    }

    private void CreateTokens()
    {
        foreach (var item in m_LocationData.tokens.Values())
        {
            if (item is WitchToken)
                locationUnitSpawner.AddMarker((WitchToken)item);
            else if (item is SpiritToken)
                locationUnitSpawner.AddMarker((SpiritToken)item);
        }
    }

    public static void BattleStopPOP()
    {
        MoveTokenHandlerPOP.OnMarkerMovePOP -= instance.locationUnitSpawner.MoveMarker;
        AddSpiritHandlerPOP.OnSpiritAddPOP -= instance.locationUnitSpawner.AddMarker;
        instance.popCameraController.onUpdate -= UpdateMarkers;
        LocationBattleStart.OnLocationBattleStart -= instance.BattleBeginPOP;
        LocationBattleEnd.OnLocationBattleEnd -= BattleStopPOP;
        isInBattle = false;
    }

    private static void WitchJoined(WitchToken token)
    {
        if (!m_LocationData.tokens.ContainsKey1(token.popIndex) && !m_LocationData.tokens.ContainsKey2(token.instance))
        {
            m_LocationData.tokens.Add(token.popIndex, token.instance, token);
            m_LocationData.currentOccupants = m_LocationData.tokens.Count;
            OnWitchEnter?.Invoke(token);
        }
    }

    private static void WitchRemoved(RemoveTokenHandlerPOP.RemoveEventData removeData)
    {
        if (m_LocationData.tokens.ContainsKey2(removeData.instance))
        {
            int popIndex = removeData.island * 3 + removeData.position;
            m_LocationData.currentOccupants--;
            OnWitchExit?.Invoke(m_LocationData.tokens[popIndex, removeData.instance] as WitchToken);
            m_LocationData.tokens.Remove(popIndex, removeData.instance);
        }
    }

    public static void EnterPOP(string id, System.Action<LocationData> OnComplete)
    {
        APIManager.Instance.Put($"place-of-power/enter/{id}", "{}", async (response, result) =>
          {
              Debug.Log(result);
              Debug.Log(response);
              AddWitchHandlerPOP.OnWitchAddPOP += WitchJoined;
              RemoveTokenHandlerPOP.OnRemoveTokenPOP += WitchRemoved;
              if (result == 200)
              {

                  m_LocationData = LocationSlotParser.HandleResponse(response);
                  OnComplete(locationData);
                  await Task.Delay(2200);
                  LoadPOPManager.LoadScene(() =>
                  {
                      LocationBattleStart.OnLocationBattleStart += instance.BattleBeginPOP;
                      LocationBattleEnd.OnLocationBattleEnd += BattleStopPOP;
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
            item.Value.AvatarTransform.rotation = instance.popCameraController.camera.transform.rotation;
        }
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

    public static void moveCamera(Vector3 position)
    {
        instance.popCameraController.MoveCamera(position, 1.2f);
    }

    private void Update()
    {
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
                    LP.OnClick();
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