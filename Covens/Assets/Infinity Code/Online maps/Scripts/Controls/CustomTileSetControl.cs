using UnityEngine;
using System.Collections;

public class CustomTileSetControl : OnlineMapsTileSetControl
{

	private Vector3 m_mapCenter;
	private float m_nextScaleModifier = 1;


	protected override void UpdateGestureZoom ()
	{

		
		if (Input.touchCount == 2)
        {
			isMapDrag = false;
			smoothZoomStarted = true;

			Vector2 p1 = Input.GetTouch(0).position;
			Vector2 p2 = Input.GetTouch(1).position;
			float distance = (p1 - p2).magnitude;
			
			if (lastGestureDistance != 0)
            {
				Vector2 center = Vector2.Lerp (p1, p2, 0.5f);

				RaycastHit hit;
				if (!cl.Raycast (activeCamera.ScreenPointToRay (center), out hit, OnlineMapsUtils.maxRaycastDistance)) return;

				float scale = 1;

                double px, py;
                GetCoords(out px, out py, center);

                if (!invertTouchZoom) scale = distance / lastGestureDistance;
				else scale = lastGestureDistance / distance;

				float initialScale = transform.localScale.x;
				transform.localScale *= scale * m_nextScaleModifier;

				float floatOffset = Mathf.Log (transform.localScale.x, 2);
				int offset = floatOffset > 0 ? Mathf.FloorToInt (floatOffset) : Mathf.CeilToInt (floatOffset);
				m_nextScaleModifier = 1;

                bool zoomChanged = false;

                if (offset != 0)
                {
                    m_nextScaleModifier = Mathf.Pow (2, -offset);
                    zoomChanged = true;
                }
				
				if (transform.localScale.x < 0.9)
                {
					m_nextScaleModifier = 2;
                    zoomChanged = true;
                }

				float scaleDifference = transform.localScale.x / initialScale - 1;
				Vector3 smoothZoomOffset = transform.position - hit.point;

				if (zoomChanged) {
//					double npx, npy;
//					GetCoords (out npx, out npy, center);
//					map.projection.CoordinatesToTile (px, py, map.zoom, out px, out py);
//					map.projection.CoordinatesToTile (npx, npy, map.zoom, out npx, out npy);
//					double cpx, cpy;
//					map.GetPosition (out cpx, out cpy);
//					map.projection.CoordinatesToTile (cpx, cpy, map.zoom, out cpx, out cpy);
//					cpx -= npx - px;
//					cpy -= npy - py;
//					map.projection.TileToCoordinates (cpx, cpy, map.zoom, out cpx, out cpy);
				} else {
					EventManager.Instance.CallSmoothZoom ();
				}

			}
			lastGestureDistance = distance;
		}
        else
        {
			if (smoothZoomStarted)
            {
				lastGestureDistance = 0;

			}
			smoothZoomStarted = false;
		}
	}

//	public void ResetZoom ()
//	{
//		transform.localPosition = m_originalPosition;
//		transform.localRotation = m_originalRotation;
//		transform.localScale = m_originalScale;
//	}
//
	private void ComputePinSizes (float scale)
	{
		/*float inverseScale = 1 / (scale + 1);
		for (int i = 0; i < OnlineMaps.instance.markers.Length; i++) {
			OnlineMaps.instance.markers [i].scale *= inverseScale;
		}*/
	}
}
