using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMoveTo : MonoBehaviour
{
    [SerializeField] private RectTransform m_Panel;
    [SerializeField] private RectTransform m_DebugButtonRT;

    [SerializeField] private Button m_DebugButton;
    [SerializeField] private Button m_DisableBuildingButton;
    [SerializeField] private Button m_EnableBuildingButton;

    [SerializeField] private Button m_MoveButton;
    [SerializeField] private Button m_ResetButton;
    [SerializeField] private TMP_InputField m_Longitude;
    [SerializeField] private TMP_InputField m_Latitude;

    private bool m_Showing = false;

    private void Awake()
    {
        m_Panel.anchoredPosition = new Vector2(0, 0);
        m_DebugButtonRT.anchoredPosition = new Vector2(0, 0);

        m_DebugButton.onClick.AddListener(() =>
        {
            m_Showing = !m_Showing;
            if (m_Showing) m_Panel.gameObject.SetActive(m_Showing);
            float start = m_Showing ? 0 : 1;
                LeanTween.value(start, 1 - start, 0.25f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_Panel.anchoredPosition = new Vector2(-m_Panel.sizeDelta.x * t, 0);
                    m_DebugButtonRT.anchoredPosition = new Vector2(-m_Panel.sizeDelta.x * t, 0);
                })
                .setOnComplete(() =>
                {
                    m_Panel.gameObject.SetActive(m_Showing);
                });
        });

        m_MoveButton.onClick.AddListener(() =>
        {
            double longitude = double.Parse(m_Longitude.text);
            double latitude = double.Parse(m_Latitude.text);

            PlayerManager.marker.position = new Vector2((float)longitude, (float)latitude);
            MarkerManagerAPI.GetMarkers(false, false);
        });

        m_ResetButton.onClick.AddListener(() =>
        {
            m_Longitude.text = "-122.4023";
            m_Latitude.text = "37.7749";
        });

        m_DisableBuildingButton.onClick.AddListener(() =>
        {
            MapController.Instance.m_StreetMap.EnableBuildings(false);
        });
        m_EnableBuildingButton.onClick.AddListener(() =>
        {
            MapController.Instance.m_StreetMap.EnableBuildings(true);
        });

        enabled = false;
    }
}
