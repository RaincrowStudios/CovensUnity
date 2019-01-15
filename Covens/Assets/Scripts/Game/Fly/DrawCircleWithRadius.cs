using UnityEngine;

public class DrawCircleWithRadius : MonoBehaviour
{
    public GameObject circle;
    public float diameterInKM = 0.5f;
    public float sizeY = 0.02f;
    
	public void UpdateSize ()
    {
		Vector2 distance = MapsAPI.Instance.DistanceBetweenPoints(MapsAPI.Instance.topLeftPosition, MapsAPI.Instance.bottomRightPosition);

		float scaleX = diameterInKM / distance.x * MapsAPI.Instance.tilesetSize.x;
		float scaleY = diameterInKM / distance.y * MapsAPI.Instance.tilesetSize.y;
		float scale = (scaleX + scaleY) / 2;

		circle.transform.localScale = new Vector3(scale, sizeY, scale);
    }
}
