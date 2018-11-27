using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamItemData : MonoBehaviour
{
    public Text Level;
    public Text Username;

    public GameObject playerInviteObject;

    public Button AcceptBtn;
    public Button RejectBtn;
    public Button AllyBtn;
    public Button UnAllyBtn;

    public Text Status;
    public Text Title;
    public GameObject titleIcon;
    public Button LevelCloseBtn;

    public void Setup(TeamData data)
    {
      
    }

}
