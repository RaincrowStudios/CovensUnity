using System.Collections.Generic;
using Raincrow.DynamicPlacesOfPower;
using UnityEngine;

public class LocationIslandController : MonoBehaviour
{
    public static LocationIslandController instance { get; private set; }
    [SerializeField] private LocationData m_LocationData;
    public static Dictionary<int, LocationIsland> m_islands = new Dictionary<int, LocationIsland>();
    public int totalIslands = 6;
    public float distance = 50;
    [SerializeField] private LineRenderer m_LineRenderer;

    public PopCameraController popCameraController { get; private set; }

    void Awake()
    {
        instance = this;
        m_LocationData = new LocationData();
        popCameraController = GameObject.FindObjectOfType<PopCameraController>().GetComponent<PopCameraController>();
        // Setup fake data
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

    void Start()
    {
        var spirit = Instantiate(Resources.Load<Transform>("SpiritPrefab"));
        popCameraController.onUpdate += (x, y, z) =>
        {
            spirit.GetChild(0).GetChild(0).rotation = popCameraController.camera.transform.rotation;
        };
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Create"))
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
            m_islands.Clear();
            CreateIslands(m_LocationData);
        }
    }

    public void CreateIslands(LocationData locationData)
    {
        float angleStep = 360 / locationData.islands.Count;
        float previousAngle = 0;
        foreach (var item in locationData.islands)
        {
            LocationIsland island = Instantiate(Resources.Load<LocationIsland>("LocationIsland"));
            island.transform.parent = transform;
            island.transform.position = Vector3.zero;
            island.transform.Rotate(0, previousAngle, 0);
            island.Setup(item.Value.tokenData, distance, popCameraController.camera);
            previousAngle += angleStep;
            m_islands.Add(item.Key, island);
        }
        m_LineRenderer.positionCount = locationData.islands.Count;
        LeanTween.value(0, 1, 1).setOnUpdate((float value) =>
        {
            transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 30, value));
            for (int i = 0; i < transform.childCount; i++)
            {
                m_LineRenderer.SetPosition(i, transform.GetChild(i).GetChild(0).position);
            }
        }).setEase(LeanTweenType.easeInOutQuad);
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