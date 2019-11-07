using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Team;
using UnityEngine.Events;

public class TeamInviteItemUI : MonoBehaviour
{
    [SerializeField] private GameObject m_Background;
    [SerializeField] private GameObject m_DisableOverlay;
    [SerializeField] private Text m_Level;
    [SerializeField] private Text m_Title;

    [SerializeField] private Button m_Button;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_DeclineButton;
    [SerializeField] private Button m_CancelButton;

    private System.Action m_OnSelect;
    private System.Action m_OnConfirm;
    private System.Action m_OnCancel;

    public string ItemId { get; private set; }
    public string ItemName { get; private set; }

    private void Awake()
    {
        m_Button.onClick.AddListener(OnSelect);
        m_ConfirmButton.onClick.AddListener(OnConfirm);
        m_CancelButton.onClick.AddListener(OnCancel);
        m_DeclineButton.onClick.AddListener(OnCancel);
    }

    private void OnSelect()
    {
        m_OnSelect?.Invoke();
    }

    private void OnConfirm()
    {
        m_OnConfirm?.Invoke();
    }

    private void OnCancel()
    {
        m_OnCancel?.Invoke();
    }

    //invitation MY COVEN sent to a player
    public void Setup(PendingInvite data, System.Action onCancel)
    {
        m_Background.SetActive(transform.GetSiblingIndex() % 2 == 0);
        //Disable(false);

        ItemId = data.Character;
        ItemName = data.Name;

        m_Level.text = data.Level.ToString();
        m_Title.text = data.Name;

        m_ConfirmButton.gameObject.SetActive(false);
        m_DeclineButton.gameObject.SetActive(false);
        m_CancelButton.gameObject.SetActive(true);

        m_ConfirmButton.interactable = false;
        m_DeclineButton.interactable = false;
        m_CancelButton.interactable = TeamManager.MyRole >= CovenRole.MODERATOR;

        m_OnSelect = () =>
        {
            TeamPlayerView.ViewCharacter(data.Character, null);
        };
        m_OnConfirm = null;
        m_OnCancel = () =>
        {
            //show confirm popup
            UIGlobalPopup.ShowPopUp(
                confirmAction: () =>
                {
                    //make the request
                    LoadingOverlay.Show();

                    TeamManager.CancelInvite(data.Character, (error) =>
                    {
                        LoadingOverlay.Hide();
                        if (string.IsNullOrEmpty(error))
                        {
                            UIGlobalPopup.ShowPopUp(onCancel, LocalizeLookUp.GetText("coven_invite_cancel_success"));
                        }
                        else
                        {
                            UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                        }
                    });
                },
                cancelAction: () => { },
                LocalizeLookUp.GetText("coven_invite_cancel"));
        };
    }

    //request MY COVEN received from a player
    public void Setup(PendingRequest data, System.Action onAccept, System.Action onReject)
    {
        m_Background.SetActive(transform.GetSiblingIndex() % 2 == 0);
        //Disable(false);

        ItemId = data.Character;
        ItemName = data.Name;

        m_Level.text = data.Level.ToString();
        m_Title.text = data.Name;

        m_ConfirmButton.gameObject.SetActive(true);
        m_DeclineButton.gameObject.SetActive(true);
        m_CancelButton.gameObject.SetActive(false);

        m_ConfirmButton.interactable = TeamManager.MyRole >= CovenRole.MODERATOR;
        m_DeclineButton.interactable = TeamManager.MyRole >= CovenRole.MODERATOR;
        m_CancelButton.interactable = false;

        m_OnSelect = () =>
        {
            LoadingOverlay.Show();
            TeamPlayerView.ViewCharacter(data.Character, null);
        };
        m_OnConfirm = () =>
        {
            //show confirm popup
            UIGlobalPopup.ShowPopUp(
                confirmAction: () =>
                {
                    LoadingOverlay.Show();

                    //make the request
                    TeamManager.AcceptRequest(data.Character, (error) =>
                    {
                        LoadingOverlay.Hide();
                        if (string.IsNullOrEmpty(error))
                        {
                            UIGlobalPopup.ShowPopUp(onAccept, LocalizeLookUp.GetText("coven_member_new").Replace("{{Coven Name}}", TeamManager.MyCovenData.Name));
                        }
                        else
                        {
                            UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                        }
                    });
                },
                cancelAction: () => { },
                LocalizeLookUp.GetText("coven_request_accept"));
        };
        m_OnCancel = () =>
        {
            //show confirm popup
            UIGlobalPopup.ShowPopUp(
                confirmAction: () =>
                {
                    LoadingOverlay.Show();

                    //make the request
                    TeamManager.RejectRequest(data.Character, (error) =>
                    {
                        LoadingOverlay.Hide();
                        if (string.IsNullOrEmpty(error))
                        {
                            UIGlobalPopup.ShowPopUp(onReject, LocalizeLookUp.GetText("coven_request_reject_success"));
                        }
                        else
                        {
                            UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                        }
                    });
                },
                cancelAction: () => { },
                LocalizeLookUp.GetText("coven_request_reject"));
        };
    }

