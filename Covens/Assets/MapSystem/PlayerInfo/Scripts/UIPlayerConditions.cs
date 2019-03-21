using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerConditions : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private UIConditionItem m_ConditionPrefab;
    [SerializeField] private Transform m_Container;
    [SerializeField] private Button m_CloseButton;

    private bool m_IsOpen;
    private SimplePool<UIConditionItem> m_ItemPool;

    public void Open()
    {
        if (m_IsOpen)
            return;

        m_IsOpen = false;

        //Container.parent.parent.gameObject.SetActive(true);
        //anim.Play("in");
        //isClicked = true;
        //SetupConditions();
    }

    public void Close()
    {
        //anim.Play("out");
        //Invoke("DisableClick", .4f);
        //Invoke("ClearItems", 1.5f);
    }
}
