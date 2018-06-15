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
	public SpellTraceManager STM;

	Text text;
	public int traces = 0;

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
		angle = (angle < 0 ? 2*Mathf.PI + angle : angle) * 180/Mathf.PI;
		return angle;
	}

	void Start () {
		text = GetComponent<Text> ();
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
			if (Mathf.Abs((float)(m_lastAngle - m_angle)) < 90.0) {
				m_angleSum += m_lastAngle - m_angle;
			}
			m_lastX= m_x;
			m_lastY= m_y;
			m_lastAngle = m_angle;

			if (Mathf.Abs ((float)(m_angleSum)) > 240.0) {
				traces++;
				PlayerDataManager.playerData.energy -= 5;
				AttackVisualFXManager.Instance.playerEnergy.text = PlayerDataManager.playerData.energy.ToString();
				text.text = "Channeled Energy : " + (traces * 5).ToString ();
				print (traces);
				ResetMetrics();
			}
		}

		if (Input.GetMouseButtonUp(0)) {
			m_isMouseDown = false;
			if (traces > 0) {
				if (SpellCarousel.currentSpell == "spell_attack") {
					SpellCastAPI.PortalAttack (traces * 5);
				} else if (SpellCarousel.currentSpell == "spell_ward") {
					SpellCastAPI.PortalAttack (traces * 5);
				} else {
					SpellCastAPI.CastSpell (traces * 5);
				}
				STM.enabled = false;
			}
			traces = 0;
			m_isTraceSuccess = (Mathf.Abs((float)(m_angleSum)) > 240.0);

			if (m_isTraceSuccess) {
				print ("traceComplete");
			}
		
			ResetMetrics();

		}
	}
}
