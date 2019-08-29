using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Raincrow;

public class UIGlobalPopup : MonoBehaviour
{
    private static UIGlobalPopup m_Instance;

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;

    [SerializeField] private TextMeshProUGUI m_SizeFitter;
    [SerializeField] private TextMeshProUGUI m_MessageText;
    [SerializeField] private TextMeshProUGUI m_ConfirmText;
    [SerializeField] private TextMeshProUGUI m_CancelText;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    public static bool IsOpen { get; private set; }
    public static bool IsLoading { get; private set; }
    private static List<System.Action> m_Queue = new List<Action>();

    private System.Action m_OnConfirm;
    private System.Action m_OnCancel;
    
    private int m_AlphaTweenId;
    private int m_ScaleTweenId;
    
    private void Awake()
    {
        m_Instance = this;

        m_CanvasGroup.alpha = 0;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_ConfirmButton.onClick.AddListener(OnClickConfirm);
        m_CancelButton.onClick.AddListener(OnClickCancel);

        if (m_Queue.Count > 0)
        {
            m_Queue[0]?.Invoke();
            m_Queue.RemoveAt(0);
        }
    }

    public static void LoadScene()
    {
        if (m_Instance != null)
            return;

        if (IsLoading)
            return;

        LoadingOverlay.Show();
        IsLoading = true;
        SceneManager.LoadSceneAsync(SceneManager.Scene.POPUP, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
        {
            IsLoading = false;
            LoadingOverlay.Hide();
        });
    }

    public static void ShowPopUp(Action confirmAction, Action cancelAction, string txt)
    {
        System.Action show = () =>
        {
            m_Instance.SetButtons("Yes", "No", confirmAction, cancelAction);
            m_Instance.SetMessage(txt, false);
            m_Instance.Show();
        };
        
        if (m_Instance == null)
        {
            m_Queue.Add(show);
            LoadScene();
            return;
        }

        if (IsOpen)
            m_Queue.Add(show);
        else
            show();
    }

    public static void ShowPopUp(Action confirmAction, string txt)
    {
        System.Action show = () =>
        {
            m_Instance.SetButtons("Ok", confirmAction);
            m_Instance.SetMessage(txt, false);
            m_Instance.Show();
        };

        if (m_Instance == null)
        {
            m_Queue.Add(show);
            LoadScene();
            return;
        }

        if (IsOpen)
            m_Queue.Add(show);
        else
            show();
    }
    
    public static void ShowError(Action confirmAction, string txt)
    {
        System.Action show = () =>
        {
            m_Instance.SetButtons("Ok", confirmAction);
            m_Instance.SetMessage(txt, true);
            m_Instance.Show();
        };

        if (m_Instance == null)
        {
            m_Queue.Add(show);
            LoadScene();
            return;
        }

        if (IsOpen)
            m_Queue.Add(show);
        else
            show();
    }

    private void Show()
    {
        IsOpen = true;

        LeanTween.cancel(m_AlphaTweenId);
        LeanTween.cancel(m_ScaleTweenId);
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_CanvasGroup.alpha = 0;
        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.5f)
            .setEaseOutCubic()
            .uniqueId;

        m_Panel.localScale = Vector3.one * 0.8f;
        m_ScaleTweenId = LeanTween.scale(m_Panel, Vector3.one, 0.25f)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void Hide()
    {
        if (m_Queue.Count > 0)
        {
            m_Queue[0]?.Invoke();
            m_Queue.RemoveAt(0);
            return;
        }

        IsOpen = false;

        LeanTween.cancel(m_AlphaTweenId);
        LeanTween.cancel(m_ScaleTweenId);
        m_InputRaycaster.enabled = false;

        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 0.25f)
            .setOnComplete(() => m_Canvas.enabled = false)
            .setEaseOutCubic()
            .uniqueId;
        m_ScaleTweenId = LeanTween.scale(m_Panel, Vector3.one * 0.8f, 0.5f)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void SetMessage(string message, bool error = false)
    {
        m_MessageText.text = message;
        m_SizeFitter.text = message;
        m_MessageText.color = error ? Color.red : Color.white;
    }

    private void SetButtons(string confirm, System.Action onConfirm)
    {
        m_ConfirmText.text = confirm;

        m_OnConfirm = onConfirm;
        m_OnCancel = null;

        m_ConfirmButton.gameObject.SetActive(true);
        m_CancelButton.gameObject.SetActive(false);
    }

    private void SetButtons(string confirm, string cancel, System.Action onConfirm, System.Action onCancel)
    {
        m_ConfirmText.text = confirm;
        m_CancelText.text = cancel;

        m_OnConfirm = onConfirm;
        m_OnCancel = onCancel;

        m_ConfirmButton.gameObject.SetActive(true);
        m_CancelButton.gameObject.SetActive(true);
    }

    private void OnClickConfirm()
    {
        m_OnConfirm?.Invoke();
        m_OnConfirm = null;
        Hide();
    }

    private void OnClickCancel()
    {
        m_OnCancel?.Invoke();
        m_OnCancel = null;
        Hide();
    }
    
#if UNITY_EDITOR
    [SerializeField] private string m_DebugString;

    [ContextMenu("Debug ShowMessage")]
    private void DebugShowMessage()
    {
        ShowPopUp(null, m_DebugString);
    }

    [ContextMenu("Debug ShowError")]
    private void DebugShowError()
    {
        ShowError(null, m_DebugString);
    }
#endif
}