using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovenWebsocketController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        WebSocketClient.OnResponseParsedEvt += WebSocketClient_OnResponseParsetEvt;
    }

    private void WebSocketClient_OnResponseParsetEvt(WSData pResp)
    {
        if (pResp.command == Constants.Commands.coven_was_allied)
        {
            TeamManager.OnReceiveCovenAlly(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_was_unallied)
        {
            TeamManager.OnReceiveCovenUnally(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_ally)
        {
            TeamManager.OnReceiveCovenMemberAlly(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_unally)
        {
            TeamManager.OnReceiveCovenMemberUnally(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_kick)
        {
            TeamManager.OnReceiveCovenMemberKick(pResp);
        }
        else if(pResp.command == Constants.Commands.coven_member_request)
        {
            TeamManager.OnReceiveCovenMemberRequest(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_promote)
        {
            TeamManager.OnReceiveCovenMemberPromote(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_title_change)
        {
            TeamManager.OnReceiveCovenMemberTitleChange(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_join)
        {
            TeamManager.OnReceiveCovenMemberJoin(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_request_invite)  //NOT IMPLEMENTED
        {
            TeamManager.OnReceiveRequestInvite(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_leave)
        {
            TeamManager.OnReceiveCovenMemberLeave(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_disbanded)
        {
            TeamManager.OnReceiveCovenDisbanded(pResp);
        }
        else if (pResp.command == Constants.Commands.character_coven_invite)
        {
            TeamManager.OnReceivedCovenInvite(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_invited)
        {
            TeamManager.OnReceivedPlayerInvited(pResp);
        }
        else if (pResp.command == Constants.Commands.character_coven_reject)
        {
            TeamManager.OnReceiveRequestRejected(pResp);
        }
    }

}

