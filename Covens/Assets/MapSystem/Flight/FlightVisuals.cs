using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlightVisuals : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private Transform m_FlyFxObj;
    [SerializeField] private SpriteRenderer m_PlayerPortrait;
    
	[SerializeField] private Transform Particles;
	[SerializeField] private GameObject FlyFX;
	//public SpriteRenderer fx;
	[SerializeField] private SpriteRenderer fx1;
	//public GameObject UIFlyGlow;
    
    private float[] m_Multipliers = new float[]
    {
        5000,
        5000,
        5000,
        4000,
        3000,
        2048,
        1000,
        560,
        450,
        250,
        128,
        40,
        40,
        8,
        3,
        2,
        2,
        2
    };

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
    
    private void OnMapPan()
    {
        Vector3 newPos = MapsAPI.Instance.GetWorldPosition();
        Vector3 delta = (newPos - m_LastMapPosition);

        m_LastMapPosition = newPos;

        delta = new Vector3(delta.x, delta.z) / m_Multipliers[(int)MapsAPI.Instance.zoom];
        m_FlyFxObj.transform.position += delta * 0.1f;
    }

	public void IconFXColor()
    {
        //Debug.Log ("IconFXColor");
        if (PlayerDataManager.playerData == null)
            return;

		if (PlayerDataManager.playerData.degree > 0) {
			Particles.GetChild (2).gameObject.SetActive (true);
			Particles.GetChild (1).gameObject.SetActive (false);
			Particles.GetChild (0).gameObject.SetActive (false);
			//fx.color = new Color (1f, 1f, 1f);
			fx1.color = new Color (1f, 0.59f, 0f);
			//Debug.Log ("color.yellow= " + Color.yellow);
		} else if (PlayerDataManager.playerData.degree < 0) {
			Particles.GetChild (2).gameObject.SetActive (false);
			Particles.GetChild (1).gameObject.SetActive (true);
			Particles.GetChild (0).gameObject.SetActive (false);
			//fx.color = new Color (1f, 1f, 1f);
			fx1.color = new Color (0.9f, 0f, 1f);
		} else {
			Particles.GetChild (2).gameObject.SetActive (false);
			Particles.GetChild (1).gameObject.SetActive (false);
			Particles.GetChild (0).gameObject.SetActive (true);
			//fx.color = new Color (1f, 1f, 1f);
			fx1.color = new Color (0.47f, 0.68f, 1f);
		}
	}

    private void OnMoveFloatingOrigin()
    {
        m_LastMapPosition = MapsAPI.Instance.GetWorldPosition();
    }

    [ContextMenu("Start flight")]
    public void StartFlight()
    {
        IconFXColor();
        MapsAPI.Instance.OnChangePosition += OnMapPan;
        MapsAPI.Instance.OnMoveOriginPoint += OnMoveFloatingOrigin;

        //Debug.Log ("StartFlight");

        m_LastMapPosition = MapsAPI.Instance.GetWorldPosition();

        gameObject.SetActive(true);
		LeanTween.scale (FlyFX, Vector3.one, 0.3f);
        if (PlayerManager.marker != null)
            (PlayerManager.marker as WitchMarker).GetPortrait(spr => m_PlayerPortrait.sprite = spr);
        if (DeathState.Instance != null)
            DeathState.Instance.FlightGlowOn();
    }

    [ContextMenu("End flight")]
    public void EndFlight()
    {
        MapsAPI.Instance.OnChangePosition -= OnMapPan;
        MapsAPI.Instance.OnMoveOriginPoint -= OnMoveFloatingOrigin;

        //Debug.Log ("EndFlight");
        if (DeathState.Instance != null)
            DeathState.Instance.FlightGlowOff();
		LeanTween.scale (FlyFX, Vector3.zero, 0.6f).setOnComplete(() => {
			gameObject.SetActive(false);
		});
    }
}
