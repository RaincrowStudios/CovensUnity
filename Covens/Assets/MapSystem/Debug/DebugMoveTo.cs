using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMoveTo : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private RectTransform m_Panel;

    [SerializeField] private Button m_DebugButton;
    [SerializeField] private Button m_DisableBuildingButton;
    [SerializeField] private Button m_EnableBuildingButton;

    [SerializeField] private Button m_MoveButton;
    [SerializeField] private Button m_ResetButton;
    [SerializeField] private TMP_InputField m_Longitude;
    [SerializeField] private TMP_InputField m_Latitude;

    private bool m_Showing = false;
    private int m_TweenId;

    private void Awake()
    {
#if PRODUCTION
        Destroy(this.gameObject);
#else
        DontDestroyOnLoad(this.gameObject);

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_Panel.anchoredPosition = new Vector2(0, 0);

        m_DebugButton.onClick.AddListener(OnClickOpen);

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
            Debug.Log("todo: enable buildings");
            //MapController.Instance.m_StreetMap.EnableBuildings(false);
        });

        m_EnableBuildingButton.onClick.AddListener(() =>
        {
            Debug.Log("todo: enable buildings");
            //MapController.Instance.m_StreetMap.EnableBuildings(true);
        });
#endif
    }

    private void OnClickOpen()
    {
        LeanTween.cancel(m_TweenId);

        m_Showing = !m_Showing;

        if (m_Showing)
            m_Canvas.enabled = true;

        float start = m_Showing ? 0 : 1;

        m_TweenId = LeanTween.value(start, 1 - start, 0.25f)
        .setEaseOutCubic()
        .setOnUpdate((float t) =>
        {
            m_Panel.anchoredPosition = new Vector2(-m_Panel.sizeDelta.x * t, 0);
        })
        .setOnComplete(() =>
        {
            m_Canvas.enabled = m_InputRaycaster.enabled = m_Showing;
        })
        .uniqueId;
    }

    private float m_Delta;
    private float m_LastPosition;
    private float m_StartTime;

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.BackQuote))
        {
            OnClickOpen();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.touchCount == 3)
            {
                m_Delta = 0;
                m_LastPosition = Input.mousePosition.x;
                m_StartTime = Time.time;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.touchCount == 3)
            {
                m_Delta += Input.mousePosition.x - m_LastPosition;
                m_LastPosition = Input.mousePosition.x;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Mathf.Abs(m_Delta) > Screen.width / 8f && Time.time - m_StartTime < 0.3f)
            {
                m_Delta = 0;
                OnClickOpen();
            }
        }
    }
}
