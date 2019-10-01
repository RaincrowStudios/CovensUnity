using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Store;

[RequireComponent(typeof(SwipeDetector)), RequireComponent(typeof(CanvasGroup))]
public class UIStoreStylesWindow : MonoBehaviour
{
    private SwipeDetector m_SwipeDetector;
    int m_CurrentIndex;

    public CanvasGroup canvasGroup => this.GetComponent<CanvasGroup>();

    public RectTransform rectTransform => this.GetComponent<RectTransform>();

    public float alpha
    {
        get => canvasGroup.alpha;
        set => canvasGroup.alpha = value;
    }

    private void Awake()
    {
        m_SwipeDetector = this.GetComponent<SwipeDetector>();

        alpha = 0;

        m_SwipeDetector.SwipeLeft = OnSwipeLeft;
        m_SwipeDetector.SwipeRight = OnSwipeRight;
    }

    private void OnSwipeLeft()
    {
        if (m_CurrentIndex < StoreManagerAPI.Store.Styles.Count - 1)
            m_CurrentIndex++;
        else
            m_CurrentIndex = 0;

        SetupStyle(StoreManagerAPI.Store.Styles[m_CurrentIndex]);
    }

    private void OnSwipeRight()
    {
        if (m_CurrentIndex > 0)
            m_CurrentIndex--;
        else
            m_CurrentIndex = StoreManagerAPI.Store.Styles.Count - 1;

        SetupStyle(StoreManagerAPI.Store.Styles[m_CurrentIndex]);
    }

    private void SetupStyle(StoreItem item)
    {
        CosmeticData cosmetic = DownloadedAssets.GetCosmetic(item.id);
        Debug.Log("setup style " + item.id);
    }
}
