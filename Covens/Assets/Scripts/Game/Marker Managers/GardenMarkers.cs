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


    public float minScaleG = .2f;
    public float maxScaleG = .6f;
    // public float minZoomG;
    public float maxZoomG;
    public float lineWidth;
    bool isCreated = false;

    [SerializeField] float minScale = .2f;
    [SerializeField] float maxScale = .6f;
    [SerializeField] float maxVisibleZoom = .2f;
    [SerializeField] float minVisibleZoom = .6f;

    private Transform loreTransform;
    public float minZoomForbidden;
    public float maxZoomForbidden;
    public float minScaleForbidden;
    public float maxScaleForbidden;
    List<Label> labels = new List<Label>();

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

    void SetGreyHandMarkerScale()
    {
        for (int i = 0; i < greyHandOffices.Length; i++)
        {

            if (map.normalizedZoom >= minZoomForbidden && map.normalizedZoom <= maxZoomForbidden)
            {
                greyHandOfficesTrans[i].gameObject.SetActive(true);

                float multiplier = MapUtils.scale(minScaleForbidden, maxScaleForbidden, maxZoomForbidden, minZoomForbidden, map.normalizedZoom);
                greyHandOfficesTrans[i].localScale = Vector3.one * multiplier * 3;
            }
            else
            {
                greyHandOfficesTrans[i].gameObject.SetActive(false);
            }
        }
    }


    void updateGardenScale()
    {

        float sMultiplier = MapUtils.scale(maxScaleG, minScaleG, 0.05f, maxZoomG, map.normalizedZoom);
        //Debug.Log(sMultiplier + " || " + map.normalizedZoom);

        foreach (Transform item in gardensTransform)
        {
            if (map.normalizedZoom > maxZoomG)
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
                item.localScale = Vector3.one * sMultiplier;
            }
        }
    }

    void SetLoreScale()
    {

        if (map.normalizedZoom >= minVisibleZoom && map.normalizedZoom <= maxVisibleZoom)
        {
            loreTransform.gameObject.SetActive(true);
            float sMultiplier = MapUtils.scale(maxScale, minScale, minVisibleZoom, maxVisibleZoom, map.normalizedZoom);
            loreTransform.localScale = Vector3.one * sMultiplier;
            // foreach (var item in greyHandOfficesTrans)
            // {
            //     item.gameObject.SetActive(true);
            //     item.localScale = Vector3.one * sMultiplier * 3;
            // }
        }
        else
        {
            // foreach (var item in greyHandOfficesTrans)
            // {
            //     item.gameObject.SetActive(false);

            // }
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
                else if (hit.transform.tag == "greyHand")
                {
                    Debug.Log("Loading Grey Hand Prefab...");
                    var gho = Instantiate(greyHandOfficePrefab);
                    gho.GetComponent<GreyHandOffice>().TextSetup(hit.transform.name);
                }
            }
        }
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

