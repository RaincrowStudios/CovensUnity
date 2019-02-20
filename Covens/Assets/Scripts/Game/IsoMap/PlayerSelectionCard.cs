using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class PlayerSelectionCard : MonoBehaviour
{
    [Header("WitchCard")]
    //public GameObject WitchCard;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI degree;
    public TextMeshProUGUI coven;
    public TextMeshProUGUI dominion;
    public TextMeshProUGUI dominionRank;
    public TextMeshProUGUI worldRank;
    public TextMeshProUGUI energy;
    public TextMeshProUGUI castButton;
	//public Button castingButton;
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


	void Start()
	{
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
		castButton.gameObject.GetComponent<Button> ().onClick.AddListener (() => ShowSelectionCard.Instance.Attack ());

		if (btnCoven != null)
		{
			btnCoven.onClick.RemoveAllListeners();
			if (string.IsNullOrEmpty(data.covenName) == false)
			{
				btnCoven.onClick.AddListener(() =>
					{
						TeamManagerUI.Instance.Show(data.covenName);

						//not sure what to do here.
						//this.close();
					});
			}
		}

		//			SpellCarouselManager.targetType = "witch";
		//			degree.text = Utilities.witchTypeControlSmallCaps (data.degree);
		//			school.text = Utilities.GetSchool (data.degree);

		energy.text = "Energy: " + data.energy.ToString();

		Invoke("SetupInviteToCoven", 1f);

		if (MarkerSpawner.ImmunityMap.ContainsKey(MarkerSpawner.instanceID))
		{
			if (MarkerSpawner.ImmunityMap[MarkerSpawner.instanceID].Contains(PlayerDataManager.playerData.instance))
			{
				//Witch Card
				SetCardImmunity(true);
			}
			else
			{
				//
				SetCardImmunity(false);
			}
		}
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
        //		school.text = Utilities.GetSchool ( MarkerSpawner.SelectedMarker.degree);
    }

    void SetupInviteToCoven()
    {
        if (PlayerDataManager.playerData.covenName != "")
        {
            if (MarkerSpawner.SelectedMarker.covenName == "")
            {
				this.gameObject.SetActive (true);
                //StartCoroutine(FadeIn(InviteToCoven, 1));
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

	public void DestroySelf()
	{
		Destroy (ShowSelectionCard.currCard);
	}

	//Something about the cast button
	private void EnableCastButton(bool enable)
	{
		if (enable)
			castButton.enabled = true;
		else
			castButton.enabled = false;
	}

	public void SetSilenced(bool isTrue)
	{
		Button castingButton = castButton.gameObject.GetComponent<Button> ();
		if (isTrue)
		{
			
			castButton.color = Color.gray;

			castingButton.enabled = false;
//			foreach (var item in castButton)
//			{
//				item.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
//				item.GetComponent<Button>().enabled = false;
//			}
			StartCoroutine(SilenceUpdateTimer());
		}
		else
		{
			castButton.color = Color.white;
			castingButton.enabled = true;
//			foreach (var item in castButtons)
//			{
//				item.text = "Cast";
//				item.GetComponent<Button>().enabled = true;
//			}
		}
	}

	public void AttackRelay()
	{
		//ShowSelectionCard.Attack ();
	}

	IEnumerator SilenceUpdateTimer()
	{
		while (BanishManager.isSilenced)
		{
			//Something about being silenced here


			//if (isCardShown)
			//{
//			foreach (var item in castButton)
//			{
//				item.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
//			}
			//}
			yield return new WaitForSeconds(1);
		}
	}


    public void Setup(MarkerDataDetail data)
    {

    }
}