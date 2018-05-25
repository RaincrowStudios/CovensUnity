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

    private void WebSocketClient_OnResponseParsetEvt(WebSocketResponse obj)
    {
        if (obj.command == Constants.Commands.coven_member_ally)
        {
            /*{
                "command":"coven_member_ally",
                "member":"okthugo021",
                "coven":"okt-19"
            }*/
        }
        else if (obj.command == Constants.Commands.coven_member_unally)
        {
            /*{
                "command":"coven_member_unally",
                "member":"okthugo021",
                "coven":"okt-19"
            }*/
        }
        else if (obj.command == Constants.Commands.coven_member_kick)
        {
            /*{
                "command":"coven_member_kick",
                "coven":"okt-19"
            }*/
        }
    }
}