using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using Raincrow.Maps;
using System.Collections.Generic;

public class ShowSelectionCard : UIAnimationManager
{
    public static ShowSelectionCard Instance { get; set; }

    public GameObject SpiritCard;
    public GameObject PortalCard;
    public GameObject WitchCard;
    public GameObject LocationCard;

    [SerializeField] Text selfEnergy;

    public static GameObject currCard;

    public static MarkerSpawner.MarkerType selectedType;


    void Awake()
    {
        Instance = this;
    }

    public void ChangeSelfEnergy()
    {
        selfEnergy.text = "Energy : " + PlayerDataManager.playerData.energy.ToString();
    }

    public void Attack()
    {
        if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.location)
        {
            if (!FirstTapVideoManager.Instance.CheckSpellCasting())
                return;
        }

        // disable cast

        //if (!PlayerManager.Instance.fly)
        //    PlayerManager.Instance.Fly();

        if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.location)
            MapSelection.Instance.OnSelect();
        else
            LocationUIManager.Instance.TryEnterLocation();
    }

}
