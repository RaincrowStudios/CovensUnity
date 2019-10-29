using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamPlayerView : MonoBehaviour
{
    public static TeamPlayerView Instance { get; set; }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI _displayName;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _degree;
    [SerializeField] private TextMeshProUGUI _coven;
    [SerializeField] private TextMeshProUGUI _dominion;
    [SerializeField] private TextMeshProUGUI _dominionRank;
    [SerializeField] private TextMeshProUGUI _worldRank;
    [SerializeField] private TextMeshProUGUI _energy;
    [SerializeField] private TextMeshProUGUI _power;
    [SerializeField] private TextMeshProUGUI _resilience;

    public Image schoolSigil;
    public ApparelView male;
    public ApparelView female;
    public Button flyToPlayerBtn;
    public Button btnBack;
    public Button inviteToCoven;

    public Sprite whiteSchool;
    public Sprite shadowSchool;
    public Sprite greySchool;

    private int m_TweenId;
    private Vector2 playerPos = Vector2.zero;
    private System.Action m_OnFly;
    private System.Action m_OnCoven;
    private System.Action m_OnClose;

    void Awake()
    {
        Instance = this;
        btnBack.onClick.AddListener(OnClickClose);
        flyToPlayerBtn.onClick.AddListener(FlyToPlayer);


        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
    }

    public void Show(WitchMarkerData data, System.Action onFly = null, System.Action onCoven = null, System.Action onClose = null)
    {
        inviteToCoven.onClick.RemoveAllListeners();
        inviteToCoven.onClick.AddListener(() =>
        {
            InviteToCoven(data.name);
        });
        if (string.IsNullOrEmpty(data.coven))
        {
            inviteToCoven.gameObject.SetActive(true);
        }
        else
        {
            inviteToCoven.gameObject.SetActive(false);
        }

        flyToPlayerBtn.interactable = true;
        flyToPlayerBtn.gameObject.SetActive(string.IsNullOrEmpty(data.covenId) == false && data.covenId == PlayerDataManager.playerData.covenId);

        playerPos.x = data.longitude;
        playerPos.y = data.latitude;

        if (data.male)
        {
            female.gameObject.SetActive(false);
            male.gameObject.SetActive(true);
            male.InitializeChar(data.equipped);
        }
        else
        {
            female.gameObject.SetActive(true);
            male.gameObject.SetActive(false);
            female.InitializeChar(data.equipped);
        }

        ChangeDegree(data.degree);
        _displayName.text = data.name;
        _level.text = LocalizeLookUp.GetText("lt_level") + " " + data.level.ToString();
        _dominion.text = LocalizeLookUp.GetText("lt_dominion") + " " + data.dominion;
        _dominionRank.text = LocalizeLookUp.GetText("lt_dominion_rank") + " " + data.dominionRank;
        _worldRank.text = LocalizeLookUp.GetText("lt_world_rank") + " " + data.worldRank;
        _coven.text = (string.IsNullOrEmpty(data.coven) ? LocalizeLookUp.GetText("lt_coven_none") : LocalizeLookUp.GetText("lt_coven") + " " + data.coven);
        _power.text = LocalizeLookUp.GetText("generic_power") + ": " + data.GetPower(null);
        _resilience.text = LocalizeLookUp.GetText("generic_resilience") + ": " + data.GetResilience(null);
        _energy.text = LocalizeLookUp.GetText("lt_energy") + " " + data.energy.ToString() + "/" + data.baseEnergy.ToString();

        m_OnFly = onFly;
        m_OnCoven = onCoven;
        m_OnClose = onClose;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 1, 0.75f)
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                m_Canvas.enabled = true;
                m_InputRaycaster.enabled = true;
            })
            .setOnUpdate((float v) =>
            {
                m_CanvasGroup.alpha = v;
                m_CanvasGroup.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, v);
            })
            .uniqueId;

        BackButtonListener.AddCloseAction(Close);
    }

    public void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_OnFly = null;
        m_OnCoven = null;
        m_OnClose = null;

        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 0.5f)
            .setEaseOutCubic()
            .setOnUpdate((float v) =>
            {
                m_CanvasGroup.alpha = v;
                m_CanvasGroup.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, v);
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .uniqueId;
    }

    private void ChangeDegree(int Degree)
    {
        _degree.text = Utilities.WitchTypeControlSmallCaps(Degree);
        if (Degree < 0)
        {
            schoolSigil.sprite = shadowSchool;
            schoolSigil.color = Utilities.Purple;
        }
        else if (Degree > 0)
        {
            schoolSigil.sprite = whiteSchool;
            schoolSigil.color = Utilities.Orange;
        }
        else
        {
            schoolSigil.sprite = greySchool;
            schoolSigil.color = Utilities.Blue;
        }
    }

    public void FlyToPlayer()
    {
        if (PlayerDataManager.playerData.energy == 0)
            return;

        m_OnFly?.Invoke();

        PlayerManager.Instance.FlyTo(playerPos.x, playerPos.y);
        Close();
    }

    private void OnClickClose()
    {
        m_OnClose?.Invoke();
        Close();
    }


    public static void ViewCharacter(string id, System.Action<WitchMarkerData, string> callback, bool searchByName = false, System.Action onFlyTo = null)
    {
        APIManager.Instance.Get(
            "character/select/" + id + "?selection=chat&name=" + searchByName.ToString().ToLower(),
            "",
            (response, result) =>
            {
                if (result == 200)
                {
                    WitchMarkerData data = JsonConvert.DeserializeObject<WitchMarkerData>(response);
                    Instance.Show(data, onFlyTo);
                    callback?.Invoke(data, null);
                }
                else
                {
                    callback?.Invoke(null, APIManager.ParseError(response));
                }
            });
    }
    public void InviteToCoven(string id)
    {
        if (string.IsNullOrEmpty(TeamManager.MyCovenId) == false)
        {
            LoadingOverlay.Show();
            TeamManager.SendInvite(id, true, (invite, error) =>
            {
                LoadingOverlay.Hide();
                if (string.IsNullOrEmpty(error))
                {
                    UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("coven_invite_success"));
                }
                else
                {
                    UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                }
            });
        }
    }
}