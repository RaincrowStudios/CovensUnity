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
    public GameObject Container;

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
        error.text = "";
        title.text = txt;
        confirm.GetComponentInChildren<Text>().text = "Yes";
        cancel.gameObject.SetActive(true);
        Container.SetActive(true);

        confirm.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();
        confirm.onClick.AddListener(delegate { Confirm(confirmAction); });
        cancel.onClick.AddListener(delegate { Cancel(cancelAction); });
    }

    public void ShowPopUp(Action cancelAction, string txt)
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);

        error.text = "";
        title.text = txt;
        confirm.GetComponentInChildren<Text>().text = "Ok";
        Container.SetActive(true);
        cancel.gameObject.SetActive(false);

        confirm.onClick.RemoveAllListeners();
        confirm.onClick.AddListener(delegate { Cancel(cancelAction); });
    }

    void Cancel(Action confirmAction)
    {
        confirmAction();
        Close();
    }

    void Confirm(Action cancelAction)
    {
        cancelAction();
    }

    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { Container.SetActive(false); });
    }


    public void Error(string err)
    {
        error.text = "Error: " + err;
    }
}