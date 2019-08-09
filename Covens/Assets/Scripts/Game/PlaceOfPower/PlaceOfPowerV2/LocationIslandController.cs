using System;
using System.Collections;
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
    public static event System.Action<WitchToken> OnWitchEnter;
    public static bool isInBattle
    {
        get
        {
            return locationData.isInBattle;
        }
    }
    public static LocationData locationData => m_LocationData;

    [SerializeField] private static LocationData m_LocationData;

    [Header("Debug Parameters")]
    [SerializeField] private int totalIslands = 6;
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

    public void Initiate()
    {
        CreateIslands(m_LocationData);
    }

    private void BattleBeginPOP()
    {
        MoveTokenHandlerPOP.OnMarkerMovePOP += instance.locationUnitSpawner.MoveMarker;
        AddSpiritHandlerPOP.OnSpiritAddPOP += instance.locationUnitSpawner.AddMarker;
        instance.popCameraController.onUpdate += UpdateMarkers;
        m_LocationData.isInBattle = true;
        CreateIslands(m_LocationData);
    }

    private void BattleStopPOP()
    {
        MoveTokenHandlerPOP.OnMarkerMovePOP -= instance.locationUnitSpawner.MoveMarker;
        AddSpiritHandlerPOP.OnSpiritAddPOP -= instance.locationUnitSpawner.AddMarker;
        popCameraController.onUpdate -= UpdateMarkers;
        LocationBattleStart.OnLocationBattleStart -= BattleBeginPOP;
        LocationBattleEnd.OnLocationBattleEnd -= BattleStopPOP;
    }

    private static void WitchJoined(WitchToken token)
    {
        if (!m_LocationData.tokens.ContainsKey1(token.popIndex) && !m_LocationData.tokens.ContainsKey2(token.instance))
        {
            Debug.Log(m_LocationData.tokens.Count + "  A");
            locationData.tokens.Add(token.popIndex, token.instance, token);
            Debug.Log(m_LocationData.tokens.Count + "  A2");

            m_LocationData.currentOccupants = m_LocationData.tokens.Count;
            OnWitchEnter?.Invoke(token);
        }
    }

    public static void EnterPOP(string id, System.Action<LocationData> OnComplete)
    {
        Debug.Log("EnterPOP");
        APIManager.Instance.Put($"place-of-power/enter/{id}", "{}", (response, result) =>
          {
              Debug.Log(result);
              Debug.Log(response);
              AddWitchHandlerPOP.OnWitchAddPOP += WitchJoined;
              if (result == 200)
              {

                  m_LocationData = LocationSlotParser.HandleResponse(response);
                  Debug.Log(m_LocationData.tokens.Count + "  E");
                  OnComplete(locationData);
                  LoadPOPManager.LoadScene(() =>
                  {
                      LocationBattleStart.OnLocationBattleStart += instance.BattleBeginPOP;
                      LocationBattleEnd.OnLocationBattleEnd += instance.BattleStopPOP;
                  });

              }
              else
              {
                  OnComplete(null);
              }
          });
    }

    public static void ExitPOP()
    {
        AddWitchHandlerPOP.OnWitchAddPOP -= WitchJoined;
        APIManager.Instance.Put($"place-of-power/leave", "{}", (response, result) =>
          {
              Debug.Log(result);
              Debug.Log(response);
          });
    }

    private void CreateIslands(LocationData locationData)
    {
        CreateGuardianSpirit();

        float angleStep = 360 / locationData.islands;
        float previousAngle = 0;
        unitPositions = new List<Transform>();
        for (int i = 0; i < locationData.islands; i++)
        {
            LocationIsland island = SpawnIsland(previousAngle);
            unitPositions.AddRange(island.Setup(distance));
            previousAngle += angleStep;
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

    private void CreateGuardianSpirit()
    {
        var spirit = Instantiate(Resources.Load<Transform>("SpiritPrefab"));
        popCameraController.onUpdate += (x, y, z) =>
        {
            spirit.GetChild(0).GetChild(0).rotation = popCameraController.camera.transform.rotation;
        };
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
        if (arg3)
        {
            foreach (var item in LocationUnitSpawner.Markers)
            {
                item.Value.AvatarTransform.rotation = instance.popCameraController.camera.transform.rotation;
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