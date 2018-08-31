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

    private void WebSocketClient_OnResponseParsetEvt(WebSocketResponse pResp)
    {
        Debug.Log(">> WebSocketClient_OnResponseParsetEvt");
        if (pResp.command == Constants.Commands.coven_member_ally)
        {
            /*{
                "command":"coven_member_ally",
                "member":"okthugo021",
                "coven":"okt-19"
            }*/
            CovenController.Player.OnReceiveCovenMemberAlly(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_unally)
        {
            /*{
                "command":"coven_member_unally",
                "member":"okthugo021",
                "coven":"okt-19"
            }*/
            CovenController.Player.OnReceiveCovenMemberUnally(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_kick)
        {
            /*{
                "command":"coven_member_kick",
                "coven":"okt-19"
            }*/
            CovenController.Player.OnReceiveCovenMemberKick(pResp);
        }
        else if(pResp.command == Constants.Commands.coven_member_request)
        {
            CovenController.Player.OnReceiveCovenMemberRequest(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_promote)
        {
            CovenController.Player.OnReceiveCovenMemberPromote(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_title_change)
        {
            CovenController.Player.OnReceiveCovenMemberTitleChange(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_join)
        {
            CovenController.Player.OnReceiveCovenMemberJoin(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_was_allied)
        {
            CovenController.Player.OnReceiveCovenAlly(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_was_unallied)
        {
            CovenController.Player.OnReceiveCovenUnally(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_request_invite)
        {
            CovenController.Player.OnReceiveRequestInvite(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_member_leave)
        {
            CovenController.Player.OnReceiveCovenMemberLeave(pResp);
        }
        else if (pResp.command == Constants.Commands.coven_disbanded)
        {
            CovenController.Player.OnReceiveCovenMemberLeave(pResp);
        }
        


    }
}

