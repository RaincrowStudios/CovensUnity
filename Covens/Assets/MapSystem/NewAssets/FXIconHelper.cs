using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXIconHelper : MonoBehaviour
{
    public Transform m_RotScale;
    public Transform m_Position;

    void Update ()
    {
        if (m_RotScale != null)
        {
            this.transform.localScale = m_RotScale.lossyScale;
            this.transform.rotation = m_RotScale.rotation;
        }

        if (m_Position != null)
        {
            this.transform.position = m_Position.position;
        }
	}
}
