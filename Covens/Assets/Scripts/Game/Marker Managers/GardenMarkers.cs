using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
public class GardenMarkers : MonoBehaviour
{
    public static GardenMarkers instance { get; set; }

    public GameObject gardenPrefab;
    public GameObject gardenCanvas;
    public GameObject lorePrefab;
    public Text title;
    public Image img;
    public Text desc;


    // public float minScaleG = .2f;
    // public float maxScaleG = .6f;
    // public float minZoomG;
    // public float maxZoomG;
    // public float lineWidth;
    bool isCreated = false;

    [SerializeField] float loreScale = 1f;
    [SerializeField] float minLoreZoom = .6f;
    [SerializeField] float gardenScale = .6f;
    [SerializeField] float forbiddenScale = .6f;
    [SerializeField] float minForbiddenZoom = .6f;

    private Transform loreTransform;
    // public float minZoomForbidden;
    // public float maxZoomForbidden;
    // public float minScaleForbidden;
    // public float maxScaleForbidden;

    [System.Serializable]
    public struct GreyHandOfficeData
    {
        public string officeLocation;
        public float officeLongitude;
        public float officeLatitude;
    }

    public GameObject greyHandOfficePrefab;
    public GameObject greyHandMarker;
    public GreyHandOfficeData[] greyHandOffices = new GreyHandOfficeData[3];
    private Transform[] greyHandOfficesTrans = new Transform[3];

    private List<Transform> gardensTransform = new List<Transform>();

    IMaps map;

    private void Awake()
    {
        instance = this;
    }
    public void SetupGardens()
    {
        map = MapsAPI.Instance;
        map.OnChangePosition += SetLoreScale;
        map.OnChangeZoom += updateGardenScale;
        map.OnChangePosition += updateGardenScale;
        map.OnChangePosition += SetGreyHandMarkerScale;
        map.OnChangeZoom += SetLoreScale;
        map.OnChangeZoom += SetGreyHandMarkerScale;
        map.OnEnterStreetLevel += checkLoreOnLand;

        foreach (var item in PlayerDataManager.config.gardens)
        {
            var g = Utilities.InstantiateObject(gardenPrefab, map.trackedContainer, 0);
            g.name = item.id;
            g.transform.position = map.GetWorldPosition(item.longitude, item.latitude);
            g.transform.localEulerAngles = new Vector3(90, 0, 180);
            g.GetComponentInChildren<TextMeshPro>().text = DownloadedAssets.gardenDict[item.id].title;
            gardensTransform.Add(g.transform);
        }

        for (int i = 0; i < greyHandOffices.Length; i++)
        {
            var greyHand = Utilities.InstantiateObject(greyHandMarker, map.trackedContainer);
            greyHand.name = greyHandOffices[i].officeLocation;
            greyHand.transform.position = map.GetWorldPosition(greyHandOffices[i].officeLongitude, greyHandOffices[i].officeLatitude);
            greyHand.transform.Rotate(90, 0, 0);
            Debug.Log("created grey hand office at: " + greyHand.transform.position);
            greyHandOfficesTrans[i] = greyHand.transform;
        }

        var loreT = Utilities.InstantiateObject(lorePrefab, map.trackedContainer);
        loreT.name = "lore";
        loreT.transform.position = map.GetWorldPosition(PlayerDataManager.config.explore.longitude, PlayerDataManager.config.explore.latitude);
        Debug.Log("|||||| Created Lore at : " + loreT.transform.position);
        loreTransform = loreT.transform;
        isCreated = true;
        loreT.SetActive(false);

    }


    void checkLoreOnLand()
    {
        Debug.Log("Checking lore");
        Debug.Log(map.DistanceBetweenPointsD(map.position, new Vector2(PlayerDataManager.config.explore.longitude, PlayerDataManager.config.explore.latitude)));
        if (!PlayerDataManager.playerData.dailies.explore.complete && map.DistanceBetweenPointsD(map.position, new Vector2(PlayerDataManager.config.explore.longitude, PlayerDataManager.config.explore.latitude)) < 8)
        {
            SendQuestLore();
        }
    }

    void SetGreyHandMarkerScale()
    {
        foreach (var item in greyHandOfficesTrans)
        {
            if (map.normalizedZoom > minForbiddenZoom)
            {
                item.gameObject.SetActive(true);
                item.localScale = Vector3.one * forbiddenScale * MapLineraScale.linearMultiplier;
                item.gameObject.SetActive(!map.streetLevel);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

    }


    void updateGardenScale()
    {

        // float sMultiplier = MapUtils.scale(maxScaleG, minScaleG, 0.05f, maxZoomG, map.normalizedZoom);
        //Debug.Log(sMultiplier + " || " + map.normalizedZoom);

        foreach (Transform item in gardensTransform)
        {
            // if (map.normalizedZoom > maxZoomG)
            // {
            //     item.gameObject.SetActive(false);
            // }
            // else
            // {
            // item.gameObject.SetActive(true);
            item.localScale = Vector3.one * gardenScale * MapLineraScale.linearMultiplier;
            item.gameObject.SetActive(!map.streetLevel);
            // }
        }
    }

    void SetLoreScale()
    {

        if (map.normalizedZoom >= minLoreZoom)
        {
            loreTransform.gameObject.SetActive(true);
            loreTransform.localScale = Vector3.one * loreScale * MapLineraScale.linearMultiplier;
            loreTransform.gameObject.SetActive(!map.streetLevel);
        }
        else
        {
            loreTransform.gameObject.SetActive(false);
        }


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && map != null && !map.streetLevel)
        {
            RaycastHit hit;

            Ray ray = map.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "garden")
                {
                    OnClick(hit.transform.name);
                }
                else if (hit.transform.name == "lore")
                {
                    SendQuestLore();
                }
                else if (hit.transform.tag == "greyHand")
                {
                    Debug.Log("Loading Grey Hand Prefab...");
                    var gho = Instantiate(greyHandOfficePrefab);
                    gho.GetComponent<GreyHandOffice>().TextSetup(hit.transform.name);
                }
            }
        }
    }

    private static void SendQuestLore()
    {
        Debug.Log("sending quest lore data");
        var data = new { lore = PlayerDataManager.config.explore.id };
        APIManager.Instance.PostData("lore/select", JsonConvert.SerializeObject(data), (string s, int r) =>
        {
            if (r == 200)
            {
                QuestsController.instance.ExploreQuestDone(data.lore);
            }
        });
    }

    public void OnClick(string id)
    {
        img.gameObject.SetActive(false);
        gardenCanvas.SetActive(true);
        title.text = DownloadedAssets.gardenDict[id].title;
        desc.text = DownloadedAssets.gardenDict[id].description;
        StartCoroutine(GetImage(id));
    }

    IEnumerator GetImage(string id)
    {
        WWW www = new WWW(DownloadAssetBundle.baseURL + "gardens/" + id + ".png");
        yield return www;
        img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        img.gameObject.SetActive(true);
    }
}

