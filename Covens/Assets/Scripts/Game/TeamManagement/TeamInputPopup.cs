using System;
using UnityEngine;
using UnityEngine.UI;

public class TeamInputPopup : MonoBehaviour
{
    public Button confirm;
    public Button cancel;
    public InputField input;
    public GameObject Container;
    public Text title;
    public Text error;

    public bool isOpen { get; private set; }

    public void ShowPopUp(Action<string> confirmAction, Action cancelAction, string txt, string initialInput = "")
    {
        error.text = "";
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);
        title.text = txt;
        Container.SetActive(true);
        confirm.gameObject.SetActive(true);

        confirm.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();

        confirm.onClick.AddListener(delegate { Confirm(confirmAction); });
        cancel.onClick.AddListener(delegate { Cancel(cancelAction); });
        input.text = initialInput;
        isOpen = true;
    }

    void Confirm(Action<string> confirmAction)
    {
        confirmAction(input.text);
    }

    void Cancel(Action cancelAction)
    {
        cancelAction();
        Close();
    }

    public void Close()
    {
        isOpen = false;
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { Container.SetActive(false); });
    }

    public void Error(string err)
    {
        error.text = "Error: " + err;
    }
}