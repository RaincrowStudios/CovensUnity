using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollSnapController : MonoBehaviour {

	public RectTransform center;
	public RectTransform container;

	public int loopThreshold = 1000;
	public int loopThresholdAlpha = 200;
	public float SnapSpeed = 10;

	[HideInInspector]
	public List<RectTransform> bttn = new List<RectTransform>();
	float[] distance;
	float[] distanceReposition;
	bool dragging = false;
	int bttnDistance;
	int minButtonNum;
	public bool isSpellCarousel = true;
	public bool isActive = true;

	public void Initialize()
	{
		int bttnCount = bttn.Count;
		distance = new float[bttnCount];
		distanceReposition = new float[bttnCount];
		bttnDistance = (int)Mathf.Abs (bttn [1].anchoredPosition.x - bttn [0].anchoredPosition.x);
	}

	void Update()
	{
		if (!isActive)
			return;
		for (int i = 0; i < bttn.Count; i++) {
			distanceReposition[i] = center.position.x - bttn [i].position.x;
			distance [i] = Mathf.Abs (distanceReposition[i]);
			if (distanceReposition [i]>loopThreshold) {
				float curX = bttn [i].anchoredPosition.x;
				float curY = bttn [i].anchoredPosition.y;
				Vector2 newAPos = new Vector2 (curX + (bttn.Count * bttnDistance), curY);
				bttn [i].anchoredPosition = newAPos;
			}

			if (distanceReposition [i]<-loopThreshold) {
				float curX = bttn [i].anchoredPosition.x;
				float curY = bttn [i].anchoredPosition.y;
				Vector2 newAPos = new Vector2 (curX - (bttn.Count * bttnDistance), curY);
				bttn [i].anchoredPosition = newAPos;
			}
		}
		float minDistance = Mathf.Min (distance);
//		float maxDistance = Mathf.Max (distance);
		for (int a = 0; a < bttn.Count; a++) {
			if (minDistance == distance [a]) {
				minButtonNum = a;
			}
		}

		for (int a = 0; a < bttn.Count; a++) {
			float s = Mathf.Lerp (0, 1, Mathf.InverseLerp (loopThreshold, 0, distance [a]));
			float alpha = Mathf.Lerp (0, 1, Mathf.InverseLerp (loopThresholdAlpha, 0, distance [a]));
			bttn[a].transform.localScale = Vector3.one*s;
			bttn [a].GetComponent<CanvasGroup>().alpha = alpha;
			if (a == minButtonNum) {
				if (isSpellCarousel) {
					SpellCarouselManager.currentSpellData = PlayerDataManager.playerData.spellsDict [bttn [a].name];
					SpellCarouselManager.Instance.SetupSpellInfo ();
					SpellCarouselManager.Instance.ManageSpellButton (true, bttn [a].name);

				} else {
				}
			} else {
				if (isSpellCarousel) {
					SpellCarouselManager.Instance.ManageSpellButton (false, bttn [a].name);
				} else {
				
				}
			}
		}

		if (!dragging) {
			LerpToBttn(-bttn[minButtonNum].anchoredPosition.x);
		}
	}

	void LerpToBttn(float position)
	{
		float newX = Mathf.Lerp (container.anchoredPosition.x, position, Time.deltaTime * SnapSpeed);
		Vector2 newPosition = new Vector2 (newX, container.anchoredPosition.y);
		container.anchoredPosition = newPosition;
	}

	public void StartDragging()
	{
		dragging = true;
	}

	public void StopDragging()
	{
		dragging = false;
	}
}
