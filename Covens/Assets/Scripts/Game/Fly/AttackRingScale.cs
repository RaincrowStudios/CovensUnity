using UnityEngine;
using System.Collections;

public class AttackRingScale : MonoBehaviour {
	Raincrow.Maps.IMaps api;
	public float multiplier = 1;
	void OnEnable()
	{
		api = MapsAPI.Instance;
		api.OnChangeZoom += Resize;
		EventManager.OnSmoothZoom += Resize;
		Invoke ("ResizeS", .1f);
	}

	void OnDisable()
	{
		api.OnChangeZoom -= Resize;
		EventManager.OnSmoothZoom -= Resize;
	}

	void OnDestroy()
	{
		api.OnChangeZoom -= Resize;
		EventManager.OnSmoothZoom -= Resize;
	}

	public void RemoveScale()
	{
		api.OnChangeZoom -= Resize;
		EventManager.OnSmoothZoom -= Resize;
	}

	public void Resize ()
	{
		Vector2 distance = MapsAPI.Instance.DistanceBetweenPoints(api.topLeftPosition, api.bottomRightPosition);

		float scaleX = PlayerDataManager.attackRadius / distance.x * api.tilesetSize.x;
		float scaleY = PlayerDataManager.attackRadius / distance.y * api.tilesetSize.y;
		float scale = (scaleX + scaleY) / 2;
		scale *= api.transform.localScale.x;
		transform.localScale = new Vector3(scale*multiplier, scale*multiplier, scale*multiplier);
	}


	public void ResizeS ()
	{
		Vector2 distance = MapsAPI.Instance.DistanceBetweenPoints(api.topLeftPosition, api.bottomRightPosition);

		float scaleX = PlayerDataManager.attackRadius / distance.x * api.tilesetSize.x;
		float scaleY = PlayerDataManager.attackRadius / distance.y * api.tilesetSize.y;
		float scale = (scaleX + scaleY) / 2;
		scale *= api.transform.localScale.x;
		transform.localScale = new Vector3(scale*multiplier, scale*multiplier, scale*multiplier);
	}
}


