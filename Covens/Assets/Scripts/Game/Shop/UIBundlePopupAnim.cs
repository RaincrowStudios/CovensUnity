using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBundlePopupAnim : MonoBehaviour
{
    [SerializeField] private CanvasGroup BundleInterCG;
    [SerializeField] private GameObject container;
    [SerializeField] private CanvasGroup containerCG;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject oldPriceSlash;
    [SerializeField] private GameObject newPrice;
    [SerializeField] private CanvasGroup newPriceCG;
    [SerializeField] private GameObject ribbon;
    [SerializeField] private Button close;

    //private int p = 0;

    //public static UIBundlePopupAnim Instance { get; set; }

    private int m_TweenId;
    private int m_ContentTweenId;
    
    void Awake()
    {
        //LeanTween.alphaCanvas(BundleInterCG, 1f, 0.4f).setOnComplete(() => { AnimIn(); });

        BundleInterCG.alpha = 0;
        containerCG.alpha = 0;
        container.transform.localScale = Vector3.zero;
        ribbon.GetComponent<CanvasGroup>().alpha = 0;
        ribbon.transform.localScale = Vector3.zero;
        oldPriceSlash.GetComponent<CanvasGroup>().alpha = 0;
        oldPriceSlash.transform.localScale = Vector3.zero;
    }

    public void Show()
    {
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_ContentTweenId);

        //hdie content items
        CanvasGroup[] items = content.GetComponentsInChildren<CanvasGroup>();
        foreach (var cg in items)
            cg.alpha = 0;

        //fade container
        m_TweenId = LeanTween.value(container.transform.localScale.x, 1, 0.7f)
            .setOnUpdate((float t) =>
            {
                BundleInterCG.alpha = LeanTween.easeOutCubic(0,1,t);
                container.transform.localScale = Vector3.one * LeanTween.easeOutCubic(0,1,t);
                containerCG.alpha = t;
            })
            .uniqueId;

        //fade content
        CanvasGroup ribbonCG = ribbon.GetComponent<CanvasGroup>();
        CanvasGroup oldPriceCG = oldPriceSlash.GetComponent<CanvasGroup>();
        m_ContentTweenId = LeanTween.value(ribbonCG.alpha, 1f, 0.6f)
            .setEaseOutCubic()
            .setDelay(0.2f)
            .setOnUpdate((float t) =>
            {
                ribbonCG.alpha = oldPriceCG.alpha = t;
                ribbon.transform.localScale = oldPriceSlash.transform.localScale = Vector3.one * LeanTween.easeInCubic(0, 1, t);
            })
            .setOnComplete(() => ShowContent())
            .uniqueId;
    }

    private void ShowContent(int idx = 0)
    {
        CanvasGroup[] items = content.GetComponentsInChildren<CanvasGroup>();

        if (idx >= items.Length)
            return;

        m_TweenId = LeanTween.alphaCanvas(items[idx], 1f, 0.25f).setOnComplete(() => ShowContent(idx + 1)).uniqueId;
    }

    public void Hide(System.Action onComplete)
    {
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_ContentTweenId);

        //fade main canvas
        m_TweenId = LeanTween.value(BundleInterCG.alpha, 0, 0.35f)
            .setOnUpdate((float t) =>
            {
                BundleInterCG.alpha = t;
                containerCG.alpha = t;
                container.transform.localScale = Vector3.one * LeanTween.easeOutCubic(0, 1, t);
            })
            .setOnComplete(() =>
            {
                ribbon.GetComponent<CanvasGroup>().alpha = 0;
                ribbon.transform.localScale = Vector3.zero;
                oldPriceSlash.GetComponent<CanvasGroup>().alpha = 0;
                oldPriceSlash.transform.localScale = Vector3.zero;
                onComplete?.Invoke();
            })
            .uniqueId;
    }

    // Update is called once per frame
    //private void AnimIn()
    //{
    //    LeanTween.scale(container, new Vector3(1f, 1f, 1f), 0.6f);
    //    LeanTween.alphaCanvas(containerCG, 1f, 0.7f).setOnComplete(() => { ShowContent(); });
    //}
    //public void AnimOut()
    //{
    //    LeanTween.scale(container, new Vector3(0f, 0f, 0f), 0.35f);
    //    LeanTween.alphaCanvas(containerCG, 0f, 0.4f);
    //}
    //public void ShowContent()
    //{
    //    LeanTween.alphaCanvas(ribbon.GetComponent<CanvasGroup>(), 1f, 0.6f);
    //    LeanTween.scale(ribbon, new Vector3(1f, 1f, 1f), 0.6f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => { ShowTextContent(); });
    //    LeanTween.alphaCanvas(oldPriceSlash.GetComponent<CanvasGroup>(), 1f, 0.6f);
    //    LeanTween.scale(oldPriceSlash, new Vector3(1f, 1f, 1f), 0.6f).setEase(LeanTweenType.easeInCubic);//.setOnComplete(() =>
    //                                                                                                     //{
    //                                                                                                     //LeanTween.alphaCanvas(newPriceCG, 1f, 0.6f).setOnComplete(() =>
    //                                                                                                     //LeanTween.scale(newPrice, new Vector3(1f, 1f, 1f), 0.4f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
    //                                                                                                     //{
    //                                                                                                     // var y = newPrice.AddComponent<FadeInOutHalf>();
    //                                                                                                     // var u = newPrice.AddComponent<ScaleInOutHalf>();
    //                                                                                                     // u.frequency = 0.5f;
    //                                                                                                     // y.frequency = 0.5f;
    //                                                                                                     //});
    //                                                                                                     //  });

    //}
    //private void ShowTextContent()
    //{
    //    LeanTween.alphaCanvas(content[p], 1f, 0.6f).setEase(LeanTweenType.easeOutCubic);
    //    p++;
    //    if (p != content.Length)
    //        Invoke("ShowTextContent", 0.3f);
    //}
    //public void Close()
    //{
    //    Debug.Log("closing");
    //    AnimOut();
    //    LeanTween.value(0f, 1f, 1.5f).setOnComplete(() =>
    //      {
    //          Destroy(this);
    //      });
    //}

}
