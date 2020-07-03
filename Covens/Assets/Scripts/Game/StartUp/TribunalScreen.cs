using TMPro;
using UnityEngine;

public class TribunalScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _tribunalTimer;
    [SerializeField] private TextMeshProUGUI _tribunalTitle;

    private int leenAnimationID;
    public void Setup(string title, string time)
    {
        _tribunalTitle.text = title;
        _tribunalTimer.text = time;
        _canvasGroup.alpha = 0;
        _canvasGroup.gameObject.SetActive(true);
        LeanTween.cancel(leenAnimationID);
        leenAnimationID = LeanTween.alphaCanvas(_canvasGroup, 1f, 1f).setEaseOutCubic().uniqueId;
    }

    public void Hide(System.Action onComplete)
    {
        LeanTween.cancel(leenAnimationID);
        leenAnimationID = LeanTween.alphaCanvas(_canvasGroup, 0f, 1f).setEaseOutCubic().setOnComplete(onComplete).uniqueId;
    }

    public void CancelAnimation()
    {
        LeanTween.cancel(leenAnimationID);
    }
}
