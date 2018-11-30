using System;
using UnityEngine;
using UnityEngine.UI;

public class TeamConfirmPopUp : MonoBehaviour
{
    public static TeamConfirmPopUp Instance { get; set; }
    public Button confirm;
    public Button cancel;
    public Text title;
    public Text error;
    public UnityEngine.GameObject Container;

    void Awake()
    {
        Instance = this;
    }

    public void ShowPopUp(Action confirmAction, Action cancelAction, string txt)
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);

        title.text = txt;
        confirm.gameObject.SetActive(true);
        Container.SetActive(true);
        confirm.onClick.AddListener(delegate { Confirm(confirmAction); });
        cancel.onClick.AddListener(delegate { Cancel(cancelAction); });
    }

    public void ShowPopUp(Action cancelAction, string txt)
    {
        title.text = txt;
        Container.SetActive(true);
        confirm.gameObject.SetActive(false);
        cancel.onClick.AddListener(delegate { Cancel(cancelAction); });
    }

    void Cancel(Action confirmAction)
    {
        confirmAction();
        Container.SetActive(false);
    }

    void Confirm(Action cancelAction)
    {
        cancelAction();
        Container.SetActive(false);
    }

    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { gameObject.SetActive(false); });
    }


    public void Error(string err)
    {
        error.text = error.text;
    }
}