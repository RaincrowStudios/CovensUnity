using System.Collections.Generic;
using UnityEngine;

public class LocationIsland : MonoBehaviour
{

    [SerializeField] private LeanTweenType leanType;
    [SerializeField] private LineRenderer m_Renderer;
    [SerializeField] private Transform[] spots;
    [Header("Highlight")]
    [SerializeField] private GameObject m_Highlight;
    [SerializeField] private Transform m_HighlightEdge;
    [SerializeField] private Transform m_HighlightRune;
    [SerializeField] private Transform m_HighlightGlow;
    [SerializeField] private Transform m_HighlightCylinde;

    private bool isConnected = false;

    private bool isActive = false;

    public bool IsActive { get => isActive; private set => isActive = value; }
    public bool IsConnected { get => isConnected; set => isConnected = value; }

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
            var childTransform = transform.GetChild(0);
            LeanTween.value(0, 1, 1).setOnUpdate((float value) =>
            {
                if (childTransform != null)
                {
                    m_Renderer.SetPosition(1, Vector3.Lerp(Vector3.zero, childTransform.position, value));
                }
            });
        }
        isConnected = isActive;
    }

    public void ActivateIsland(bool active)
    {
        m_Highlight.SetActive(active);

        if (!IsActive && active)
        {
            AnimateHighlight(true);
        }
        else if (IsActive && !active)
        {
            AnimateHighlight(false);
        }

        IsActive = active;
    }

    private void AnimateHighlight(bool forward)
    {
        LeanTween.value(forward ? 0 : 1, forward ? 1 : 0, 1.5f).setOnUpdate((float v) =>
        {
            m_HighlightCylinde.localScale = new Vector3(51.5f, Mathf.Lerp(0, 5.7f, v), 51.5f);
            m_HighlightRune.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, Mathf.Lerp(0, 60, v)));
            m_HighlightGlow.localScale = Vector3.one * Mathf.Lerp(40, 65, v);
            m_HighlightEdge.localScale = Vector3.one * Mathf.Lerp(11, 13, v);
        }).setEase(LeanTweenType.easeInOutQuad);
    }
}