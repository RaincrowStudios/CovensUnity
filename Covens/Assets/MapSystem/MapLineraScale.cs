using Raincrow.Maps;
using UnityEngine;

public class MapLineraScale : MonoBehaviour
{
    private IMaps map;
    public static float linearMultiplier = 0;
    private float[] zoom = new float[] { 0.05f, 0.16f, 0.226f, 0.335f, 0.467f, 0.541f, 0.632f, 0.71f, 0.78f, 0.85f, 0.9f };
    private float[] scale = new float[] { 31600, 21000, 13000, 6000, 2100, 1050, 350, 100, 25, 6.2f, 4.1f };

    public void Initialize()
    {
        map = MapsAPI.Instance;
        map.OnChangeZoom += GetLinearScale;
    }

    private void GetLinearScale()
    {
        if (map.streetLevel) return;
        int index = GetLowerIndex();
        try
        {
            linearMultiplier = MapUtils.scale(scale[index - 1], scale[index], zoom[index - 1], zoom[index], map.normalizedZoom);

        }
        catch (System.Exception)
        {
            // throw;
        }
    }

    private int GetLowerIndex()
    {
        for (int i = 0; i < zoom.Length; i++)
        {
            if (zoom[i] > map.normalizedZoom) return i;
        }
        return -1;
    }
}