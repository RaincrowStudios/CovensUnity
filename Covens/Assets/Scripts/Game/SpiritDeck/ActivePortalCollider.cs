using UnityEngine;
using System.Collections;

public class ActivePortalCollider : MonoBehaviour
{
public float endScale = 2;
public float endPos = 0;
public float speed = 1;
public float alpha = 1;
public int sortLayer = 205;
public bool isMiddle = false;
void OnTriggerEnter(Collider other)
{
	if (other.tag == "activeportaltrigger") {
		StartCoroutine( SlideCard (other.transform));
			if (isMiddle) {
				other.GetComponent<PortalSetup> ().Focus ();
			} else {
				other.GetComponent<PortalSetup> ().DeFocus ();
			}
	}
}

//void OnTriggerExit(Collider other)
//{
//	if (isMiddle) {
//			if (other.tag == "activeportaltrigger") {
//				other.GetComponent<PortalSetup> ().DeFocus ();
//		}
//	}
//}


IEnumerator SlideCard(Transform tr)
{
		tr.GetChild(0).GetComponentInChildren<Canvas> ().sortingOrder = sortLayer;
	var cg = tr.GetComponent<CanvasGroup> ();
	var childTr = tr.GetChild (0);
	float posx = childTr.transform.localPosition.x;
	float startAlpha = cg.alpha;
	float sc = tr.localScale.x;
	float t = 0;
	while (t<=1) {
		t += Time.deltaTime * speed;
		tr.localScale = Vector3.one * Mathf.SmoothStep (sc, endScale, t);
		cg.alpha = Mathf.SmoothStep (startAlpha, alpha, t);
		childTr.localPosition = new Vector3 (Mathf.SmoothStep (posx, endPos, t),0,0);
		yield return null;
	}
}
}

