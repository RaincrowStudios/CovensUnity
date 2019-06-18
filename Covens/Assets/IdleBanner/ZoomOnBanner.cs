using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomOnBanner : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Awake()
    {
        button.onClick.AddListener(() => {
            ShowBanner();
        });
    }

    // Update is called once per frame
    public void ShowBanner()
    {
        MapCameraUtils.SetZoom(1f, 2f, false);
        MapCameraUtils.SetRotation(30f, 2, false, null);
    }
}
