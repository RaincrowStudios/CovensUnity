using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundleManager : MonoBehaviour
{
    public CanvasGroup BundleInterCG;
    public GameObject container;
    public CanvasGroup containerCG;
    public CanvasGroup[] content;
    public GameObject oldPriceSlash;
    public GameObject newPrice;
    public CanvasGroup newPriceCG;
    public GameObject ribbon;
    public Button close;
    int p = 0;

    public static BundleManager Instance { get; set; }


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        LeanTween.alphaCanvas(BundleInterCG, 1f, 0.4f).setOnComplete(() => { AnimIn(); });
    }

    // Update is called once per frame
    public void AnimIn()
    {
        LeanTween.scale(container, new Vector3(1f, 1f, 1f), 0.6f);
        LeanTween.alphaCanvas(containerCG, 1f, 0.7f).setOnComplete(() => { ShowContent(); });
    }
    public void AnimOut()
    {
        LeanTween.scale(container, new Vector3(0f, 0f, 0f), 0.35f);
        LeanTween.alphaCanvas(containerCG, 0f, 0.4f);
    }
    public void ShowContent()
    {
        LeanTween.alphaCanvas(ribbon.GetComponent<CanvasGroup>(), 1f, 0.6f);
        LeanTween.scale(ribbon, new Vector3(1f, 1f, 1f), 0.6f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => { ShowTextContent(); });
        LeanTween.alphaCanvas(oldPriceSlash.GetComponent<CanvasGroup>(), 1f, 0.6f);
        LeanTween.scale(oldPriceSlash, new Vector3(1f, 1f, 1f), 0.6f).setEase(LeanTweenType.easeInCubic);//.setOnComplete(() =>
                                                                                                         //{
                                                                                                         //LeanTween.alphaCanvas(newPriceCG, 1f, 0.6f).setOnComplete(() =>
                                                                                                         //LeanTween.scale(newPrice, new Vector3(1f, 1f, 1f), 0.4f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
                                                                                                         //{
                                                                                                         // var y = newPrice.AddComponent<FadeInOutHalf>();
                                                                                                         // var u = newPrice.AddComponent<ScaleInOutHalf>();
                                                                                                         // u.frequency = 0.5f;
                                                                                                         // y.frequency = 0.5f;
                                                                                                         //});
                                                                                                         //  });

    }
    private void ShowTextContent()
    {
        LeanTween.alphaCanvas(content[p], 1f, 0.6f).setEase(LeanTweenType.easeOutCubic);
        p++;
        if (p != content.Length)
            Invoke("ShowTextContent", 0.3f);
    }
    public void Close()
    {
        Debug.Log("closing");
        AnimOut();
        LeanTween.value(0f, 1f, 1.5f).setOnComplete(() =>
          {
              Destroy(this);
          });
    }

}
