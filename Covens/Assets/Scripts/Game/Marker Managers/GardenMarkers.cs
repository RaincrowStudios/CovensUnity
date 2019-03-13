using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raincrow.Maps;
using TMPro;
public class GardenMarkers : MonoBehaviour
{
    public GameObject gardenPrefab;
    public Transform container;
    public GameObject gardenCanvas;

    public Text title;
    public Image img;
    public Text desc;
    public SpriteMapsController sm;
    bool isCreated = false;
    public Camera camera;
    void Start()
    {

    }

    void OnEnable()
    {

        if (!isCreated)
        {
            foreach (var item in PlayerDataManager.config.gardens)
            {
                var g = Utilities.InstantiateObject(gardenPrefab, container);
                g.name = item.id;
                g.transform.position = sm.GetWorldPosition(item.longitude, item.latitude);
                g.transform.localEulerAngles = new Vector3(0, 0, 180);
                g.GetComponentInChildren<TextMeshPro>().text = DownloadedAssets.gardenDict[item.id].title;
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
                if (hit.transform.tag == "garden")
                {
                    OnClick(hit.transform.name);
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

