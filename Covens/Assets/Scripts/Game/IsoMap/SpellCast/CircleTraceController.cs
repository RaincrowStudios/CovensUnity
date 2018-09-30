using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CircleTraceController : MonoBehaviour {

	bool m_isTraceSuccess = false;
	
	private double m_centerX = Screen.width / 2.0;
	private double m_centerY = Screen.height / 2.0;

	private double m_lastX;
	private double m_lastY;
	private double m_lastAngle;

	private double m_angleSum;
	private bool m_isMouseDown = false;

	private double m_x;
	private double m_y;
	private double m_angle;

	public bool isClockwise; 
	public Text energyTitle;
	public CanvasGroup CastButton;
	public GameObject fx;
	public int traces = 0;
	public Camera cam;
	public CastingSound CS;
	public float distancefromcamera = 5;
	public GameObject magic;
	GameObject magicTrace;
	Material  PSMat; 
	Color curColor;
	bool isFirst = true;
	public GameObject magicShadow;

	private void ResetMetrics(){
		m_isTraceSuccess = false; 
		m_x = Input.mousePosition.x - m_centerX;
		m_y = Input.mousePosition.y - m_centerY;
		m_angle = betterAtan2(m_x,m_y);

		m_lastX = m_x;
		m_lastY = m_y;
		m_lastAngle  = m_angle;
		
		m_angleSum = 0.0;
	}


	private double betterAtan2(double x, double y) {
		double angle =Mathf.Atan2((float)(x), (float)(y));
	
		angle = (angle < 0 ? 2*Mathf.PI + angle : angle) * 360/Mathf.PI;
		return angle;
	}

	void OnEnable()
	{
		energyTitle.text = "";

	} 

	void OnDisable()
	{
		if (CS != null) {
			CS.enabled = false;
		}
		if(magicTrace!=null)
			Destroy (magicTrace);
	}

	void CreateParticle ( GameObject type )
	{
		if (magicTrace != null) {
			magicTrace.GetComponent<MouseFollow> ().DisableDestroy ();
		}
		var targetPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distancefromcamera);
		targetPos = cam.ScreenToWorldPoint (targetPos);
//		magic.GetComponent<Renderer> ().sharedMaterial.SetColor ("_TintColor", curColor);
		magicTrace = Instantiate (type, targetPos, Quaternion.identity);
//		PSMat = magicTrace.GetComponent<Renderer> ().material;
//		PSMat.SetColor ("_TintColor", Utilities.Purple);

	}

	void Update () {

		if (Input.GetMouseButtonDown(0)  && !m_isTraceSuccess) {
			m_isMouseDown = true;
			ResetMetrics();
			}

		if (m_isMouseDown && !m_isTraceSuccess) {
			m_x = Input.mousePosition.x - m_centerX;
			m_y = Input.mousePosition.y - m_centerY;
			m_angle = betterAtan2(m_x,m_y);
			if ((m_lastAngle - m_angle) > 0) {
				if (!isFirst && !isClockwise) {
					CreateParticle (magicShadow);
				}
				isClockwise = true;
//				if(PSMat!=null)
//					PSMat.SetColor ("_TintColor", Utilities.Orange);
			
//				curColor = Utilities.Orange;
			} else {
				if (!isFirst && isClockwise) {
					CreateParticle (magic);
				}
				isClockwise = false;
//				if(PSMat!=null)
//				PSMat.SetColor ("_TintColor", Utilities.Purple);
//				curColor = Utilities.Purple;
			}

		

			if (Mathf.Abs((float)(m_lastAngle - m_angle)) < 90.0) {
				m_angleSum += m_lastAngle - m_angle;
			}
			m_lastX= m_x;
			m_lastY= m_y;
			m_lastAngle = m_angle;

			if (Mathf.Abs ((float)(m_angleSum)) > 240.0) {
				if (!isClockwise)
					traces++;
				else
					traces--;

				if (traces == 0) {
					if (CastButton.alpha != .3f) {
						CastButton.alpha = .3f;
						fx.SetActive (false);
						energyTitle.text = "";
					}
				} else {
					if (CastButton.alpha != 1) {
						CastButton.alpha = 1;
						fx.SetActive (true);
					}

					if (traces > 0) {
							energyTitle.text = "Empower the Portal : " + (traces * 5).ToString ();
					} else {
							energyTitle.text = "Destroy the Portal : " + (traces * 5).ToString ();
					}
				}
				ResetMetrics();
			}
			isFirst = false;
		}

		if (Input.GetMouseButtonDown (0)) {
			if(isClockwise)
			CreateParticle (magicShadow);
			else
				CreateParticle (magic);

			CS.enabled = true;

		}

		if (Input.GetMouseButtonUp(0)) {
			m_isMouseDown = false;
			IsoPortalUI.traces = traces;
			traces = 0;
			m_isTraceSuccess = (Mathf.Abs((float)(m_angleSum)) > 240.0);

			if (m_isTraceSuccess) {
//				print ("traceComplete");
			}
			CS.enabled = false;
			ResetMetrics();
		}
	}
}
