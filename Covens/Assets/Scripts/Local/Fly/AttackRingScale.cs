using UnityEngine;
using System.Collections;

public class AttackRingScale : MonoBehaviour {
	OnlineMaps api;
	public float radius;
	void OnEnable()
	{
		api = OnlineMaps.instance;
//		api.OnChangeZoom += Resize;
		EventManager.OnSmoothZoom += Resize;
		Invoke ("ResizeS", .1f);
	}

	void OnDisable()
	{
//		api.OnChangeZoom -= Resize;
		EventManager.OnSmoothZoom -= Resize;
	}

	void OnDestroy()
	{
//		api.OnChangeZoom -= Resize;
		EventManager.OnSmoothZoom -= Resize;
	}

	public void RemoveScale()
	{
//		api.OnChangeZoom -= Resize;
		EventManager.OnSmoothZoom -= Resize;
	}

	public void Resize ()
	{
		OnlineMaps api = OnlineMaps.instance;

		Vector2 distance = OnlineMapsUtils.DistanceBetweenPoints(api.topLeftPosition, api.bottomRightPosition);

		float scaleX = PlayerDataManager.attackRadius / distance.x * api.tilesetSize.x;
		float scaleY = PlayerDataManager.attackRadius / distance.y * api.tilesetSize.y;
		float scale = (scaleX + scaleY) / 2;
		scale *= api.transform.localScale.x;
		transform.localScale = new Vector3(scale, scale, scale);
	}


	public void ResizeS ()
	{
		OnlineMaps api = OnlineMaps.instance;

		Vector2 distance = OnlineMapsUtils.DistanceBetweenPoints(api.topLeftPosition, api.bottomRightPosition);

		float scaleX = PlayerDataManager.attackRadius / distance.x * api.tilesetSize.x;
		float scaleY = PlayerDataManager.attackRadius / distance.y * api.tilesetSize.y;
		float scale = (scaleX + scaleY) / 2;
		scale *= api.transform.localScale.x;
		transform.localScale = new Vector3(scale, scale, scale);
	}
}


