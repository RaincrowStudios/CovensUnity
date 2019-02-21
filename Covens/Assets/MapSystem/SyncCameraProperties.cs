using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class SyncCameraProperties : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private Camera m_TargetCamera;

    public bool m_FieldOfView;

    private void OnValidate()
    {
        m_Camera = this.GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (m_TargetCamera == null)
            return;

        if (m_FieldOfView)
            m_Camera.fieldOfView = m_TargetCamera.fieldOfView;
    }
}
