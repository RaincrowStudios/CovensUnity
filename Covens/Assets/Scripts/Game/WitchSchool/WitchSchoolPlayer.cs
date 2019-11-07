using Raincrow;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WitchSchoolPlayer : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private MediaPlayerCtrl m_VideoPlayer;
    [SerializeField] private RawImage m_VideoImage;
    [SerializeField] private TextMeshProUGUI m_VideoTitle;
    [SerializeField] private Slider m_Slider;

    [Space(2)]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private CanvasGroup m_PlayCanvasGroup;
    [SerializeField] private CanvasGroup m_PauseCanvasGroup;
    [SerializeField] private CanvasGroup m_StopCanvasGroup;
    [SerializeField] private CanvasGroup m_SliderCanvasGroup;
    [SerializeField] private CanvasGroup m_Loading;

    [Space(2)]
    [SerializeField] private Button m_PlayerButton;
    [SerializeField] private Button m_CloseButton;
    
    private static WitchSchoolPlayer m_Instance;

    public static void Open(string title, string url, System.Action onOpen, System.Action onClose)
    {
        if (m_Instance != null)
        {
            m_Instance._Open(title, url, onOpen, onClose);
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.VIDEO_PLAYER, UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) => { },
                () =>
                {
                    LoadingOverlay.Hide();
                    m_Instance._Open(title, url, onOpen, onClose);
                });
        }
    }

    private int m_TweenId;
    private int m_ButtonTweenId;
    private int m_LoadingTweenId;

    private System.Action m_OnClose;

    void Awake()
    {
        m_Instance = this;

        gameObject.SetActive(false);
        m_InputRaycaster.enabled = false;

        m_PlayCanvasGroup.alpha = 0;
        m_PauseCanvasGroup.alpha = 0;
        m_StopCanvasGroup.alpha = 0;
        m_CanvasGroup.alpha = 0;
        m_SliderCanvasGroup.alpha = 0;

        m_PlayerButton.onClick.AddListener(OnClickPlayer);
        m_CloseButton.onClick.AddListener(Close);
        m_Slider.onValueChanged.AddListener(OnUpdateSlider);
        
        //m_VideoPlayer.OnVideoBuffering = OnVideoBuffering;
        //m_VideoPlayer.OnVideoBufferingEnd = OnVideoBufferingEnd;
        m_VideoPlayer.OnVideoFirstFrameReady = OnVideoReady;
        m_VideoPlayer.OnEnd = OnVideoEnd;
    }

    private void _Open(string title, string url, System.Action onOpen, System.Action onClose)
    {
        Debug.Log($"[VideoPlayer] Loading \"{title}\" ({url})");
        m_OnClose = onClose;

        gameObject.SetActive(true);
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                onOpen?.Invoke();
                StartCoroutine(LoadVideoCoroutine(url));
            })
            .uniqueId;

        Debug.LogError("TODO: PAUSE MAIN AUDIOSOURCE");
        //PlayerDataManager.Instance?.GetComponent<AudioSource>().Pause();
        m_VideoTitle.text = title;
        m_VideoImage.color = Color.black;

        m_Slider.value = 0;
        OnVideoBuffering();

        BackButtonListener.AddCloseAction(Close);
    }

    private void Close()
    {
        BackButtonListener.RemoveCloseAction();

        StopCoroutine("LoadVideoCoroutine");

        m_OnClose?.Invoke();
        m_OnClose = null;
        m_InputRaycaster.enabled = false;
        m_VideoPlayer.UnLoad();

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 0.5f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_VideoPlayer.SetVolume(t);
            })
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
                Debug.LogError("TODO: RESUME MAIN AUDIOSOURCE");
                //PlayerDataManager.Instance?.GetComponent<AudioSource>().UnPause();

                m_TweenId = LeanTween.value(0, 0, 120).setOnComplete(() =>
                {
                    SceneManager.UnloadScene(SceneManager.Scene.VIDEO_PLAYER, null, null);
                }).uniqueId;
            })
            .uniqueId;
    }


    private IEnumerator LoadVideoCoroutine(string url)
    {
        //UpdateState(MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY);
        m_VideoPlayer.m_bAutoPlay = true;
        m_VideoPlayer.Load(url);

        while (m_VideoPlayer.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
            yield return 0;
        yield return 0;

        m_TweenId = LeanTween.value(0, 1, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_VideoPlayer.SetVolume(t);
                m_VideoImage.color = Color.Lerp(Color.black, Color.white, t);
            })
            .uniqueId;
    }

    private void OnVideoBuffering()
    {
        LeanTween.cancel(m_LoadingTweenId);
        m_LoadingTweenId = LeanTween.alphaCanvas(m_Loading, 1f, 0.1f).uniqueId;
    }

    private void OnVideoBufferingEnd()
    {
        LeanTween.cancel(m_LoadingTweenId);
        m_LoadingTweenId = LeanTween.alphaCanvas(m_Loading, 0f, 1f).setEaseOutCubic().uniqueId;
    }

    private void OnVideoReady()
    {
        OnVideoBufferingEnd();
        UpdateState(MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING);
    }

    private void OnVideoEnd()
    {
        m_VideoPlayer.Stop();
        UpdateState(MediaPlayerCtrl.MEDIAPLAYER_STATE.END);
    }

    private void OnUpdateSlider(float value)
    {
        if (m_VideoPlayer.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
        {
            m_VideoPlayer.SetSeekBarValue(value);
            m_VideoPlayer.Play();
            UpdateState(m_VideoPlayer.GetCurrentState());
        }
    }

	private void OnClickPlayer()
	{        
        MediaPlayerCtrl.MEDIAPLAYER_STATE state = m_VideoPlayer.GetCurrentState();
        
        //set new state
        if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
        {
            m_VideoPlayer.Pause();
        }
        else if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
        {
            m_VideoPlayer.Play();
        }
        else if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
        {
            m_VideoPlayer.Stop();
            m_VideoPlayer.Play();
        }
        else if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
        {
            m_VideoPlayer.Play();
        }

        //update ui
        UpdateState(m_VideoPlayer.GetCurrentState());
    }

    private void UpdateState(MediaPlayerCtrl.MEDIAPLAYER_STATE state)
    {
        LeanTween.cancel(m_ButtonTweenId);

        if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
        {
            m_PlayCanvasGroup.alpha = 1f;
            m_PauseCanvasGroup.alpha = 0f;
            m_StopCanvasGroup.alpha = 0f;
            m_SliderCanvasGroup.alpha = 1f;

            m_ButtonTweenId = LeanTween.value(m_PlayCanvasGroup.alpha, 0, 1f)
                .setOnUpdate((float t) =>
                {
                    m_SliderCanvasGroup.alpha = t;
                    m_PlayCanvasGroup.alpha = t;

                }).uniqueId;
        }
        else if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
        {
            m_PlayCanvasGroup.alpha = 0f;
            m_PauseCanvasGroup.alpha = 1;
            m_StopCanvasGroup.alpha = 0f;
            m_SliderCanvasGroup.alpha = 1;
        }
        else if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.END || state == MediaPlayerCtrl.MEDIAPLAYER_STATE.STOPPED)
        {
            m_PlayCanvasGroup.alpha = 0f;
            m_PauseCanvasGroup.alpha = 0;
            m_StopCanvasGroup.alpha = 1f;
            m_SliderCanvasGroup.alpha = 1;
        }
        else if (state == MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
        {
            m_PlayCanvasGroup.alpha = 0f;
            m_PauseCanvasGroup.alpha = 0;
            m_StopCanvasGroup.alpha = 0f;
            m_SliderCanvasGroup.alpha = 0;
        }
    }

    private void Update()
    {
        if (m_VideoPlayer.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING)
            m_Slider.value = m_VideoPlayer.GetSeekBarValue();
    }

#if UNITY_EDITOR

    [SerializeField] private MediaPlayerCtrl.MEDIAPLAYER_STATE m_DebugPlayerState;
    private void LateUpdate()
    {
        m_DebugPlayerState = m_VideoPlayer.GetCurrentState();
    }
#endif
}
