using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupLocationTip : MonoBehaviour
{
    public GameObject atLocation_UI;
    public static PopupLocationTip Instance { get; set; }
    public GameObject Panel;
    //	public TextMeshProUGUI text;
    public CanvasGroup PanelCG;
    private Vector3 pre;
    private Vector3 post;


    // Use this for initialization
    void Start()
    {
        Panel.GetComponent<Button>().onClick.AddListener(() =>
        {
            PopupAnimOut();
        });
        Instance = this;
        atLocation_UI = gameObject;
        pre = new Vector3(.6f, .6f, .6f);
        post = new Vector3(1f, 1f, 1f);
        //text.text = text;
        //text = LocalizeLookUp.GetText ("chat_at_location");
        Panel = atLocation_UI.transform.GetChild(0).gameObject;
        //LeanTween.moveLocalX(atLocation_UI, 100f, 0.1f);
        //LeanTween.moveLocalX(Panel, 730f, 0.01f);
        PanelCG = Panel.GetComponent<CanvasGroup>();

        PanelCG.alpha = 0;

        Panel.transform.localScale = pre;
        PopupAnimIn();

    }
    void PopupAnimIn()
    {
        LeanTween.alphaCanvas(PanelCG, 1f, .5f).setEase(LeanTweenType.easeInCubic);
        //LeanTween.moveLocalX (Panel, 0f, 0.5f);
        LeanTween.scale(Panel, post, .5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.value(0f, 1f, 5.5f).setOnComplete(() =>
        {
            PopupAnimOut();
        });
    }// Update is called once per fram
    void PopupAnimOut()
    {
        //LeanTween.moveLocalX (Panel, 200f, 0.3f);
        LeanTween.scale(Panel, pre, .3f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.alphaCanvas(PanelCG, 0f, .3f).setEase(LeanTweenType.easeOutCubic);
    }
}
