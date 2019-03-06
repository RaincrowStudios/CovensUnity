using UnityEngine;

public class DynamicLabelItem : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private Color colorIcon;
    [SerializeField] private Sprite dot;
    [SerializeField] private Sprite icon;
    [SerializeField] private SpriteRenderer sp;
    private Camera cam;
    private SpriteMapsController sm;
    public void Setup(SpriteMapsController sm)
    {
        cam = sm.m_Camera;
        this.sm = sm;
        SetScale();
        this.sm.onChangeZoom += SetScale;
    }

    void SetScale()
    {
        if (cam.orthographicSize > .03f)
        {
            if (sp.color != color)
            {
                sp.sprite = dot;
                sp.color = color;
            }
        }
        else
        {
            if (sp.color != colorIcon)
            {
                sp.color = colorIcon;
                sp.sprite = icon;
            }

        }
    }

    void OnDestroy()
    {
        sm.onChangeZoom -= SetScale;
    }
}