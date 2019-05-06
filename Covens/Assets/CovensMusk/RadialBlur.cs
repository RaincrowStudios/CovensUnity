using UnityEngine;

public class RadialBlur : MonoBehaviour
{
    [SerializeField] private Material material;

    private Material m_MaterialInstance;
    public Material materialInstance
    {
        get
        {
            if (m_MaterialInstance == null)
                m_MaterialInstance = new Material(material);
            return m_MaterialInstance;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, materialInstance);
    }
}