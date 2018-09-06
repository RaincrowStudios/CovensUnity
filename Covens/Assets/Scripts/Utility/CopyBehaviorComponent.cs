using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// it just disables/enables the children following this component behavior
/// </summary>
public class CopyBehaviorComponent : MonoBehaviour
{
    public GameObject[] m_Children;

    private void OnEnable()
    {
        foreach (var pGO in m_Children)
            pGO.SetActive(true);
    }

    private void OnDisable()
    {
        foreach (var pGO in m_Children)
            pGO.SetActive(false);
    }

}
