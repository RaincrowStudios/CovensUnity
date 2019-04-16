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

    public void Show(IMarker marker)
    {
        Token markerData = marker.customData as Token;
        selectedType = markerData.Type;
        ChangeSelfEnergy();
        var data = MarkerSpawner.SelectedMarker;
        if (markerData.Type == MarkerSpawner.MarkerType.spirit)
        {
            currCard = Instantiate(SpiritCard);
        }
        else if (markerData.Type == MarkerSpawner.MarkerType.portal)
        {
            currCard = Instantiate(PortalCard);
        }
        else if (markerData.Type == MarkerSpawner.MarkerType.witch)
        {
            currCard = Instantiate(WitchCard);
        }
        else if (markerData.Type == MarkerSpawner.MarkerType.location)
        {
            currCard = Instantiate(LocationCard);

            UILocationInfo locationSelectionCard = currCard.GetComponent<UILocationInfo>();
            locationSelectionCard.Show(marker);
        }
    }

    public void SetupDetails(MarkerSpawner.MarkerType markerType, MarkerDataDetail markerDetail)
    {
        if (currCard != null)
        {
            if (markerType == MarkerSpawner.MarkerType.location)
            {
                UILocationInfo locationSelectionCard = currCard.GetComponent<UILocationInfo>();
                locationSelectionCard.SetupDetails(markerDetail);
            }
        }        
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
