using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPrinceOfMerchantsManager : MonoBehaviour
{

    public GameObject PrinceChar;
    public CanvasGroup StairsBG;
    public Button close;
    public CanvasGroup[] secondaryFades;
    public CanvasGroup[] contentCG;
    public int p = 0;

    public static UIPrinceOfMerchantsManager Instance { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        close.onClick.AddListener(() => Close());
        LeanTween.value(0f, 1f, 1f).setOnComplete(() =>
          {
              AnimateShow();
          });
        LeanTween.value(0f, 1f, 2.5f).setOnComplete(() =>
        {
            AnimateShowContent();
        });
    }

    // Update is called once per frame
    public void Close()
    {
        LeanTween.alphaCanvas(PrinceChar.GetComponent<CanvasGroup>(), 0f, 1f).setOnComplete(() =>
        //LeanTween.alphaCanvas(StairsBG, 0f, 2f)
        {
            LeanTween.alphaCanvas(this.GetComponent<CanvasGroup>(), 0f, 0.4f).setOnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        });
    }

    public void AnimateShow()
    {
        LeanTween.alphaCanvas(this.transform.GetChild(0).GetComponent<CanvasGroup>(), 1f, 0.4f).setOnComplete(() =>
        {
            //LeanTween.moveLocalX(PrinceChar, 75f, 1f);
            LeanTween.alphaCanvas(StairsBG, 1f, 0.7f).setOnComplete(() =>
            {
                LeanTween.alphaCanvas(PrinceChar.GetComponent<CanvasGroup>(), 1f, 1f);
                foreach (CanvasGroup i in secondaryFades)
                {
                    LeanTween.alphaCanvas(i, 1f, 1f);
                }
            });
        });

    }
    void AnimateShowContent()
    {
        LeanTween.alphaCanvas(contentCG[p], 1f, 0.5f);
        p = p + 1;
        Invoke("AnimateShowContent", 0.5f);
    }
}
