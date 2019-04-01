using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapZoomFX : MonoBehaviour
{

    private MapController MC;
    public Material matTop;
    public Material matSide;
    Camera cam;
    // [SerializeField] private PostProcessingBehaviour CC;
    private void Start()
    {
        MC = MapController.Instance;
        cam = MC.m_StreetMap.camera;

        matTop.color = new Color(matTop.color.r, matTop.color.g, matTop.color.b, 1);
        matSide.color = new Color(matSide.color.r, matSide.color.g, matSide.color.b, 1);
        //    centerPoint = MapController.Instance.m_StreetMap.cameraCenter;
        MC.m_StreetMap.OnChangeZoom += ManageZoomFX;
        MC.m_StreetMap.OnChangeZoom += ManageScaling;
        MC.m_StreetMap.OnChangePosition += ManageScaling;
    }
    //13//45
    void ManageZoomFX()
    {
        //scale Markers
        float alpha = MapUtils.scale(0, 1, 45, 13, cam.fieldOfView);
        matTop.color = new Color(matTop.color.r, matTop.color.g, matTop.color.b, alpha);
        matSide.color = new Color(matSide.color.r, matSide.color.g, matSide.color.b, alpha);
    }

    void ManageScaling()
    {
        float scale = MapUtils.scale(2, 1, 45, 13, cam.fieldOfView);
        // PlayerManager.marker.gameObject.transform.localScale = Vector3.one * scale;
        foreach (var item in MarkerManager.Markers)
        {
            if (item.Value[0].gameObject.activeInHierarchy && item.Value[0].gameObject.transform.localScale.x != scale)
            {
                item.Value[0].gameObject.transform.localScale = Vector3.one * scale;
            }
        }
    }
}