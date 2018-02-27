using System;
using UnityEngine;

public class CustomTileSetControl : OnlineMapsTileSetControl
{
	private Vector3 m_originalPosition;
	private Quaternion m_originalRotation;
	private Vector3 m_originalScale;
	//private float m_nextScaleModifier = 1;
    private bool originalZoomOnDoubleClick;
    private int needRestoreZoom = 0;

	public void Awake ()
	{
		m_originalPosition = transform.localPosition;
		m_originalRotation = transform.localRotation;
		m_originalScale = transform.localScale;
	}

    protected override void OnEnableLate()
    {
        base.OnEnableLate();

        originalZoomOnDoubleClick = zoomInOnDoubleClick;
    }

    private int CheckMapSize(int z)
    {
        if (z < 3) return CheckMapSize(3);

        int max = (1 << z) * OnlineMapsUtils.tileSize;
        int w = map.tilesetWidth;
        int h = map.tilesetHeight;
        if (max < w || max < h) return CheckMapSize(z + 1);

        return z;
    }

    protected override void UpdateGestureZoom ()
	{
		if (!smoothZoom)
        {
			base.UpdateGestureZoom ();
			return;
		}
		
		if (!allowUserControl) return;

	    if (needRestoreZoom > 0)
	    {
	        needRestoreZoom--;
	        if (needRestoreZoom == 0) zoomInOnDoubleClick = originalZoomOnDoubleClick;
	    }
		
		if (Input.touchCount == 2)
        {
			isMapDrag = false;
            if (!smoothZoomStarted) zoomInOnDoubleClick = false;

			smoothZoomStarted = true;
            waitZeroTouches = true;

            Vector2 p1 = Input.GetTouch(0).position;
			Vector2 p2 = Input.GetTouch(1).position;
			float distance = (p1 - p2).magnitude;
			
			if (Math.Abs(lastGestureDistance) > float.Epsilon)
            {
				Vector2 center = Vector2.Lerp (p1, p2, 0.5f);

				RaycastHit hit;
				if (!cl.Raycast (activeCamera.ScreenPointToRay (center), out hit, OnlineMapsUtils.maxRaycastDistance)) return;

				float scale;

                double px, py;
                GetCoords(out px, out py, center);

                if (!invertTouchZoom) scale = distance / lastGestureDistance;
				else scale = lastGestureDistance / distance;

				float initialScale = transform.localScale.x;
				transform.localScale *= scale;

                int z = CheckMapSize(map.zoom - 1);
                if (map.zoom == z && transform.localScale.x < 1)  transform.localScale = Vector3.one;

                float floatOffset = Mathf.Log (transform.localScale.x, 2);
				int offset = floatOffset > 0 ? Mathf.FloorToInt (floatOffset) : Mathf.CeilToInt (floatOffset);

                bool zoomChanged = false;

                if (offset != 0)
                {
					ZoomOnPoint (offset, center);
                    //m_nextScaleModifier = Mathf.Pow (2, -offset);
                    transform.localScale *= Mathf.Pow(2, -offset);
                    zoomChanged = true;
                }
				
				if (transform.localScale.x < 0.9)
                {
					ZoomOnPoint (offset - 1, center);
                    transform.localScale *= 2;
                    //m_nextScaleModifier = 2;
                    zoomChanged = true;
                }

				float scaleDifference = transform.localScale.x / initialScale - 1;
				Vector3 smoothZoomOffset = transform.position - hit.point;
				transform.position += new Vector3 (smoothZoomOffset.x, 0, smoothZoomOffset.z) * scaleDifference;

                if (zoomChanged)
                {
                    double npx, npy;
                    GetCoords(out npx, out npy, center);
                    map.projection.CoordinatesToTile(px, py, map.zoom, out px, out py);
                    map.projection.CoordinatesToTile(npx, npy, map.zoom, out npx, out npy);
                    double cpx, cpy;
                    map.GetPosition(out cpx, out cpy);
                    map.projection.CoordinatesToTile(cpx, cpy, map.zoom, out cpx, out cpy);
                    cpx -= npx - px;
                    cpy -= npy - py;
                    map.projection.TileToCoordinates(cpx, cpy, map.zoom, out cpx, out cpy);
                    map.SetPosition(cpx, cpy);
                }

				ComputePinSizes (transform.localScale.x);
			}
			lastGestureDistance = distance;
		}
        else
        {
			if (smoothZoomStarted)
            {
				lastGestureDistance = 0;
				Vector2 center = new Vector2 (Screen.width / 2, Screen.height / 2);
				double ox, oy;
                if (transform.localScale.x < 0.9)
                {
                    ZoomOnPoint(1, center);
                    transform.localScale *= 2;
                }
                if (GetCoords (out ox, out oy, center))
                {
					map.SetPosition (ox, oy);
					float scaleDifference = m_originalScale.x - transform.localScale.x;
					transform.localPosition = m_originalPosition + new Vector3 (map.tilesetSize.x / -2, 0, map.tilesetSize.y / 2) * scaleDifference;
				}
				ComputePinSizes (transform.localScale.x);
                needRestoreZoom = 3;
            }
			smoothZoomStarted = false;
        }
	}

	public void ResetZoom ()
	{
		transform.localPosition = m_originalPosition;
		transform.localRotation = m_originalRotation;
		transform.localScale = m_originalScale;
	}

	private void ComputePinSizes (float scale)
	{
		/*float inverseScale = 1 / (scale + 1);
		for (int i = 0; i < OnlineMaps.instance.markers.Length; i++) {
			OnlineMaps.instance.markers [i].scale *= inverseScale;
		}*/
	}
}