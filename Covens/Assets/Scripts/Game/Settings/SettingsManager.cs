using Raincrow;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.Unity.Example;
using Newtonsoft.Json;
using TMPro;
using Raincrow.FTF;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycast;

    int currWitchButton;
    int currCollButton;
    int currSpiritButton;
    private GameObject creditsClone;
    private int m_TweenId;

    [SerializeField] private TextMeshProUGUI RID;
    [SerializeField] private TextMeshProUGUI m_AppVersion;
    [SerializeField] private Button m_HelpCrowButton;
    [SerializeField] private Button m_WitchSchoolButton;

    public Button[] buildingsOnOff = new Button[2];
    public Button[] soundOnOff = new Button[2];

    public GameObject Credits;
    public Button[] Languages;

    public Button tOS;
    public Button privacyPolicy;


    public GameObject LanguageSelect;
    public CanvasGroup LanguageCG;
    public Button Language;

    public Button FirstTapBtn;

    public Color buttonSelected;
    public Color buttonNotSelected;

    public Vector3 vectButtonSelected;
    public Vector3 vectButtonNotSel;

    public CanvasGroup CG;
    public GameObject container;

    private static SettingsManager m_Instance;

    public static bool AudioEnabled
    {
        get
        {
            return PlayerPrefs.GetInt("Settings.AudioEnabled", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Settings.AudioEnabled", value ? 1 : 0);
            AudioListener.pause = !value;
            OnAudioToggle?.Invoke(value);
        }
    }

    public static bool BuildingsEnabled
    {
        get
        {
            return PlayerPrefs.GetInt("Settings.BuildingsEnabled", (Application.isEditor || SystemInfo.systemMemorySize > 3000) ? 1 : 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Settings.BuildingsEnabled", value ? 1 : 0);
            OnBuildingsToggle?.Invoke(value);
        }
    }

    public static event System.Action<bool> OnAudioToggle;
    public static event System.Action<bool> OnBuildingsToggle;

    public static void LoadSettings()
    {
        AudioEnabled = SettingsManager.AudioEnabled;
        BuildingsEnabled = SettingsManager.BuildingsEnabled;
    }

    public static void OpenUI()
    {
        if (m_Instance != null)
        {
            m_Instance.Show();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.SETTINGS, UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (t) => { },
                () =>
                {
                    LoadingOverlay.Hide();
                    m_Instance.Show();
                });
        }
    }

    public static void CloseUI()
    {
        if (m_Instance == null)
            return;
        m_Instance.Hide();
    }

    void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycast.enabled = false;
        CG.alpha = 0;
        FirstTapBtn.GetComponentInChildren<Image>().color = buttonSelected;
        FirstTapBtn.onClick.AddListener(ToggleFirstTap);
        LanguageCG.alpha = 0f;
        LeanTween.scale(LanguageSelect, Vector3.zero, 0.1f);
        LanguageSelect.SetActive(false);
        vectButtonNotSel.Set(1f, 1f, 1f);
        vectButtonSelected.Set(1.1f, 1.1f, 1.1f);
        RID.text = LocalizeLookUp.GetText("raincrow_id") + " : " + LoginAPIManager.StoredUserName;

        //setting up listeners for buttons
        for (int i = 0; i < Languages.Length; i++)
        {
            object k = i;
            Languages[i].onClick.AddListener(() =>
            {
                ToggleLanguage(k);
            });
        }

        Language.onClick.AddListener(showLanguages);

        tOS.onClick.AddListener(ShowTOS);
        privacyPolicy.onClick.AddListener(ShowPrivacyPolicy);

        soundOnOff[0].onClick.AddListener(() =>
        {
            if (!AudioEnabled)
                AudioEnabled = true;
        });
        soundOnOff[1].onClick.AddListener(() =>
        {
            if (AudioEnabled)
                AudioEnabled = false;
        });

        buildingsOnOff[0].onClick.AddListener(() =>
        {
            if (!BuildingsEnabled)
                BuildingsEnabled = true;
        }
        );
        buildingsOnOff[1].onClick.AddListener(() =>
        {
            if (BuildingsEnabled)
                BuildingsEnabled = false;
        });

        m_HelpCrowButton.onClick.AddListener(() =>
        {
            Raincrow.Chat.UI.UIChat.Open(Raincrow.Chat.ChatCategory.SUPPORT);
            this.Hide();
        });

        m_WitchSchoolButton.onClick.AddListener(() =>
        {
            WitchSchoolManager.Open();
            this.Hide();
        });

        ToggleSound(AudioEnabled);
        EnableDisableBuildings(BuildingsEnabled);
    }

    public void ToggleLanguage(object obj)
    {
        var ClickedButton = (int)obj;

        for (int i = 0; i < Languages.Length; i++)
        {
            var p = Languages[i].transform.GetChild(1).GetComponent<Image>();
            if (i == ClickedButton)
            {
                SoundManagerOneShot.Instance.PlayButtonTap();
                p.color = buttonSelected;
            }
            else
            {
                p.color = buttonNotSelected;
            }
        }
    }
    public void ToggleFirstTap()
    {
        FirstTapManager.ResetFirsts();
        FirstTapBtn.GetComponentInChildren<Image>().color = buttonNotSelected;
    }
    public void ToggleSound(bool soundOn)
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        if (soundOn)
        {
            soundOnOff[0].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(soundOnOff[0].gameObject, vectButtonSelected, 0.3f);
            soundOnOff[1].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(soundOnOff[1].gameObject, vectButtonNotSel, 0.3f);
        }
        else
        {
            soundOnOff[1].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(soundOnOff[1].gameObject, vectButtonSelected, 0.3f);
            soundOnOff[0].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(soundOnOff[0].gameObject, vectButtonNotSel, 0.3f);
        }
    }

    public void EnableDisableBuildings(bool enableBuildings)
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        if (enableBuildings)
        {
            buildingsOnOff[0].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(buildingsOnOff[0].gameObject, vectButtonSelected, 0.3f);
            buildingsOnOff[1].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(buildingsOnOff[1].gameObject, vectButtonNotSel, 0.3f);
        }
        else
        {
            buildingsOnOff[1].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(buildingsOnOff[1].gameObject, vectButtonSelected, 0.3f);
            buildingsOnOff[0].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(buildingsOnOff[0].gameObject, vectButtonNotSel, 0.3f);
        }
    }

    public void ShowCredits()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        creditsClone = Utilities.InstantiateObject(Credits, transform.GetChild(0));
        var rect = creditsClone.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector3.zero;
    }

    public void DestroyCredits()
    {
        if (this.transform.GetChild(0).GetChild(6) != null)
        {
            Destroy(creditsClone);
        }
    }

    public void ShowTOS()
    {
        Application.OpenURL(CovenConstants.TERMS_OF_SERVICE_URL);
    }

    public void ShowPrivacyPolicy()
    {
        Application.OpenURL(CovenConstants.PRIVACY_POLICY_URL);
    }

    public void ChangeSoundLevel(float value)
    {
        AudioListener.volume = value;
    }

    public void Show()
    {
        OnAudioToggle += _OnToggleAudio;
        OnBuildingsToggle += _OnToggleBuilding;

        m_Canvas.enabled = true;
        m_InputRaycast.enabled = true;

        float start = CG.alpha;
        float end = 1;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(start, end, 0.35f)
            .setOnUpdate((float t) =>
            {
                float t2 = LeanTween.easeOutCirc(start, end, t);

                CG.alpha = t;
                container.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t2);
            })
            .setOnComplete(() =>
            {
                UIStateManager.Instance.CallWindowChanged(false);
                MapsAPI.Instance.HideMap(true);
            })
            .uniqueId;

        m_AppVersion.text = string.Concat(LocalizeLookUp.GetText("settings_version") + " ", DownloadedAssets.AppVersion);
    }

    public void Hide()
    {
        OnAudioToggle -= _OnToggleAudio;
        OnBuildingsToggle -= _OnToggleBuilding;

        UIStateManager.Instance.CallWindowChanged(true);
        MapsAPI.Instance.HideMap(false);

        m_InputRaycast.enabled = false;
        m_AppVersion.text = string.Empty;
        float start = container.transform.localScale.x;
        float end = 0;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.45f)
            .setOnUpdate((float t) =>
            {
                float t1 = (1 - t) * (0.35f / 0.45f);
                float t2 = LeanTween.easeOutCirc(start, end, t);

                CG.alpha = t1;
                container.transform.localScale = new Vector3(t2, t2, t2);
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                m_TweenId = LeanTween.value(0, 0, 5f).setOnComplete(() =>
                {
                    SceneManager.UnloadScene(SceneManager.Scene.SETTINGS, null, null);
                }).uniqueId;

            })
            .uniqueId;
    }

    public void showLanguages()
    {
        LanguageCG.interactable = true;
        LanguageSelect.SetActive(true);
        LeanTween.scale(LanguageSelect, Vector3.one, 0.5f);
        LeanTween.alphaCanvas(LanguageCG, 1f, 0.3f);
    }

    public void hideLanguages()
    {
        LanguageCG.interactable = false;
        LeanTween.scale(LanguageSelect, Vector3.zero, 0.5f).setOnComplete(() =>
        {
            LanguageSelect.SetActive(false);
        });
        LeanTween.alphaCanvas(LanguageCG, 0f, 0.3f);
    }

    private void _OnToggleAudio(bool enabled)
    {
        ToggleSound(enabled);
    }

    private void _OnToggleBuilding(bool enabled)
    {
        EnableDisableBuildings(enabled);
    }
}