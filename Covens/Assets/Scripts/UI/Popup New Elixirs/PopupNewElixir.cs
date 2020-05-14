using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupNewElixir : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private CanvasGroup _container;
    [SerializeField] private Transform _madameFortuna;
    [SerializeField] private Button _buttonClose;
    [SerializeField] private Button _buttonShop;

    [Header("Animations Time")]
    [SerializeField] private float _backgroundTime = 0.3f;
    [SerializeField] private float _alphaContainerTime = 0.45f;
    [SerializeField] private float _madameAnimationTime = 0.3f;

    [Header("Animations Delay")]
    [SerializeField] private float _alphaContainerDelay = 0.1f;
    [SerializeField] private float _madameAnimationDelay = 0.2f;


    private int _backgroundAnimationId;
    private int _backgroundContainerId;
    private int _madameFortunaAnimationId;

    private Vector3 _madameFortunaInitialPosition;
    private bool _closing;
    // Start is called before the first frame update

    public void Show()
    {
        _buttonClose.onClick.AddListener(Hide);
        _buttonShop.onClick.AddListener(OnClickOpenShop);

        _madameFortunaInitialPosition = _madameFortuna.position;
        Vector3 newPosition = _madameFortunaInitialPosition;
        newPosition.x = Screen.width + _madameFortuna.GetComponent<RectTransform>().rect.width;
        _madameFortuna.position = newPosition;

        _background.color = new Vector4(0, 0, 0, 0);
        _container.alpha = 0;

        gameObject.SetActive(true);

        _backgroundAnimationId = LeanTween.value(0, 0.8f, _backgroundTime).setOnUpdate((a) => _background.color = new Vector4(0, 0, 0, a)).uniqueId;
        _backgroundContainerId = LeanTween.alphaCanvas(_container, 1, _alphaContainerTime).setDelay(_alphaContainerDelay).uniqueId;
        _madameFortunaAnimationId = LeanTween.moveX(_madameFortuna.gameObject, _madameFortunaInitialPosition.x, _madameAnimationTime).setDelay(_madameAnimationDelay).uniqueId;
    }

    private void Hide()
    {
        if (_closing)
        {
            return;
        }

        LeanTween.cancel(_backgroundAnimationId);
        LeanTween.cancel(_backgroundContainerId);
        LeanTween.cancel(_madameFortunaAnimationId);

        _closing = true;

        LeanTween.value(0.8f, 0, _backgroundTime).setOnUpdate((a) => _background.color = new Vector4(0, 0, 0, a));
        LeanTween.moveX(_madameFortuna.gameObject, _madameFortunaInitialPosition.x * 3, _madameAnimationTime);
        LeanTween.alphaCanvas(_container, 0, _alphaContainerTime).setOnComplete(() =>
        {
            _closing = true;
            gameObject.SetActive(false);
        });
    }

    private void OnClickOpenShop()
    {
        UIStore.OpenCharmsStore();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
