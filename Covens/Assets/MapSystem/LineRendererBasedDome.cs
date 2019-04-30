using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererBasedDome : MonoBehaviour
{
	public static LineRendererBasedDome Instance;
    [SerializeField, HideInInspector] private LineRenderer m_LineRenderer;
    [SerializeField] private int m_Points = 20;
    [SerializeField] private float m_Radius = 1275;

    private void OnValidate()
    {
        if (m_LineRenderer == null)
            m_LineRenderer = GetComponent<LineRenderer>();
    }

    private void Awake()
    {
		Instance = this;
        LoginAPIManager.OnCharacterInitialized += LoginAPIManager_OnCharacterInitialized;
    }

    private void Start()
    {
        MapsAPI.Instance.OnEnterStreetLevel += OnStopFlying;
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
    }

    private void LoginAPIManager_OnCharacterInitialized()
    {
        Setup(PlayerDataManager.DisplayRadius * GeoToKmHelper.OneKmInWorldspace);
    }

    public void Setup(float radiusInWorldspace)
    {
        m_Radius = radiusInWorldspace;
        SetupDome();
    }

    private void OnStartFlying()
    {
        if (PlayerManager.marker == null)
            return;

        transform.SetParent(MapsAPI.Instance.mapCenter);
        transform.localPosition = Vector3.zero;
    }

    private void OnStopFlying()
    {
        if (PlayerManager.marker == null)
            return;

        transform.SetParent(PlayerManager.marker.gameObject.transform);
        transform.localPosition = Vector3.zero;
    }

    [ContextMenu("Setup dome")]
    public void SetupDome()
    {
        float spacing = (360f / m_Points) * Mathf.Deg2Rad;
        Vector3[] points = new Vector3[m_Points + 1];

        float angle = 0;
        int i;
        for (i = 0; i < points.Length - 1; i++)
        {
            points[i].x = (m_Radius * Mathf.Cos(angle));
            points[i].z = -4;
            points[i].y = (m_Radius * Mathf.Sin(angle));
            angle += spacing;
        }
        points[i] = points[0];

        m_LineRenderer.positionCount = points.Length;
        m_LineRenderer.SetPositions(points);

        if (PlayerDataManager.playerData == null)
            return;

		//changing dome Color based on
		if (PlayerDataManager.playerData.degree > 0) {
			m_LineRenderer.startColor = new Color (1f, 0.59f, 0f);
			m_LineRenderer.endColor = new Color (1f, 0.59f, 0f);
			//Debug.Log ("color.yellow= " + Color.yellow);
		} else if (PlayerDataManager.playerData.degree < 0) {
			m_LineRenderer.startColor = new Color (0.9f, 0f, 1f);
			m_LineRenderer.endColor = new Color (0.9f, 0f, 1f);
		} else {
			m_LineRenderer.startColor = new Color (0.47f, 0.68f, 1f);
			m_LineRenderer.endColor = new Color (0.47f, 0.68f, 1f);
		}
    }
}
