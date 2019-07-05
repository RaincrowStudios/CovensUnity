using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPopInfoNew : MonoBehaviour
{
    private static UIPopInfoNew m_Instance;
    public static UIPopInfoNew Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIPopInfoNew>("UIPOPinfoNew_2"));
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

    public static void SetupDetails(LocationMarkerDetail data, string instance)
    {
        if (isOpen && Instance.m_Token.instance == instance)
            Instance.SetupDetails(data);
    }


    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_Loading;
    [SerializeField] private CanvasGroup m_LoadingBlock;

    [Header("sub UIs")]
    [SerializeField] private UIPopInfoUnclaimed m_Unclaimed;
    [SerializeField] private UIPopInfoClaimed m_Claimed;
    [SerializeField] private CanvasGroup m_LimitReachedPopup;
    [SerializeField] private Button m_LimitConfirmBtn;

    [SerializeField] private CanvasGroup m_PhysicalPopup;
    [SerializeField] private TextMeshProUGUI m_PhysicalDistance;
    [SerializeField] private Button m_PhysicalConfirmBtn;

    [Header("Offering")]
    [SerializeField] private CanvasGroup m_OfferingCanvasGroup;
    [SerializeField] private TextMeshProUGUI m_OfferingTitle;
    [SerializeField] private TextMeshProUGUI m_OfferingHerb;
    [SerializeField] private TextMeshProUGUI m_OfferingGem;
    [SerializeField] private TextMeshProUGUI m_OfferingTool;
    [SerializeField] private Button m_OfferingConfirmBtn;
    [SerializeField] private Button m_OfferingCancelBtn;


    private int m_LoadingBlockTweenId;
    private int m_OfferingTweenId;
    private int m_LimitReachedTweenId;
    private int m_PhysicalOnlyTweenId;

    private IMarker m_Marker;
    private Token m_Token;
    private LocationMarkerDetail m_LocationDetail;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_OfferingCanvasGroup.alpha = 0;
        m_Loading.alpha = 0;
        m_LoadingBlock.alpha = 0;
        m_Loading.gameObject.SetActive(false);

        m_OfferingConfirmBtn.onClick.AddListener(OnOfferingConfirm);
        m_OfferingCancelBtn.onClick.AddListener(OnOfferingCancel);

        m_Unclaimed.SetupListeners(OnClickEnter, OnClickOffering, OnClickClose);
        m_Claimed.SetupListeners(OnClickEnter, OnClickClose);

        m_Unclaimed.Close();
        m_Claimed.Close();

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
        m_Marker = marker;
        m_Token = data;
        m_LocationDetail = null;

        if (string.IsNullOrEmpty(data.owner))
            m_Unclaimed.Show(marker, data);
        else
            m_Claimed.Show(marker, data);

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.value(0f, 0f, 0.4f).setOnComplete(() => SoundManagerOneShot.Instance.PlayPostEffect2(0.5f));
        m_Loading.gameObject.SetActive(true);
        m_Loading.alpha = 1f;
    }

    public void SetupDetails(LocationMarkerDetail data)
    {
        LeanTween.alphaCanvas(m_Loading, 0, 0.5f).setOnComplete(() => m_Loading.gameObject.SetActive(false));

        m_LocationDetail = data;

        bool isUnclaimed = string.IsNullOrEmpty(data.controlledBy);

        if (isUnclaimed)
        {
            if (m_Claimed.IsOpen)
                m_Claimed.Close();
            if (m_Unclaimed.IsOpen == false)
                m_Unclaimed.Show(m_Marker, m_Token);
            m_Unclaimed.SetupDetails(data);
        }
        else
        {
            if (m_Unclaimed.IsOpen)
                m_Unclaimed.Close();
            if (m_Claimed.IsOpen == false)
                m_Claimed.Show(m_Marker, m_Token);
            m_Claimed.SetupDetails(data);
        }

        if (data.physicalOnly && PlayerManager.inSpiritForm)
        {
            ShowPhysicalOnly(null);
        }
        else
        {
            if (data.limitReached)
            {
                ShowLimitReached(null);
            }
        }
    }

    public void Close()
    {
        System.Action onClose = () =>
        {
            m_Canvas.enabled = false;
            m_InputRaycaster.enabled = false;

            m_Marker = null;
            m_Token = null;
            m_LocationDetail = null;
        };

        if (m_Claimed.IsOpen)
            m_Claimed.Close(onClose);
        else
            m_Unclaimed.Close(onClose);
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
        if (string.IsNullOrEmpty(m_LocationDetail.herb) == false)
        {
            ownedHerbs = PlayerDataManager.playerData.ingredients.Amount(m_LocationDetail.herb);
            hasRequiredIngredients &= ownedHerbs > 0;

            m_OfferingHerb.text = string.Concat(LocalizeLookUp.GetText("pop_required_ingredients").Replace("{{ingredient}}", string.Concat("1 ", LocalizeLookUp.GetCollectableName(m_LocationDetail.herb))));
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
        if (string.IsNullOrEmpty(m_LocationDetail.gem) == false)
        {
            ownedGems = PlayerDataManager.playerData.ingredients.Amount(m_LocationDetail.gem);
            hasRequiredIngredients &= ownedGems > 0;

            m_OfferingGem.text = string.Concat(LocalizeLookUp.GetText("pop_required_ingredients").Replace("{{ingredient}}", string.Concat("1 ", LocalizeLookUp.GetCollectableName(m_LocationDetail.gem))));
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
        if (string.IsNullOrEmpty(m_LocationDetail.tool) == false)
        {
            ownedTools = PlayerDataManager.playerData.ingredients.Amount(m_LocationDetail.tool);
            hasRequiredIngredients &= ownedTools > 0;

            m_OfferingTool.text = string.Concat(LocalizeLookUp.GetText("pop_required_ingredients").Replace("{{ingredient}}", string.Concat("1 ", LocalizeLookUp.GetCollectableName(m_LocationDetail.gem))));// + " (1/" + ownedTools + ")";
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


    private void ShowLimitReached(System.Action onConfirm)
    {
        bool isMine = false;
        if (m_LocationDetail.isCoven)
            isMine = m_LocationDetail.controlledBy == PlayerDataManager.playerData.covenName;
        else
            isMine = m_LocationDetail.controlledBy == PlayerDataManager.playerData.displayName;

        if (isMine)
            return;

        m_LimitConfirmBtn.onClick.RemoveAllListeners();
        m_LimitConfirmBtn.onClick.AddListener(() =>
        {
            m_LimitReachedPopup.interactable = false;
            m_LimitReachedPopup.blocksRaycasts = false;

            LeanTween.cancel(m_LimitReachedTweenId);
            m_LimitReachedTweenId = LeanTween.alphaCanvas(m_LimitReachedPopup, 0, 0.5f).uniqueId;
            onConfirm?.Invoke();
        });
        
        m_LimitReachedPopup.interactable = true;
        m_LimitReachedPopup.blocksRaycasts = true;

        LeanTween.cancel(m_LimitReachedTweenId);
        m_LimitReachedTweenId = LeanTween.alphaCanvas(m_LimitReachedPopup, 1, 0.25f).uniqueId;
    }

    private void ShowPhysicalOnly(System.Action onConfirm)
    {
        m_PhysicalConfirmBtn.onClick.RemoveAllListeners();
        m_PhysicalConfirmBtn.onClick.AddListener(() =>
        {
            m_PhysicalPopup.interactable = false;
            m_PhysicalPopup.blocksRaycasts = false;

            LeanTween.cancel(m_PhysicalOnlyTweenId);
            m_PhysicalOnlyTweenId = LeanTween.alphaCanvas(m_PhysicalPopup, 0, 0.5f).uniqueId;

            onConfirm?.Invoke();
        });

        m_PhysicalPopup.interactable = true;
        m_PhysicalPopup.blocksRaycasts = true;

        double distance = MapsAPI.Instance.DistanceBetweenPointsD(m_Marker.coords, MapsAPI.Instance.physicalPosition);
        string distanceString = "";
        if (distance < 0.1)
            distanceString = string.Format("{0:00}m", distance*1000);
        else if (distance < 1)
            distanceString = string.Format("{0:000}m", distance*1000);
        else
            distanceString = string.Format("{0:0.00}km", distance);

        m_PhysicalDistance.text = LocalizeLookUp.GetText("pop_physical_desc").Replace("{{Distance}}", distanceString);

        LeanTween.cancel(m_PhysicalOnlyTweenId);
        m_PhysicalOnlyTweenId = LeanTween.alphaCanvas(m_PhysicalPopup, 1, 0.25f).uniqueId;
    }



    private void OnClickEnter()
    {
        SoundManagerOneShot.Instance.SetBGTrack(1);
        ShowLoadingBlock();
        SoundManagerOneShot.Instance.PlayButtonTap();
        PlaceOfPower.EnterPoP(m_Marker, m_LocationDetail, (result, response) =>
        {
            HideLoadingBlock();
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

        ShowLoadingBlock();
        CloseOfferingScreen();

        PlaceOfPower.StartOffering(m_Token.instance, (result, response) =>
        {
            HideLoadingBlock();

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

                //request the new marker details from server
                MarkerSpawner.Instance.onClickMarker(m_Marker);

                Dictionary<string, object> offerResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

                if (offerResult.ContainsKey("name"))
                    UILocationClaimed.Instance.Show(offerResult["name"] as string);
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
