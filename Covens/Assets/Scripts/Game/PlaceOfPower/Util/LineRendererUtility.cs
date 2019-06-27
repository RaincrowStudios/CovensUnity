using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineRendererUtility : MonoBehaviour
{
    [SerializeField] private LineRenderer m_Renderer;

    private void OnValidate()
    {
        m_Renderer = this.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        if (Application.isPlaying)
            enabled = false;
    }

    void Update()
    {
        if (transform.childCount != m_Renderer.positionCount)
            m_Renderer.positionCount = transform.childCount;

        for (int i = 0; i < transform.childCount; i++)
        {
                m_Renderer.SetPosition(i, transform.GetChild(i).position);
        }
    }
}
