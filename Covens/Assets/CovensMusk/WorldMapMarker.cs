using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapMarker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_NearRenderer;
    [SerializeField] private SpriteRenderer m_FarRenderer;

    public SpriteRenderer nearRenderer { get { return m_NearRenderer; } }
    public SpriteRenderer farRenderer { get { return m_FarRenderer; } }
}
