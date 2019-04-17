using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlightVisuals : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private Transform m_FlyFxObj;
    [SerializeField] private SpriteRenderer m_PlayerPortrait;

    [Header("UI")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private TextMeshProUGUI m_LabelTitle;
    [SerializeField] private TextMeshProUGUI m_LabelSubtitle;

    private static FlightVisuals m_Instance;
    public static FlightVisuals Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<FlightVisuals>("FlightVisuals"));
            return m_Instance;
        }
    }

    private Vector3 m_LastMapPosition;
    private Vector3 m_TargetPosition;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        MapsAPI.Instance.OnChangePosition = OnMapPan;
    }

    private void OnDisable()
    {
        MapsAPI.Instance.OnChangePosition -= OnMapPan;
    }

    private void OnMapPan()
    {
        Vector3 newPosition = MapsAPI.Instance.GetWorldPosition();
        Vector3 delta = newPosition - m_LastMapPosition;
        m_LastMapPosition = newPosition;

        delta.y = delta.z;
        delta.z = 0;

        m_TargetPosition = m_FlyFxObj.position + delta.normalized * Time.deltaTime * (120 * (1 - MapsAPI.Instance.normalizedZoom) + 65 * MapsAPI.Instance.normalizedZoom);
    }

    private void Update()
    {
        m_FlyFxObj.position = Vector3.Lerp(m_FlyFxObj.position, m_TargetPosition, Time.deltaTime * 20);
    }

    public void StartFlight()
    {
        m_LastMapPosition = MapsAPI.Instance.GetWorldPosition();
        gameObject.SetActive(true);
        (PlayerManager.marker as WitchMarker).GetPortrait(spr => m_PlayerPortrait.sprite = spr);
    }

    public void EndFlight()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(string title, string subtitle)
    {
        m_LabelTitle.text = title;
        m_LabelSubtitle.text = subtitle;
    }
}
