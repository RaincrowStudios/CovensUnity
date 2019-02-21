using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class createMap : MonoBehaviour
{

    public GameObject tile;
    public Transform point;

    public Vector2 gps;

    void Start()
    {

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                var g = (GameObject)Instantiate(tile, Vector3.zero, Quaternion.identity);
                g.transform.parent = this.transform;
                g.transform.localPosition = new Vector3(MapUtils.scale(-750, 750, 0, 1500, j * 100), MapUtils.scale(-750, 750, 0, 1500, i * 100), 0);
                g.transform.localScale = Vector3.one;

                g.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tiles/" + j.ToString() + "/" + (15 - i).ToString());
            }
        }
        point.localPosition = MapUtils.GPSToUnityUnits(gps);
        // point.transform.parent = this.transform;
//        createcountryLabels.instance.addLabels();
    }


    // void Update()
    // {
    //     point.position = MapUtils.GPSToUnityUnits(gps);
    //     pos = point.position;
    // }

}
