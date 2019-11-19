using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapCameraUtils : MonoBehaviour
{
    private static MapCameraUtils m_Instance;

    [SerializeField] private MapCameraController m_Controller;
    [SerializeField] private CovensMuskMap m_Map;

    [Header("Settings")]
    [SerializeField] private Vector2 m_TargetFocusOffset = new Vector2(19.1266f, 19.5f);

    private Material m_MarkerMat;
    private Material m_MarkerMat_Aux;

    private Material m_FontMat;
    private Material m_FontMat_Aux;

    private Material m_EnergyMat;
    private Material m_EnergyMatAux;

    private int m_HighlightTweenId;

    private List<MuskMarker> m_HighlightedMarkers = new List<MuskMarker>();

    private void Awake()
    {
        m_Instance = this;
    }

    private IEnumerator Start()
    {
        while (PlayerManager.marker == null)
            yield return null;

        m_MarkerMat = PlayerManager.witchMarker.AvatarRenderer.sharedMaterial;
        m_MarkerMat_Aux = new Material(m_MarkerMat);

        m_FontMat = PlayerManager.witchMarker.GetComponentInChildren<TextMeshPro>(true).fontSharedMaterial;
        m_FontMat_Aux = new Material(m_FontMat);

        m_EnergyMat = PlayerManager.witchMarker.EnergyRing.sharedMaterial;
        m_EnergyMatAux = new Material(m_EnergyMat);
    }

    public static void FocusOnPosition(Vector3 worldPosition, float normalizedZoom, bool allowCancel, float time = 1f)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
        m_Instance.m_Controller.AnimateZoom(normalizedZoom, time, allowCancel);
    }

    public static void FocusOnPosition(Vector3 worldPosition, bool allowCancel, float time = 1f)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
    }

    public static void FocusOnPosition(float longitude, float latitude, bool allowCancel, float time = 1f)
    {
        m_Instance.m_Controller.AnimatePosition(new Vector2(longitude, latitude), time, allowCancel);
    }

    public static void FocusOnMarker(Vector3 position, float time = 1f)
    {
        FocusOnPosition(
            position + m_Instance.m_Controller.CenterPoint.right * m_Instance.m_TargetFocusOffset.x + m_Instance.m_Controller.CenterPoint.forward * m_Instance.m_TargetFocusOffset.y,
            1,
            false,
            time
        );
    }

    public static void FocusOnTargetCenter(IMarker marker, float time = 1)
    {
        if (marker == null)
            return;

        if (marker.GameObject == null)
            return;

        FocusOnPosition(
            marker.GameObject.transform.position + m_Instance.m_Controller.CenterPoint.forward * 10.5f,
            1f,
            false,
            time
        );
    }

    private static int m_ShakeTweenId;
    private static int m_ResetTweenId;

    public static void ShakeCamera(Vector3 axis, float amount, float periodTime, float duration)
    {
        StopCameraShake();
        LeanTween.cancel(m_ResetTweenId);
        LeanTween.cancel(m_ShakeTweenId);

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
            .setOnComplete(() =>
            {
                LeanTween.cancel(shake.uniqueId);
                m_ResetTweenId = LeanTween.rotateLocal(m_Instance.m_Controller.camera.gameObject, Vector3.zero, 1f).setEaseOutCubic().uniqueId;
            })
            .uniqueId;
    }

    public static void StopCameraShake()
    {
        LeanTween.cancel(m_ResetTweenId);
        LeanTween.cancel(m_ShakeTweenId, true);
    }

    public static void SetLayer(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
            SetLayer(child, layer);
    }
    
    public static void SetRotation(float eulerAngle, float time, bool allowCancel, System.Action onComplete)
    {
        m_Instance.m_Controller.AnimateRotation(eulerAngle, time, allowCancel, onComplete);
    }


    /// <summary>
    /// Use this for short (street level) distances
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="time"></param>
    /// <param name="allowCancel"></param>
    public static void SetPosition(Vector3 worldPosition, float time, bool allowCancel)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
    }

    /// <summary>
    /// Use this for long (world level) distances.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="time"></param>
    /// <param name="allowCancel"></param>
    public static void SetPosition(Vector2 coordinates, float time, bool allowCancel)
    {
        m_Instance.m_Controller.AnimatePosition(coordinates, time, allowCancel);
    }

    public static void SetZoom(float normalizedZoom, float time, bool allowCancel)
    {
        m_Instance.m_Controller.AnimateZoom(normalizedZoom, time, allowCancel);
    }

    public static void SetCameraRotation(Vector3 euler, float time, System.Action onComplete)
    {
        m_Instance.m_Controller.AnimateCamRotation(euler, time, onComplete);
    }

    public static void SetExtraFOV(float value)
    {
        m_Instance.m_Controller.ExtraFOV = value;
    }

    public static void HighlightMarkers(List<MuskMarker> markers)
    {
        m_Instance._HighlightMarkers(markers);
    }

    public static void SetMaterial(MuskMarker marker, Material sprite, Material energy, Material font)
    {
        SpriteRenderer[] renderers = marker.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sharedMaterial = sprite;

        if (marker is CharacterMarker)
            (marker as CharacterMarker).EnergyRing.sharedMaterial = energy;

        TextMeshPro[] texts = marker.GetComponentsInChildren<TextMeshPro>();
        for (int i = 0; i < texts.Length; i++)
            texts[i].fontSharedMaterial = font;
    }

    private void _HighlightMarkers(List<MuskMarker> markers)
    {        
        //reset previous markers
        foreach (var marker in m_HighlightedMarkers)
            SetMaterial(marker, m_MarkerMat, m_EnergyMat, m_FontMat);

        //set targeted markers
        foreach (var marker in markers)
            SetMaterial(marker, m_MarkerMat_Aux, m_EnergyMatAux, m_FontMat_Aux);

        m_HighlightedMarkers = markers;

        //lerp the alpha
        LeanTween.cancel(m_HighlightTweenId);
        float target = markers == null || markers.Count == 0 ? 1 : 0.1f;
        m_HighlightTweenId = LeanTween.value(m_MarkerMat.GetColor("_Color").a, target, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_MarkerMat.SetColor("_Color", new Color(1, 1, 1, t));
                m_EnergyMat?.SetColor("_Color", new Color(t, t, t, 1));
                m_FontMat.SetColor("_FaceColor", new Color(1, 1, 1, t));
                m_FontMat.SetColor("_UnderlayColor", new Color(0, 0, 0, t));
            })
            .uniqueId;

    }

    private void OnDestroy()
    {
        m_MarkerMat?.SetColor("_Color", new Color(1, 1, 1, 1));
        m_EnergyMat?.SetColor("_Color", new Color(1, 1, 1, 1));
        m_FontMat?.SetColor("_FaceColor", new Color(1, 1, 1, 1));
        m_FontMat?.SetColor("_UnderlayColor", new Color(0, 0, 0, 1));
    }
}
