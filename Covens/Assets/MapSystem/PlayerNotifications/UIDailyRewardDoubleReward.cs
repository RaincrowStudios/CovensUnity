using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDailyRewardDoubleReward : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _textItemName;
    [SerializeField] private Image _iconItem;
    [SerializeField] private Image _flareEffect;
    [SerializeField] private CanvasGroup _bandge2X;

    int amount;
    string textTitle;
    public void Setup(string text, int amount, Color colorFlare ,Sprite icon)
    {
        _canvasGroup.alpha = 0;
        _bandge2X.gameObject.SetActive(false);
        _iconItem.sprite = icon;
        _textItemName.text = string.Format(text, amount);
        _flareEffect.color = colorFlare;

        gameObject.SetActive(true);
        LeanTween.alphaCanvas(_canvasGroup, 1, 1);

        this.amount = amount;
        textTitle = text;
    }

    public void UpdateIcon(Sprite icon)
    {
        _iconItem.sprite = icon;
    }

    public void DoubleItem()
    {       
        _textItemName.text = string.Format(textTitle, amount*2);
        _bandge2X.gameObject.SetActive(true);
        LeanTween.alphaCanvas(_bandge2X,1, 1);
    }
}
