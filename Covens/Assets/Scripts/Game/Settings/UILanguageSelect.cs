using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILanguageSelect : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_ButtonContainer;
    [SerializeField] private Button m_ButtonPrefab;
    [SerializeField] private Button m_CloseButton;

    public Color buttonSelected;
    public Color buttonNotSelected;

    private int m_TweenId;
    private Button[] m_Buttons;

    private void Awake()
    {
        //{ "English", "Portuguese", "Spanish", "Japanese", "German", "Russian" };
        string[] languages = DictionaryManager.Languages;
        m_Buttons = new Button[languages.Length];

        for(int i = 0; i < languages.Length; i++)
        {
            int aux = i;
            Button butt = Instantiate(m_ButtonPrefab, m_ButtonContainer);
            butt.GetComponentInChildren<TextMeshProUGUI>().text = LocalizeLookUp.GetText("language_" + languages[aux].ToLower());
            butt.onClick.AddListener(() => OnClickLanguage(aux));
            butt.gameObject.SetActive(true);
            m_Buttons[i] = butt;
        }

        m_ButtonPrefab.gameObject.SetActive(false);

        ToggleButtons(DictionaryManager.languageIndex);

        m_CloseButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        BackButtonListener.AddCloseAction(Hide);
        
        m_CanvasGroup.blocksRaycasts = true;
        m_CanvasGroup.interactable = true;
        gameObject.SetActive(true);

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .uniqueId;
    }

    public void Hide()
    {
        BackButtonListener.RemoveCloseAction();

        m_CanvasGroup.blocksRaycasts = false;
        m_CanvasGroup.interactable = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
            })
            .uniqueId;
    }

    public void OnClickLanguage(int idx)
    {
        SoundManagerOneShot.Instance.PlayButtonTap();

        int previousLanguage = DictionaryManager.languageIndex;
        DictionaryManager.languageIndex = idx;

        LoadingOverlay.Show();

        DictionaryManager.GetLocalisationDictionary(
            DownloadManager.AssetVersion.localization,
            onDicionaryReady: () =>
            {
                ToggleButtons(idx);
                LoadingOverlay.Hide();
                LocalizationManager.OnChangeLanguage?.Invoke();
            },
            onDownloadError: (status, response) =>
            {
                LoadingOverlay.Hide();
                UIGlobalPopup.ShowError(null, LocalizeLookUp.GetText("account_creation_error").Replace("{{Error}}", status.ToString()));
            },
            onParseError: () =>
            {
                LoadingOverlay.Hide();
                UIGlobalPopup.ShowError(null, LocalizeLookUp.GetText("account_creation_error").Replace("{{Error}}", LocalizeLookUp.GetText("lt_failed")));
            });
    }

    private void ToggleButtons(int idx)
    {
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            m_Buttons[i].targetGraphic.color = idx == i ? buttonSelected : buttonNotSelected;
        }
    }
}
