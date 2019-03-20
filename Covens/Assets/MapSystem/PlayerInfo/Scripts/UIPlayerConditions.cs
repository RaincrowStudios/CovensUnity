using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerConditions : MonoBehaviour
{
    private bool m_IsOpen;

    public void Open()
    {
        if (m_IsOpen)
            return;

        m_IsOpen = false;

        Container.parent.parent.gameObject.SetActive(true);
        anim.Play("in");
        isClicked = true;
        SetupConditions();
    }

    public void Close()
    {
        anim.Play("out");
        Invoke("DisableClick", .4f);
        Invoke("ClearItems", 1.5f);
    }
}