    //invite I received from a coven
    public void Setup(CovenInvite data, System.Action onAccept, System.Action onReject)
    {
        m_Background.SetActive(transform.GetSiblingIndex() % 2 == 0);
        //Disable(false);

        ItemId = data.coven;
        ItemName = data.name;

        m_Level.text = data.worldRank.ToString();
        m_Title.text = data.name;

        m_ConfirmButton.gameObject.SetActive(true);
        m_DeclineButton.gameObject.SetActive(true);
        m_CancelButton.gameObject.SetActive(false);

        m_OnSelect = () =>
        {
            TeamManagerUI.Open(data.coven, () => TeamManagerUI.Open(TeamManager.MyCovenId));
        };

        m_OnConfirm = () =>
        {
            //show confirm popup
            UIGlobalPopup.ShowPopUp(
                confirmAction: () =>
                {
                    LoadingOverlay.Show();

                    //make the request
                    TeamManager.AcceptInvite(data.coven, (coven, error) =>
                    {
                        LoadingOverlay.Hide();
                        if (string.IsNullOrEmpty(error))
                        {
                            UIGlobalPopup.ShowPopUp(onAccept, LocalizeLookUp.GetText("coven_invite_join_success"));
                        }
                        else
                        {
                            UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                        }
                    });
                },
                cancelAction: () => { },
                LocalizeLookUp.GetText("coven_invite_join"));
        };

        m_OnCancel = () =>
        {
            //show confirm popup
            UIGlobalPopup.ShowPopUp(
                confirmAction: () =>
                {
                    LoadingOverlay.Show();

                    //make the request
                    TeamManager.DeclineInvite(data.coven, (error) =>
                    {
                        LoadingOverlay.Hide();
                        if (string.IsNullOrEmpty(error))
                        {
                            UIGlobalPopup.ShowPopUp(onReject, LocalizeLookUp.GetText("coven_decline_invite_success"));
                        }
                        else
                        {
                            UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                        }
                    });
                },
                cancelAction: () => { },
                LocalizeLookUp.GetText("coven_decline_invite"));
        };
    }

    //request I sent to another coven
    public void Setup(CovenRequest data)
    {
        m_Background.SetActive(transform.GetSiblingIndex() % 2 == 0);
        //Disable(false);

        ItemId = data.coven;
        ItemName = data.name;

        m_Level.text = data.worldRank.ToString();
        m_Title.text = data.name;

        m_ConfirmButton.gameObject.SetActive(false);
        m_DeclineButton.gameObject.SetActive(false);
        m_CancelButton.gameObject.SetActive(false);

        m_OnSelect = () =>
        {
            Debug.LogError("TODO: SHOW COVEN");

        };
        m_OnConfirm = null;
        m_OnCancel = null;
    }

    //public void Disable(bool disable)
    //{
    //    m_DisableOverlay.SetActive(disable); 
    //}
}
