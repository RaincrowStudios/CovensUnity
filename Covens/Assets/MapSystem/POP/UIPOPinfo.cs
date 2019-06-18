using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.Maps;
using System.Collections;

public class UIPOPinfo : MonoBehaviour
{
    private static UIPOPinfo m_Instance;
    public static UIPOPinfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIPOPinfo>("UIPOPinfo"));
            return m_Instance;
        }
    }

    public static bool isOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.m_Canvas.enabled;
        }
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_Loading;
    [SerializeField] private CanvasGroup m_LoadingBlock;


    [Header("PoP Info - Unlaimed")]
    [SerializeField] private CanvasGroup m_UnclaimedGroup;
    [SerializeField] private TextMeshProUGUI m_UnclaimedTitle;
    [SerializeField] private TextMeshProUGUI m_UnclaimedDefendedBy;
    [SerializeField] private Image m_UnclaimedSpiritArt;
    [SerializeField] private Button m_UnclaimedEnterBtn;
    [SerializeField] private Button m_UnclaimedOfferingBtn;
    [SerializeField] private Button m_UnclaimedCloseBtn;
    
    [Header("PoP Info - Claimed")]
    [SerializeField] private CanvasGroup m_ClaimedGroup;
    [SerializeField] private TextMeshProUGUI m_ClaimedTitle;
    [SerializeField] private TextMeshProUGUI m_ClaimedDefendedBy;
    [SerializeField] private TextMeshProUGUI m_ClaimedOwner;
    [SerializeField] private TextMeshProUGUI m_ClaimedRewardOn;
    [SerializeField] private TextMeshProUGUI m_ClaimedCooldown;
    [SerializeField] private Button m_ClaimedEnterBtn;
    [SerializeField] private Button m_ClaimedCloseBtn;

    [Header("Offering Popup")]
    [SerializeField] private CanvasGroup m_OfferingCanvasGroup;
    [SerializeField] private TextMeshProUGUI m_OfferingTitle;
    [SerializeField] private TextMeshProUGUI m_OfferingHerb;
    [SerializeField] private TextMeshProUGUI m_OfferingGem;
    [SerializeField] private TextMeshProUGUI m_OfferingTool;
    [SerializeField] private Button m_OfferingConfirmBtn;
    [SerializeField] private Button m_OfferingCancelBtn;

    public IMarker marker { get; private set; }
    public Token tokenData { get; private set; }
    public LocationMarkerDetail details { get; private set; }

    private int m_TweenId;
    private int m_LoadingBlockTweenId;
    private int m_OfferingTweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_ClaimedGroup.alpha = 0;
        m_UnclaimedGroup.alpha = 0;
        m_OfferingCanvasGroup.alpha = 0;
        m_Loading.alpha = 0;
        m_LoadingBlock.alpha = 0;
        m_Loading.gameObject.SetActive(false);
        m_ClaimedGroup.gameObject.SetActive(false);
        m_UnclaimedGroup.gameObject.SetActive(false);
        m_OfferingCanvasGroup.gameObject.SetActive(false);

        m_ClaimedEnterBtn.onClick.AddListener(OnClickEnter);
        m_UnclaimedEnterBtn.onClick.AddListener(OnClickEnter);

        m_ClaimedCloseBtn.onClick.AddListener(OnClickClose);
        m_UnclaimedCloseBtn.onClick.AddListener(OnClickClose);

        m_UnclaimedOfferingBtn.onClick.AddListener(OnClickOffering);
        m_OfferingConfirmBtn.onClick.AddListener(OnOfferingConfirm);
        m_OfferingCancelBtn.onClick.AddListener(OnOfferingCancel);

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        if (isOpen)
            return;

        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        Destroy(this.gameObject);
    }

    public void Show(IMarker marker, Token data)
    {
        this.tokenData = data;
        this.marker = marker;

        bool isUnclaimed = string.IsNullOrEmpty(data.owner);

        if (isUnclaimed)
        {
            m_UnclaimedTitle.text = LocalizeLookUp.GetText("pop_title");
            m_UnclaimedDefendedBy.text = "";
            m_UnclaimedCooldown.text = "";
            m_UnclaimedSpiritArt.color = new Color(0, 0, 0, 0);

            m_UnclaimedEnterBtn.interactable = false;
            m_ClaimedGroup.gameObject.SetActive(false);
            m_UnclaimedGroup.gameObject.SetActive(true);
        }
        else
        {
            m_ClaimedTitle.text = LocalizeLookUp.GetText("pop_title");
            m_ClaimedDefendedBy.text = "";
            m_ClaimedOwner.text = "";
            m_ClaimedRewardOn.text = "";
            m_ClaimedCooldown.text = "";
            m_ClaimedEnterBtn.interactable = false;

            m_ClaimedGroup.gameObject.SetActive(true);
            m_UnclaimedGroup.gameObject.SetActive(false);
        }

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_Loading.gameObject.SetActive(true);
        m_Loading.alpha = 1;
        
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(isUnclaimed ? m_UnclaimedGroup : m_ClaimedGroup, 1f, 0.3f).setEase(LeanTweenType.easeInCubic).uniqueId;
        LeanTween.value(0f,1f,0.3f).setOnComplete(() => {
        SoundManagerOneShot.Instance.PlayPostEffect2(0.5f);
        LeanTween.scale(m_UnclaimedSpiritArt.gameObject, new Vector3(1f, 1f, 1f), 0.3f).setEase(LeanTweenType.easeInCubic);
        });
       
    }

    /*
     * 
     */
    public void Setup(LocationMarkerDetail data)
    {
        LeanTween.scale(m_UnclaimedSpiritArt.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.01f);
        details = data;
        SpiritDict spirit = string.IsNullOrEmpty(data.spiritId) ? null : DownloadedAssets.GetSpirit(data.spiritId);

        bool isUnclaimed = string.IsNullOrEmpty(data.controlledBy);

        //reload the UI
        if (data.controlledBy != marker.token.owner)
            Show(marker, marker.token);

        System.TimeSpan cooldownTimer = Utilities.TimespanFromJavaTime(data.takenOn);
        float secondsRemaining = (60 * 60) - Mathf.Abs((float)cooldownTimer.TotalSeconds);
        bool isCooldown = secondsRemaining > 0;

        if (isUnclaimed)
        {
            if (!string.IsNullOrEmpty(data.displayName))
                m_UnclaimedTitle.text = data.displayName;

            m_OfferingTitle.text = LocalizeLookUp.GetText("pop_offering_title").Replace("{{Spirit Name}}", spirit.spiritName);
            m_UnclaimedDefendedBy.text = (spirit == null ? "" : LocalizeLookUp.GetText("pop_defended").Replace("{{spirit}}", spirit.spiritName).Replace("{{tier}}", spirit.spiritTier.ToString()));

            if (isCooldown)
                StartCoroutine(CooldownCoroutine(secondsRemaining, m_UnclaimedEnterBtn.GetComponent<TextMeshProUGUI>()));
            else
                m_UnclaimedEnterBtn.GetComponent<TextMeshProUGUI>().text = LocalizeLookUp.GetText("pop_challenge");

            if (spirit != null)
                DownloadedAssets.GetSprite(data.spiritId, (spr) =>
                {
                    m_UnclaimedSpiritArt.overrideSprite = spr;
                    LeanTween.color(m_UnclaimedSpiritArt.rectTransform, Color.white, 1f).setEaseOutCubic();
                });

            m_UnclaimedEnterBtn.interactable = !isCooldown;
        }
        else
        {
            if (!string.IsNullOrEmpty(data.displayName))
                m_ClaimedTitle.text = data.displayName;

            m_ClaimedDefendedBy.text = (spirit == null ? "" : LocalizeLookUp.GetText("pop_defended").Replace("{{spirit}}", spirit.spiritName).Replace("{{tier}}", spirit.spiritTier.ToString()));

            if (isCooldown)
                StartCoroutine(CooldownCoroutine(secondsRemaining, m_ClaimedCooldown));

            if (data.isCoven)
                m_ClaimedOwner.text = LocalizeLookUp.GetText("pop_owner_coven").Replace("{{coven}}", data.controlledBy);
            else
                m_ClaimedOwner.text = LocalizeLookUp.GetText("pop_owner_player").Replace("{{player}}", data.controlledBy);

            if (data.rewardOn != 0)
                m_ClaimedRewardOn.text = LocalizeLookUp.GetText("pop_treasure_time").Replace("{{time}}", GetTime(data.rewardOn));
            else
                m_ClaimedRewardOn.text = "";

            m_ClaimedEnterBtn.interactable = !isCooldown;
        }
              
        LeanTween.alphaCanvas(m_Loading, 0f, 1f).setEaseOutCubic().setOnComplete(() => m_Loading.gameObject.SetActive(false));
    }
    
    private void Close(float time = 0.5f, System.Action onComplete = null)
    {
        
        m_InputRaycaster.enabled = false;
        StopAllCoroutines();
        LeanTween.cancel(m_TweenId);

        m_TweenId = LeanTween.alphaCanvas(string.IsNullOrEmpty(marker.token.owner) ? m_UnclaimedGroup : m_ClaimedGroup, 0f, time)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                m_ClaimedGroup.gameObject.SetActive(false);
                m_UnclaimedGroup.gameObject.SetActive(false);
                m_UnclaimedSpiritArt.overrideSprite = null;
                onComplete?.Invoke();
            }).uniqueId;

        HideLoadingBlock();
        //SoundManagerOneShot.Instance.SetBGTrack(PlayerDataManager.soundTrack);
    }


    private void ShowOfferingScreen()
    {
        LeanTween.cancel(m_OfferingTweenId);
        SoundManagerOneShot.Instance.PlayMakeYourOffering();

        int ownedHerbs = 0;
        int ownedGems = 0;
        int ownedTools = 0;
        bool hasRequiredIngredients = true;

        //herb
        if (string.IsNullOrEmpty(details.herb) == false)
        {
            IngredientDict herb = DownloadedAssets.GetIngredient(details.herb);
            ownedHerbs = PlayerDataManager.playerData.ingredients.Amount(details.herb);
            hasRequiredIngredients &= ownedHerbs > 0;

            m_OfferingHerb.text = string.Concat(LocalizeLookUp.GetText("pop_required_ingredients").Replace("{{ingredient}}", string.Concat("1 ", herb.name)));
            if (ownedHerbs != 0) 
                {
                m_OfferingHerb.text = string.Concat(m_OfferingHerb.text, " (1/1)"); 
                }
            else 
                {
                m_OfferingHerb.text = string.Concat(m_OfferingHerb.text, " (0/1)");//the 1 needs to change to required amount from backend depending on tier
                }
            m_OfferingHerb.color = ownedHerbs <= 0 ? Color.red : Color.white;
            m_OfferingHerb.gameObject.SetActive(true);
            
        }
        else
        {
            m_OfferingHerb.gameObject.SetActive(false);
        }

        //gem
        if (string.IsNullOrEmpty(details.gem) == false)
        {
            IngredientDict gem = DownloadedAssets.GetIngredient(details.gem);
            ownedGems = PlayerDataManager.playerData.ingredients.Amount(details.gem);
            hasRequiredIngredients &= ownedGems > 0;

            m_OfferingGem.text = string.Concat(LocalizeLookUp.GetText("pop_required_ingredients").Replace("{{ingredient}}", string.Concat("1 ", gem.name)));
            if (ownedGems != 0) 
                {
                m_OfferingGem.text = string.Concat(m_OfferingGem.text, " (1/1)");
                }
            else
                {
                m_OfferingGem.text = string.Concat(m_OfferingGem.text, " (0/1)");
                }
            m_OfferingGem.color = ownedGems <= 0 ? Color.red : Color.white;
            m_OfferingGem.gameObject.SetActive(true);
        }
        else
        {
            m_OfferingGem.gameObject.SetActive(false);
        }

        //tool
        if (string.IsNullOrEmpty(details.tool) == false)
        {
            IngredientDict tool = DownloadedAssets.GetIngredient(details.tool);
            ownedTools = PlayerDataManager.playerData.ingredients.Amount(details.tool);
            hasRequiredIngredients &= ownedTools > 0;

            m_OfferingTool.text = string.Concat(LocalizeLookUp.GetText("pop_required_ingredients").Replace("{{ingredient}}", string.Concat("1 ", tool.name)));// + " (1/" + ownedTools + ")";
            if (ownedTools != 0)
                {
                m_OfferingTool.text = string.Concat(m_OfferingTool.text, " (1/1)");
                }  
            else 
                {
                m_OfferingTool.text = string.Concat(m_OfferingTool.text, " (0/1)");
                } 
            m_OfferingTool.color = ownedTools <= 0 ? Color.red : Color.white;
            m_OfferingTool.gameObject.SetActive(true);
        }
        else
        {
            m_OfferingTool.gameObject.SetActive(false);
        }

        m_OfferingConfirmBtn.interactable = hasRequiredIngredients;

        m_OfferingTweenId = LeanTween.value(m_OfferingCanvasGroup.alpha, 1, 0.5f)
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                m_OfferingCanvasGroup.gameObject.SetActive(true);
            })
            .setOnUpdate((float v) =>
            {
                m_OfferingCanvasGroup.alpha = v * v;
                m_OfferingCanvasGroup.transform.localScale = new Vector3(v, v, v);
            })
            .uniqueId;
    }

    private void CloseOfferingScreen()
    {
        LeanTween.cancel(m_OfferingTweenId);
        m_OfferingTweenId = LeanTween.value(m_OfferingCanvasGroup.alpha, 0, 0.5f)
            .setEaseInCubic()
            .setOnUpdate((float v) =>
            {
                m_OfferingCanvasGroup.alpha = v;
                m_OfferingCanvasGroup.transform.localScale = new Vector3(v * v, v * v, v * v);
            })
            .setOnComplete(() =>
            {
                m_OfferingCanvasGroup.gameObject.SetActive(false);
            })
            .uniqueId;
    }


    private void ShowLoadingBlock()
    {
        m_Loading.gameObject.SetActive(true);
        m_LoadingBlock.gameObject.SetActive(true);

        LeanTween.cancel(m_LoadingBlockTweenId);
        m_LoadingBlockTweenId = LeanTween.value(m_LoadingBlock.alpha, 1f, 0.5f)
            .setOnUpdate((float v) =>
            {
                m_LoadingBlock.alpha = v;
                m_Loading.alpha = v;
            })
            .uniqueId;
    }

    private void HideLoadingBlock()
    {
        LeanTween.cancel(m_LoadingBlockTweenId);
        m_LoadingBlockTweenId = LeanTween.value(m_LoadingBlock.alpha, 0f, 0.5f)
            .setOnUpdate((float v) =>
            {
                m_LoadingBlock.alpha = v;
                m_Loading.alpha = v;
            })
            .setOnComplete(() =>
            {
                m_Loading.gameObject.SetActive(false);
                m_LoadingBlock.gameObject.SetActive(false);
            })
            .uniqueId;
    }

    private string GetTime(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }

        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = dtDateTime.Subtract(System.DateTime.UtcNow);
        string stamp = "";

        if (timeSpan.Days > 1)
        {
            stamp = string.Concat(timeSpan.Days.ToString(), " ", LocalizeLookUp.GetText("lt_time_days"), ", ");

        }
        else if (timeSpan.Days == 1)
        {
            stamp = string.Concat(timeSpan.Days.ToString(), " ", LocalizeLookUp.GetText("lt_time_day"),", ");
        }
        if (timeSpan.Hours > 1)
        {
            stamp += string.Concat(timeSpan.Hours.ToString(), " ", LocalizeLookUp.GetText("lt_time_hours"), " ");// hours ";
        }
        else if (timeSpan.Hours == 1)
        {
            stamp += string.Concat(timeSpan.Hours.ToString(), " ", LocalizeLookUp.GetText("lt_time_hour"), " ");// hour ";
        }
        else
        {
            if (timeSpan.Minutes > 1)
            {
                stamp += string.Concat(timeSpan.Minutes.ToString(), " ", LocalizeLookUp.GetText("lt_time_minutes"), " ");// minutes ";
            }
            else if (stamp.Length < 4)
            {
                stamp.Remove(4);
            }
        }
        return stamp;
    }

    private IEnumerator CooldownCoroutine(float totalseconds, TextMeshProUGUI textMesh)
    {
        int minutes, seconds;
        while (totalseconds > 0)
        {
            minutes = (int)totalseconds / 60;
            seconds = (int)(totalseconds % 60);

            if (minutes > 0)
                textMesh.text = string.Concat(LocalizeLookUp.GetText("pop_cooldown").Replace("{{time}}", string.Concat(minutes, LocalizeLookUp.GetText("lt_time_minutes")," ", seconds, LocalizeLookUp.GetText("lt_time_secs"), " ")));
            else
                textMesh.text = string.Concat(LocalizeLookUp.GetText("pop_cooldown").Replace("{{time}}", string.Concat(seconds, LocalizeLookUp.GetText("lt_time_secs"))));

            yield return new WaitForSeconds(1.001f);
            totalseconds -= 1;
        }
        textMesh.text = "";
        Setup(details);
    }



    private void OnClickEnter()
    {
        SoundManagerOneShot.Instance.SetBGTrack(1);
        ShowLoadingBlock();
        SoundManagerOneShot.Instance.PlayButtonTap();
        PlaceOfPower.EnterPoP(marker, details, (result, response) =>
        {
            if (result == 200)
            {
                //close the UI
                Close();
            }
            else
            {
                //show error and hide the loading block
                UIGlobalErrorPopup.ShowError(HideLoadingBlock, string.Concat(LocalizeLookUp.GetText("generic_error"), " ", response));
            }
        });
    }


    private void OnClickOffering()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        ShowOfferingScreen();
    }

    private void OnOfferingConfirm()
    {
        SoundManagerOneShot.Instance.IngredientAdded();
        CloseOfferingScreen();
        PlaceOfPower.StartOffering(this.tokenData.instance, (result, response) =>
        {
            if (result != 200)
            {
                int errorCode;
                string errorMessage;

                if (int.TryParse(response, out errorCode))
                    errorMessage = errorCode.ToString();
                else
                    errorMessage = "unknown";

                UIGlobalErrorPopup.ShowError(() => Close(), errorMessage);
            }
            else
            {
                /*{
                    "location":"local:4968b67d-b471-4d13-98dc-c3b4ecca5f62",
                    "name":"East Green Lake Dr N & Sunnyside Ave N",
                    "physicalOnly":false,
                    "level":1,
                    "buff":
                    {
                        "id":"reach",
                        "type":"spirit",
                        "spiritId":"spirit_kijo",
                        "buff":7
                    }
                }*/

                //close the UI and send another request for map/select for the updated marker data
                Close();
                LeanTween.value(0, 0, 0.1f).setOnComplete(() => MarkerSpawner.Instance.onClickMarker(marker));
            }         
        });
    }

    private void OnOfferingCancel()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        CloseOfferingScreen();
    }

    private void OnClickClose()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        Close();
    }

}