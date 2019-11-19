using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class SyncCameraProperties : MonoBehaviour
{
    [SerializeField] private Camera m_TargetCamera;

    private Camera m_Camera;
    public bool m_FieldOfView;

    private void Awake()
    {
        m_Camera = this.GetComponent<Camera>();

        if (m_TargetCamera == null)
        {
            Debug.LogError("No target camera set");
            this.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (m_FieldOfView)
            m_Camera.fieldOfView = m_TargetCamera.fieldOfView;
    }
}
