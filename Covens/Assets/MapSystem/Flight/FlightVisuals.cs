using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlightVisuals : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private Transform m_FlyFxObj;
    [SerializeField] private SpriteRenderer m_PlayerPortrait;

   // [Header("UI")]
   // [SerializeField] private Canvas m_Canvas;
   // [SerializeField] private TextMeshProUGUI m_LabelTitle;
   // [SerializeField] private TextMeshProUGUI m_LabelSubtitle;

	public Transform Particles;
	public GameObject FlyFX;
	//public SpriteRenderer fx;
	public SpriteRenderer fx1;
	//public GameObject UIFlyGlow;

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

	public void Start()
	{
		
		//LeanTween.scale (FlyFX, Vector3.zero, 0.1f);
	}
    private void Awake()
    {
		
        //gameObject.SetActive(false);
		IconFXColor ();
		//Debug.Log ("Awake");
    }

    private void OnEnable()
    {
        MapsAPI.Instance.OnChangePosition = OnMapPan;
        m_LastMapPosition = MapsAPI.Instance.GetWorldPosition();
		//Debug.Log ("OnEnable");

    }

    private void OnDisable()
    {
		//Debug.Log ("OnDisable");
        MapsAPI.Instance.OnChangePosition -= OnMapPan;
    }

    private void OnMapPan()
    {
		//Debug.Log ("OnMapPan");
        Vector3 newPos = MapsAPI.Instance.GetWorldPosition();
        Vector3 delta = (newPos - m_LastMapPosition);
        m_LastMapPosition = newPos;

    }
	public void IconFXColor() {
		//Debug.Log ("IconFXColor");

		if (PlayerDataManager.playerData.degree > 0) {
//<<<<<<< Updated upstream
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

    public void StartFlight()
    {
		FlyFX = transform.GetChild (0).GetChild (0).gameObject;
		//Debug.Log ("StartFlight");

        m_LastMapPosition = MapsAPI.Instance.GetWorldPosition();
        gameObject.SetActive(true);
		LeanTween.scale (FlyFX, Vector3.one, 0.3f);
        (PlayerManager.marker as WitchMarker).GetPortrait(spr => m_PlayerPortrait.sprite = spr);
		DeathState.Instance.FlightGlowOn ();
    }

    public void EndFlight()
    {
		//Debug.Log ("EndFlight");
		DeathState.Instance.FlightGlowOff ();
		LeanTween.scale (FlyFX, Vector3.zero, 0.6f).setOnComplete(() => {
			gameObject.SetActive(false);
		});
    }

    public void UpdateUI(string title, string subtitle)
    {
		//Debug.Log ("UpdateUI");
  //      m_LabelTitle.text = title;
   //     m_LabelSubtitle.text = subtitle;
    }
}
