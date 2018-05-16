using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShowComponent : MonoBehaviour
{
    public struct Target
    {
        public UIBaseAnimated m_UITarget;
        public GameObject m_GOTarget;
    }

    public Target m_Target;


    public void Show()
    {
        if (m_Target.m_UITarget != null)
        {
            m_Target.m_UITarget.Show();
            return;
        }

        if(m_Target.m_GOTarget != null)
        {
            m_Target.m_GOTarget.SetActive(true);
        }
    }
    public void Close()
    {
        if (m_Target.m_UITarget != null)
        {
            m_Target.m_UITarget.Close();
            return;
        }

        if (m_Target.m_GOTarget != null)
        {
            m_Target.m_GOTarget.SetActive(false);
        }
    }
}
