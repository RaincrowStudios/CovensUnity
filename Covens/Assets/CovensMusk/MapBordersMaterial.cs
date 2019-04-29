using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBordersMaterial : MonoBehaviour
{
    [SerializeField] private Material m_Material;
    [SerializeField] private CovensMuskMap m_Maps;
    [SerializeField] private MapCameraController m_Controller;

    //[Header("Debug - do not change")]
    private Material m_MaterialInstance;
    
    private void Awake()
    {
        m_Controller.onUpdate += OnMapUpdate;
        m_Controller.onEnterStreetLevel += () => this.enabled = false;
        m_Controller.onExitStreetLevel += () => this.enabled = true;
    }

    private void OnEnable()
    {
        if (Application.isPlaying == false)
            return;

        if (m_MaterialInstance == null)
            m_MaterialInstance = new Material(m_Material);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_MaterialInstance);
    }

    private void OnMapUpdate(bool pos, bool zoom, bool rot)
    {
        double lng, lat;
        m_Maps.GetCoordinates(out lng, out lat);

        Vector2 topLeft = m_Controller.camera.WorldToViewportPoint(m_Maps.topLeftBorder);
        Vector2 botRight = m_Controller.camera.WorldToViewportPoint(m_Maps.botRightBorder);
        Rect bounds = new Rect(0, 0, 1, 1);

        if(lng < -100)
            bounds.x = topLeft.x;
        else if (lng > 100)
            bounds.width = botRight.x;

        if (lat < -30)
            bounds.y = botRight.y;
        else if (lat > 30)
            bounds.height = topLeft.y;

        m_MaterialInstance.SetVector("_Screen", new Vector4(bounds.x, bounds.y, bounds.width, bounds.height));
    }
}
