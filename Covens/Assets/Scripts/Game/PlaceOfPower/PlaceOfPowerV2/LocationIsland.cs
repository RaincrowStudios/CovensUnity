using System.Collections.Generic;
using UnityEngine;

public class LocationIsland : MonoBehaviour
{

    [SerializeField] private LeanTweenType leanType;
    [SerializeField] private LineRenderer m_Renderer;
    [SerializeField] private Transform[] spots;
    [SerializeField] private GameObject m_Highlight;


    public Transform[] Setup(float distance, int islandIndex)
    {
        Transform moveTransform = transform.GetChild(0);
        distance += Random.Range(-30, 30);
        LeanTween.value(0, 1, 1).setOnUpdate((float value) =>
         {

             moveTransform.localPosition = new Vector3(Mathf.Lerp(0, distance, value), 0, 0);
             moveTransform.localScale = Vector3.one * Mathf.Lerp(.2f, 1, value);
         }).setEase(leanType);
        SetSpiritConnection(false);
        for (int i = 0; i < spots.Length; i++)
        {
            spots[i].GetComponentInChildren<LocationPosition>().Setup(i, islandIndex);
        }
        return spots;
    }

    public void SetSpiritConnection(bool isActive)
    {
        m_Renderer.enabled = isActive;
        if (isActive)
        {
            m_Renderer.positionCount = 2;
            LeanTween.value(0, 1, 1).setOnUpdate((float value) =>
            {
                m_Renderer.SetPosition(1, Vector3.Lerp(transform.position, Vector3.zero, value));
            });
        }
    }

    public void ActivateIsland(bool isActive)
    {
        m_Highlight.SetActive(isActive);
    }

}