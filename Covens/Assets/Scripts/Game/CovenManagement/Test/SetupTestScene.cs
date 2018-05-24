using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupTestScene : MonoBehaviour {

    public bool m_IsInCoven;
    public bool m_CanJoinCoven;
    public CovenController.CovenRole m_CurrentRole;
    public bool m_IsCovenAnAlly;
    public string m_CovenName = "Hugo's coven";

    // Use this for initialization
    void Awake ()
    {
        /*CovenController pPlayer = new CovenController(m_CovenName);
        //pPlayer.IsInCoven = m_IsInCoven;
        pPlayer.CurrentRole = m_CurrentRole;
        pPlayer.IsCovenAnAlly = m_IsCovenAnAlly;       

        CovenController.Player = pPlayer;*/

	}
}
