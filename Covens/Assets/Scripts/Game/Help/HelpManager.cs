using System.Collections;
using System.Collections.Generic;
using Raincrow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    private static HelpManager m_Instance;
    public static bool IsOpen { get; private set; }

    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_MainRect;
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private Button m_EmailBtn;
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private TextMeshProUGUI m_msg;
    [SerializeField] private TextMeshProUGUI m_CloseTxt;
    [SerializeField] private TextMeshProUGUI m_EmailTxt;


    // Start is called before the first frame update

    void Awake()
    {
        m_Instance = this;
        m_CloseBtn.onClick.AddListener(Close);
        m_EmailBtn.onClick.AddListener(SendEmail);

        if (Application.systemLanguage == SystemLanguage.English)
        {
            m_msg.text = "Dear witches, if you ever need any help just go to settings and tap the symbol above.";
            m_CloseTxt.text = "Return to Covens";
            m_EmailTxt.text = "Contact us now";
        }
        else if (Application.systemLanguage == SystemLanguage.Japanese)
        {
            m_msg.text = "親愛なる魔女たち。助けが必要な場合は、設定に移動して上の記号をタップしてください。";
            m_CloseTxt.text = "Covensに戻る";
            m_EmailTxt.text = "今すぐお問い合わせください";
        }
        else if (Application.systemLanguage == SystemLanguage.Spanish)
        {
            m_msg.text = "Queridas brujas, si alguna vez necesitas ayuda, solo ve a la configuración y toca el símbolo de arriba.";
            m_CloseTxt.text = "Regreso a Covens";
            m_EmailTxt.text = "Contáctanos ahora";
        }
        else if (Application.systemLanguage == SystemLanguage.Portuguese)
        {
            m_msg.text = "Queridas bruxas, se você precisar de ajuda, basta acessar as configurações e tocar no símbolo acima.";
            m_CloseTxt.text = "Retornar para Covens";
            m_EmailTxt.text = "Entre em contato conosco agora";
        }
        else if (Application.systemLanguage == SystemLanguage.German)
        {
            m_msg.text = "Liebe Hexen, wenn Sie jemals Hilfe benötigen, gehen Sie einfach zu den Einstellungen und tippen Sie auf das Symbol oben.";
            m_CloseTxt.text = "Kehre zu Covens zurück";
            m_EmailTxt.text = "Kontaktiere uns jetzt";
        }
        else
        {
            m_msg.text = "Дорогие ведьмы, если вам когда-нибудь понадобится помощь, просто зайдите в настройки и нажмите на символ выше.";
            m_CloseTxt.text = "Возвращение в Ковены";
            m_EmailTxt.text = "Свяжитесь с нами сейчас";
        }
    }

    public static void Open()
    {
        if (m_Instance != null)
        {
            m_Instance._Open();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.HELP, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            {
                LoadingOverlay.Hide();
                m_Instance._Open();
            });
        }
        IsOpen = true;
    }


    public static void Close()
    {
        IsOpen = false;
        if (m_Instance == null)
            return;
        m_Instance._Close();
    }

    private void _Open()
    {

        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;
        LeanTween.value(m_CanvasGroup.alpha, 1f, 0.6f)
           .setEaseOutCubic()
           .setOnUpdate((float t) =>
           {
               m_CanvasGroup.alpha = t;
               m_MainRect.localScale = new Vector3(t, t, t);
           });
    }

    private void _Close()
    {

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_MainRect.localScale = Vector3.one * .6f;
    }

    private void SendEmail()
    {
        string email = "help@raincrowgames.com";
        string subject = MyEscapeURL("Covens Help");
        string body = MyEscapeURL($"Version: {Application.version} \n Platform: {Application.platform} \n\n\n ***Your Message*** +\n\n\n ***Screenshot***\n\n\n");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

    }
    private string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
}
