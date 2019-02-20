using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class PlayerSelectionCard : MonoBehaviour
{
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI degree;
    public TextMeshProUGUI coven;
    public TextMeshProUGUI dominion;
    public TextMeshProUGUI dominionRank;
    public TextMeshProUGUI worldRank;
    public TextMeshProUGUI energy;
    public TextMeshProUGUI castButton;
    public Image schoolSigil;
    public Sprite whiteSchool;
    public Sprite shadowSchool;
    public Sprite greySchool;
    public GameObject Immune;
    public ApparelView male;
    public ApparelView female;
    public Button btnCoven;
    public GameObject InviteToCoven;
    public Button inviteButton;
    public TextMeshProUGUI InviteText;
    public GameObject inviteLoading;
    public Button close;

    void Start()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.alphaCanvas(cg, 1, .4f);
        var data = MarkerSpawner.SelectedMarker;

        if (MarkerSpawner.SelectedMarker.equipped[0].id.Contains("_m_"))
        {
            female.gameObject.SetActive(false);
            male.gameObject.SetActive(true);
            male.InitializeChar(MarkerSpawner.SelectedMarker.equipped);
        }
        else
        {
            female.gameObject.SetActive(true);
            male.gameObject.SetActive(false);
            female.InitializeChar(MarkerSpawner.SelectedMarker.equipped);
        }
        ChangeDegree();
        displayName.text = data.displayName;
        level.text = "Level: " + data.level.ToString();
        dominion.text = "Dominion: " + data.dominion;
        dominionRank.text = "Dominion Rank: " + data.dominionRank;
        worldRank.text = "World Rank: " + data.worldRank;
        coven.text = "Coven: " + (data.covenName == "" ? "None" : data.covenName);

        if (!string.IsNullOrEmpty(data.covenName))
        {
            btnCoven.onClick.AddListener(() =>
                {
                    TeamManagerUI.Instance.Show(data.covenName);
                });
        }


        energy.text = "Energy: " + data.energy.ToString();

        Invoke("SetupInviteToCoven", 1f);

        if (MarkerSpawner.ImmunityMap.ContainsKey(MarkerSpawner.instanceID))
        {
            if (MarkerSpawner.ImmunityMap[MarkerSpawner.instanceID].Contains(PlayerDataManager.playerData.instance))
            {
                SetCardImmunity(true);
            }
            else
            {
                SetCardImmunity(false);
            }
        }
        SetSilenced(BanishManager.isSilenced);
        castButton.GetComponent<Button>().onClick.AddListener(AttackRelay);
        close.onClick.AddListener(Close);
    }


    public void ChangeEnergy()
    {
        energy.text = "Energy : " + MarkerSpawner.SelectedMarker.energy.ToString();
    }

    public void ChangeLevel()
    {
        level.text = MarkerSpawner.SelectedMarker.level.ToString();
    }

    public void ChangeDegree()
    {
        degree.text = Utilities.witchTypeControlSmallCaps(MarkerSpawner.SelectedMarker.degree);
        if (MarkerSpawner.SelectedMarker.degree < 0)
        {
            schoolSigil.sprite = shadowSchool;
            schoolSigil.color = Utilities.Purple;
        }
        else if (MarkerSpawner.SelectedMarker.degree > 0)
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

    void SetupInviteToCoven()
    {
        if (PlayerDataManager.playerData.covenName != "")
        {
            if (MarkerSpawner.SelectedMarker.covenName == "")
            {
                this.gameObject.SetActive(true);
                InviteToCoven.SetActive(true);
                InviteText.text = DownloadedAssets.localizedText[LocalizationManager.invite_coven];
                inviteLoading.SetActive(false);
                inviteButton.onClick.AddListener(SendInviteRequest);
                InviteText.color = Color.white;
            }
        }
        else
        {
            InviteToCoven.SetActive(false);
        }
    }

    public void SendInviteRequest()
    {
        var data = new { invited = MarkerSpawner.instanceID };
        inviteLoading.SetActive(true);
        APIManager.Instance.PostData("coven/invite", JsonConvert.SerializeObject(data), requestResponse);
    }

    public void requestResponse(string s, int r)
    {
        inviteLoading.SetActive(false);
        if (r == 200)
        {
            inviteButton.onClick.RemoveListener(SendInviteRequest);
            InviteText.text = "Invitation Sent!";
        }
        else
        {
            Debug.Log(s);
            if (s == "4803")
            {
                InviteText.text = "Invitation already Sent!";
                InviteText.color = Color.red;
            }
            else
            {
                InviteText.text = "Invite Failed...";
                InviteText.color = Color.red;
            }
            inviteButton.onClick.RemoveListener(SendInviteRequest);
        }
    }


    public void SetCardImmunity(bool isImmune)
    {
        if (BanishManager.isSilenced)
            return;
        if (isImmune)
        {
            Immune.SetActive(true);
            castButton.text = "Cast";
            castButton.GetComponent<Button>().enabled = false;
        }
        else
        {
            castButton.gameObject.SetActive(true);
            castButton.text = "Cast";
            castButton.GetComponent<Button>().enabled = true;
            Immune.SetActive(false);
        }
    }


    //Something about the cast button
    private void EnableCastButton(bool enable)
    {
        if (enable)
            castButton.enabled = true;
        else
            castButton.enabled = false;
    }

    void SetSilenced(bool isTrue)
    {
        Button castingButton = castButton.GetComponent<Button>();
        if (isTrue)
        {

            castButton.color = Color.gray;

            castingButton.enabled = false;
            castButton.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            StartCoroutine(SilenceUpdateTimer());
        }
        else
        {
            castButton.color = Color.white;
            castingButton.enabled = true;
            castButton.text = "Cast";
        }
    }


    IEnumerator SilenceUpdateTimer()
    {
        while (BanishManager.isSilenced)
        {
            castButton.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            yield return new WaitForSeconds(1);
        }
    }

    void AttackRelay()
    {
        ShowSelectionCard.Instance.Attack();
        Close();
    }

    void Close()
    {
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .4f).setOnComplete(() => Destroy(gameObject));
    }
}