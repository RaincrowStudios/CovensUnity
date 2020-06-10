using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckConnection : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button _buttonRetry;
    [SerializeField] private TMPro.TextMeshProUGUI _textButton;
    [SerializeField] private float _timeToRetry = 15;

    private Coroutine _waitConnection;
    public void CheckInternetConnection(System.Action onConnect)
    {
        if (IsConnected())
        {
            onConnect?.Invoke();
        } else
        {
            gameObject.SetActive(true);
            _waitConnection = StartCoroutine(WaitConnection(onConnect));
        }

        _buttonRetry.onClick.RemoveAllListeners();
        _buttonRetry.onClick.AddListener(()=> {
            OnClickRetry(onConnect);
        });
    }

    private IEnumerator WaitConnection(System.Action onConnect)
    {
        bool reconnected = false;
        float time = _timeToRetry;
        string text = DownloadedAssets.LocalizationDictionary["button_retry_connection"];
        while (!reconnected)
        {
            time -= Time.deltaTime;
            _textButton.text = string.Format(text, (int)time);

            if(time <= 0)
            {
                reconnected = IsConnected();
                time = _timeToRetry;
            }

            yield return null;
        }

        onConnect?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnClickRetry(System.Action onConnect)
    {
        StopCoroutine(_waitConnection);

        CheckInternetConnection(onConnect);
    }

    private bool IsConnected()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
