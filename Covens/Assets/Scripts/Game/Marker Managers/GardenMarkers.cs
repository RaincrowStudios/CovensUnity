using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.GameEventResponses;

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
    //bool isCreated = false;

    [SerializeField] float loreScale = 1f;
    [SerializeField] float minLoreZoom = .6f;
    [SerializeField] float gardenScale = .6f;
    [SerializeField] float forbiddenScale = .6f;
    [SerializeField] float minForbiddenZoom = .6f;

    private IMarker loreMarker;
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

    public GameObject greyHandMarker;
    public GreyHandOfficeData[] greyHandOffices = new GreyHandOfficeData[3];
    private Transform[] greyHandOfficesTrans = new Transform[3];

    private List<Transform> gardensTransform = new List<Transform>();
    private List<GardenData> gardens = new List<GardenData>();

    IMaps map;

    private void Awake()
    {
        instance = this;
    }

    private void OnStartFlying()
    {
        map.OnCameraUpdate += OnMapUpdate;
    }

    private void OnStopFlying()
    {
        map.OnCameraUpdate -= OnMapUpdate;
        checkLoreOnLand();

        SetLoreScale();
        updateGardenScale();
        SetGreyHandMarkerScale();
    }

    public void SetupGardens()
    {
        map = MapsAPI.Instance;

        map.OnExitStreetLevel += OnStartFlying;
        map.OnEnterStreetLevel += OnStopFlying;

        foreach (var item in DownloadedAssets.gardenDict)
        {
            var g = Utilities.InstantiateObject(gardenPrefab, map.trackedContainer, 0);
            g.name = item.Key;
            g.transform.position = map.GetWorldPosition(item.Value.longitude, item.Value.latitude);
            g.transform.localEulerAngles = new Vector3(90, 0, 180);
            g.GetComponentInChildren<TextMeshPro>().text = LocalizeLookUp.GetGardenName(item.Key);

            gardensTransform.Add(g.transform);
            gardens.Add(item.Value);
        }

        for (int i = 0; i < greyHandOffices.Length; i++)
        {
            string officeName = greyHandOffices[i].officeLocation;
            var greyHand = Utilities.InstantiateObject(greyHandMarker, map.trackedContainer);
            greyHand.name = "[greyhand] " + greyHandOffices[i].officeLocation;
            greyHand.transform.position = map.GetWorldPosition(greyHandOffices[i].officeLongitude, greyHandOffices[i].officeLatitude);
            greyHand.transform.Rotate(90, 0, 0);
            greyHandOfficesTrans[i] = greyHand.transform;

            MuskMarker marker = greyHandMarker.GetComponent<MuskMarker>();
            if (marker == null)
                marker = greyHand.AddComponent<MuskMarker>();
            marker.OnClick = (m) => OnClickGreyOffice(officeName);
            marker.Coords = new Vector2(greyHandOffices[i].officeLongitude, greyHandOffices[i].officeLatitude);
        }
        SetGreyHandMarkerScale();

        Debug.Log("setup explore quests");
        QuestsController.GetQuests(error =>
        {
            if (string.IsNullOrEmpty(error))
                SetupExplore(QuestsController.Quests.explore);
        });
    }

    private void SetupExplore(QuestsController.CovenDaily.Explore lore)
    {

        if (PlayerDataManager.playerData.quest.explore.completed)
            return;

        if (loreMarker == null)
        {
            var go = Utilities.InstantiateObject(lorePrefab, map.trackedContainer);

            go.name = "[lore] EXPLORE QUEST";
            go.transform.position = map.GetWorldPosition(lore.location.longitude, lore.location.latitude);
            go.SetActive(false);

            loreMarker = go.GetComponent<MuskMarker>();
            if (loreMarker == null)
                loreMarker = go.AddComponent<MuskMarker>();
        }

        loreMarker.OnClick = (m) => SendQuestLore();
        loreMarker.Coords = new Vector2(lore.location.longitude, lore.location.latitude);
        SetLoreScale();
    }


    private void OnMapUpdate(bool a, bool b, bool c)
    {
        SetLoreScale();
        updateGardenScale();
        SetGreyHandMarkerScale();
    }

    void checkLoreOnLand()
    {
        if (loreMarker == null)
            return;

        Debug.Log("check lore on land:\n" + map.position + "\n" + loreMarker.Coords);

        if (!PlayerDataManager.playerData.quest.explore.completed &&
            map.DistanceBetweenPointsD(map.position, new Vector2(loreMarker.Coords.x, loreMarker.Coords.y)) < 8)
        {
            SendQuestLore();
        }
    }

    void SetGreyHandMarkerScale()
    {
        for (int i = 0; i < greyHandOffices.Length; i++)
        {
            if (map.normalizedZoom > minForbiddenZoom)
            {
                greyHandOfficesTrans[i].localScale = Vector3.one * forbiddenScale * MapLineraScale.linearMultiplier;
                greyHandOfficesTrans[i].transform.position = map.GetWorldPosition(greyHandOffices[i].officeLongitude, greyHandOffices[i].officeLatitude);
                greyHandOfficesTrans[i].gameObject.SetActive(!map.streetLevel);
            }
            else
            {
                greyHandOfficesTrans[i].gameObject.SetActive(false);
            }
        }
    }


    void updateGardenScale()
    {
        for (int i = 0; i < gardens.Count; i++)
        {
            gardensTransform[i].position = map.GetWorldPosition(gardens[i].longitude, gardens[i].latitude);
            gardensTransform[i].localScale = Vector3.one * gardenScale * MapLineraScale.linearMultiplier;
            gardensTransform[i].gameObject.SetActive(!map.streetLevel);
        }
    }

    void SetLoreScale()
    {
        if (loreMarker == null)
            return;

        if (map.normalizedZoom >= minLoreZoom)
        {
            loreMarker.GameObject.transform.position = map.GetWorldPosition(QuestsController.Quests.explore.location.longitude, QuestsController.Quests.explore.location.latitude);
            loreMarker.GameObject.transform.localScale = Vector3.one * loreScale * MapLineraScale.linearMultiplier;
            loreMarker.GameObject.SetActive(!map.streetLevel);
        }
        else
        {
            loreMarker.GameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && map != null && !map.streetLevel)
        {
            RaycastHit hit;

            Ray ray = map.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "garden")
                {
                    OnClick(hit.transform.name);
                }
            }
        }
    }

    private void SendQuestLore()
    {
        if (loreMarker == null)
            return;

        loreMarker.Interactable = false;
        loreMarker.SetAlpha(0.5f, 0.2f);

        QuestsController.CompleteExplore(error =>
        {
            if (string.IsNullOrEmpty(error))
            {

                if (loreMarker == null)
                    return;

                loreMarker.SetAlpha(0, 1f, () =>
                {
                    loreMarker.GameObject.SetActive(false);
                });
            }
            else
            {
                loreMarker.Interactable = true;
                loreMarker.SetAlpha(1, 1f);
                UIGlobalPopup.ShowError(null, error);
            }
        });
    }

    public void OnClick(string id)
    {
        img.gameObject.SetActive(false);
        gardenCanvas.SetActive(true);
        title.text = LocalizeLookUp.GetGardenName(id);
        desc.text = LocalizeLookUp.GetGardenDesc(id);
        StartCoroutine(GetImage(id));
    }

    private void OnClickGreyOffice(string name)
    {
        GreyHandOffice.Show(name);
    }

    IEnumerator GetImage(string id)
    {
        WWW www = new WWW(DownloadAssetBundle.baseURL + "gardens/" + id + ".png");
        yield return www;
        img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        img.gameObject.SetActive(true);
    }
}

