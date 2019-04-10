using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
public class GardenMarkers : MonoBehaviour
{
    public GameObject gardenPrefab;
    public Transform container;
    public GameObject gardenCanvas;
    public GameObject lorePrefab;
    public Text title;
    public Image img;
    public Text desc;
    public SpriteMapsController sm;
    bool isCreated = false;
    public Camera camera;

    [SerializeField] float minScale = .2f;
    [SerializeField] float maxScale = .6f;
    [SerializeField] float visibleZoom = 2.4f;
    [SerializeField] float minVisibleZoom = 1.3f;

    private Transform loreTransform;
    private float minZoom;
    private float maxZoom;
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

    void Start()
    {
        sm.onChangePosition += SetLoreScale;
        sm.onChangePosition += SetGreyHandMarkerScale;
        sm.onChangeZoom += SetLoreScale;
        sm.onChangeZoom += SetGreyHandMarkerScale;
        minZoom = sm.m_MinZoom;
        maxZoom = sm.m_MaxZoom;
    }


    void OnEnable()
    {

        if (!isCreated)
        {
            foreach (var item in PlayerDataManager.config.gardens)
            {
                var g = Utilities.InstantiateObject(gardenPrefab, container, 0);

                g.name = item.id;
                g.transform.position = sm.GetWorldPosition(item.longitude, item.latitude);
                g.transform.localEulerAngles = new Vector3(0, 0, 180);
                g.GetComponentInChildren<TextMeshPro>().text = DownloadedAssets.gardenDict[item.id].title;
            }
            for (int i = 0; i < greyHandOffices.Length; i++)
            {
                var greyHand = Utilities.InstantiateObject(greyHandMarker, container.parent);
                greyHand.name = greyHandOffices[i].officeLocation;
                greyHand.transform.position = sm.GetWorldPosition(greyHandOffices[i].officeLongitude, greyHandOffices[i].officeLatitude);
                Debug.Log("created grey hand office at: " + greyHand.transform.position);
                greyHandOfficesTrans[i] = greyHand.transform;
            }
            var loreT = Utilities.InstantiateObject(lorePrefab, container.parent);
            loreT.name = "lore";
            loreT.transform.position = sm.GetWorldPosition(PlayerDataManager.config.explore.longitude, PlayerDataManager.config.explore.latitude);
            Debug.Log("|||||| Created Lore at : " + loreT.transform.position);
            loreTransform = loreT.transform;
            isCreated = true;
            loreT.SetActive(false);
        }

    }

    void SetGreyHandMarkerScale()
    {
        for (int i = 0; i < greyHandOffices.Length; i++)
        {
            if (MapUtils.inMapView(greyHandOfficesTrans[i].position, camera))
            {
                if (camera.orthographicSize <= visibleZoom)
                {
                    greyHandOfficesTrans[i].gameObject.SetActive(true);

                    float clampZoom = Mathf.Clamp(camera.orthographicSize, minVisibleZoom, visibleZoom);
                    float multiplier = MapUtils.scale(minScale, maxScale, minVisibleZoom, visibleZoom, clampZoom);
                    greyHandOfficesTrans[i].localScale = Vector3.one * multiplier * 3;
                }
                else
                {
                    greyHandOfficesTrans[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void SetLoreScale()
    {
        if (MapUtils.inMapView(loreTransform.position, camera))
        {
            if (camera.orthographicSize <= visibleZoom)
            {
                loreTransform.gameObject.SetActive(true);

                float clampZoom = Mathf.Clamp(camera.orthographicSize, minVisibleZoom, visibleZoom);
                float multiplier = MapUtils.scale(minScale, maxScale, minVisibleZoom, visibleZoom, clampZoom);
                loreTransform.localScale = Vector3.one * multiplier;
            }
            else
            {
                loreTransform.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
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

