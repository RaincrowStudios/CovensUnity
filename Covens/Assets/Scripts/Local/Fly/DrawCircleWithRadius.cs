using UnityEngine;

public class DrawCircleWithRadius : MonoBehaviour
{
    public GameObject circle;
    public float diameterInKM = 0.5f;
    public float sizeY = 0.02f;

    private OnlineMapsMarker marker;

	public void AddMarker ()
    {
        marker = OnlineMaps.instance.AddMarker(OnlineMaps.instance.position, "Marker");


    }


	public void UpdateSize ()
    {
		circle.transform.position = CustomTileSetControl.instance.GetWorldPosition(marker.position);

		OnlineMaps api = OnlineMaps.instance;

		Vector2 distance = OnlineMapsUtils.DistanceBetweenPoints(api.topLeftPosition, api.bottomRightPosition);

		float scaleX = diameterInKM / distance.x * api.tilesetSize.x;
		float scaleY = diameterInKM / distance.y * api.tilesetSize.y;
		float scale = (scaleX + scaleY) / 2;

		circle.transform.localScale = new Vector3(scale, sizeY, scale);
    }
}
