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

    [SerializeField] private LocationData m_LocationData;

    [Header("Debug Parameters")]
    [SerializeField] private int totalIslands = 6;
    [SerializeField] private float distance = 50;

    [Header("Prefabs")]
    [SerializeField] private LineRenderer m_LinePrefab;

    [Header("Scripts")]
    [SerializeField] private PopCameraController popCameraController;
    [SerializeField] private LocationUnitSpawner locationUnitSpawner;

    void Awake()
    {
        instance = this;
        GenerateFakeData();
    }

    private void GenerateFakeData()
    {
        m_LocationData = new LocationData();
        for (int i = 0; i < totalIslands; i++)
        {
            int range = Random.Range(0, 3);
            var tokenData = new Dictionary<int, string>();
            for (int j = 0; j <= range; j++)
            {
                tokenData.Add(j, Random.Range(-99999, 99999).ToString());
            }
            var mLData = new LocationIslandData();
            mLData.tokenData = tokenData;
            m_LocationData.islands.Add(i, mLData);
        }
    }

    public void Initiate()
    {
        CreateIslands(m_LocationData);
    }

    // IEnumerator StartTest()
    // {
    // }

    private void CreateIslands(LocationData locationData)
    {
        CreateGuardianSpirit();

        float angleStep = 360 / locationData.islands.Count;
        float previousAngle = 0;
        unitPositions = new List<Transform>();
        foreach (var item in locationData.islands)
        {
            LocationIsland island = SpawnIsland(previousAngle);
            unitPositions.AddRange(island.Setup(distance));
            previousAngle += angleStep;
        }
        m_LinePrefab.positionCount = locationData.islands.Count;
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

    public static void EnterPOP()
    {

        MoveTokenHandlerPOP.OnMarkerMovePOP += instance.locationUnitSpawner.MoveMarker;
        MoveTokenHandlerPOP.OnMarkerMovePOP += instance.locationUnitSpawner.MoveMarker;
        AddWitchHandlerPOP.OnWitchAddPOP += instance.locationUnitSpawner.AddMarker;
        AddSpiritHandlerPOP.OnSpiritAddPOP += instance.locationUnitSpawner.AddMarker;
        instance.popCameraController.onUpdate += UpdateMarkers;
        instance.CreateIslands(instance.m_LocationData);
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

    public static void ExitPOP()
    {
        MoveTokenHandlerPOP.OnMarkerMovePOP -= instance.locationUnitSpawner.MoveMarker;
        MoveTokenHandlerPOP.OnMarkerMovePOP -= instance.locationUnitSpawner.MoveMarker;
        AddWitchHandlerPOP.OnWitchAddPOP -= instance.locationUnitSpawner.AddMarker;
        AddSpiritHandlerPOP.OnSpiritAddPOP -= instance.locationUnitSpawner.AddMarker;
        instance.popCameraController.onUpdate -= UpdateMarkers;
    }
}

public class LocationData
{
    public Dictionary<int, LocationIslandData> islands = new Dictionary<int, LocationIslandData>();
}

public class LocationIslandData
{
    public Dictionary<int, string> tokenData = new Dictionary<int, string>();
}

