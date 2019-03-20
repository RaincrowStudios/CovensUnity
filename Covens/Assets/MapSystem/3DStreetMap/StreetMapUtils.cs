using Mapbox.Unity.Map;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetMapUtils : MonoBehaviour
{
    private static StreetMapUtils m_Instance;

    [SerializeField] private AbstractMap m_Map;
    [SerializeField] private MapCameraController m_Controller;
    [Header("other")]
    [SerializeField] private Camera m_HighlightCamera;
    [SerializeField] private SpriteRenderer m_HighlightSprite;

    private int m_HightlighTweenId;
    public static int markersLayer;
    public static int highlightsLayer;

    private void Awake()
    {
        m_Instance = this;

        m_HighlightCamera.enabled = false;
        m_HighlightSprite.color = new Color(0, 0, 0, 0);
        markersLayer = LayerMask.NameToLayer("MapMarkers");
        highlightsLayer = LayerMask.NameToLayer("HighlightMarker");
    }

    public static void FocusOnPosition(Vector3 worldPosition, bool clampZoom, float zoom, bool allowCancel)
    {
        m_Instance.m_Controller.SetPosition(worldPosition, 0.6f, allowCancel);
        m_Instance.m_Controller.SetZoom(zoom, clampZoom, 0.6f, allowCancel);
    }

    public static void FocusOnTarget(IMarker marker)
    {
        if (marker == null)
            return;

        if (marker.gameObject == null)
            return;

        bool redcap = marker.customData != null && (marker.customData as Token).redcap;

        FocusOnPosition(
            marker.gameObject.transform.position + m_Instance.m_Controller.CenterPoint.right * (19.1266f + 3) + m_Instance.m_Controller.CenterPoint.forward * (19.5f + (redcap ? 40 : 15)),
            false,
           redcap ? 10 : 9,
            false
        );
    }

    public static void FocusOnTargetCenter(IMarker marker)
    {
        if (marker == null)
            return;

        if (marker.gameObject == null)
            return;

        FocusOnPosition(
            marker.gameObject.transform.position + m_Instance.m_Controller.CenterPoint.forward * 10.5f,
            false,
            9,
            false
        );
    }

    /// <summary>
    /// return the current worldPosition the camera is focused at
    /// </summary>
    public static Vector3 CurrentPosition()
    {
        return m_Instance.m_Controller.CenterPoint.position;
    }

    private static int m_ShakeTweenId;
    public static void ShakeCamera(Vector3 axis, float amount, float periodTime, float duration)
    {
        StopCameraShake();

        LTDescr shake = LeanTween.rotateAroundLocal(
            m_Instance.m_Controller.camera.gameObject,
            axis,
            amount,
            periodTime)
        .setEase(LeanTweenType.easeShake)
        .setLoopClamp()
        .setRepeat(-1);

        m_ShakeTweenId = LeanTween.value(m_Instance.m_Controller.camera.gameObject, amount, 0, duration)
            .setOnUpdate((float t) =>
            {
                shake.setTo(axis * t);
            })
            .setEaseOutQuad()
            .uniqueId;
    }

    public static void StopCameraShake()
    {
        LeanTween.cancel(m_ShakeTweenId);
    }



    public static void Highlight(IMarker[] markers)
    {
        return;

        SpriteRenderer m_HighlightSprite = m_Instance.m_HighlightSprite;
        Camera m_HighlightCamera = m_Instance.m_HighlightCamera;

        LeanTween.cancel(m_Instance.m_HightlighTweenId, true);

        for (int i = 0; i < markers.Length; i++)
        {
            SetLayer(markers[i].gameObject.transform, highlightsLayer);
        }

        m_HighlightCamera.enabled = true;
        Color aux = new Color(0, 0, 0, m_HighlightSprite.color.a);

        m_Instance.m_HightlighTweenId = LeanTween.value(m_HighlightSprite.color.a, 0.6f, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                aux.a = t;
                m_HighlightSprite.color = aux;
            })
            .uniqueId;
    }

    public static void DisableHighlight(IMarker[] markers)
    {
        return;

        SpriteRenderer m_HighlightSprite = m_Instance.m_HighlightSprite;
        Camera m_HighlightCamera = m_Instance.m_HighlightCamera;

        LeanTween.cancel(m_Instance.m_HightlighTweenId, true);
        Color aux = new Color(0, 0, 0, m_HighlightSprite.color.a);

        m_Instance.m_HightlighTweenId = LeanTween.value(m_HighlightSprite.color.a, 0, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                aux.a = t;
                m_HighlightSprite.color = aux;
            })
            .setOnComplete(() =>
            {
                m_HighlightCamera.enabled = false;

                for (int i = 0; i < markers.Length; i++)
                {
                    SetLayer(markers[i].gameObject.transform, markersLayer);
                }
            })
            .uniqueId;
    }

    public static void SetLayer(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
            SetLayer(child, layer);
    }
}
