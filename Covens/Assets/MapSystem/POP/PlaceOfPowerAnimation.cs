using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPowerAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_GroundGlyph;
    [SerializeField] private SpriteRenderer[] m_Shadows;

    private int m_PoPTweenId;

    private void Awake()
    {
        Color aux;
        foreach (SpriteRenderer _shadow in m_Shadows)
        {
            aux = _shadow.color;
            aux.a = 0;
            _shadow.color = aux;
        }

        aux = m_GroundGlyph.color;
        aux.a = 0;
        m_GroundGlyph.color = aux;
        m_GroundGlyph.transform.localScale = Vector3.one * 0 * 25;
    }

    [ContextMenu("Show")]
    public void Show()
    {
        MapsAPI.Instance.ScaleBuildings(0);

        LeanTween.cancel(m_PoPTweenId);

        float v2;
        Color aux;

        m_PoPTweenId = LeanTween.value(0f, 1f, 1f).setEaseOutCubic()
            .setOnStart(() =>
            {
                gameObject.SetActive(true);
            })
            .setOnUpdate((float v) =>
            {
                v2 = v * v;

                foreach (SpriteRenderer _shadow in m_Shadows)
                {
                    aux = _shadow.color;
                    aux.a = v2;
                    _shadow.color = aux;
                }

                aux = m_GroundGlyph.color;
                aux.a = v;
                m_GroundGlyph.color = aux;
                m_GroundGlyph.transform.localScale = Vector3.one * v * 25;
            })
            .uniqueId;
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        MapsAPI.Instance.ScaleBuildings(1);
        
        float v2;
        Color aux;

        LeanTween.cancel(m_PoPTweenId);

        m_PoPTweenId = LeanTween.value(1f, 0f, 1f).setEaseInCubic()
            .setOnUpdate((float v) =>
            {
                v2 = v * v;

                foreach (SpriteRenderer _shadow in m_Shadows)
                {
                    aux = _shadow.color;
                    aux.a = v2;
                    _shadow.color = aux;
                }

                aux = m_GroundGlyph.color;
                aux.a = v;
                m_GroundGlyph.color = aux;
                m_GroundGlyph.transform.localScale = Vector3.one * v * 25;
            })
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
            })
            .uniqueId;
    }

}
