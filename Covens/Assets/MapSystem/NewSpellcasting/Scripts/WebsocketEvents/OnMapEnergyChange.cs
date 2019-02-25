using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class OnMapEnergyChange
{
    public static void HandleEvent(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        if (data.instance == player.instance)
        {
            player.energy = data.newEnergy;
            if (player.state != "dead" && data.newState == "dead")
            {
                if (IsoPortalUI.isPortal)
                    IsoPortalUI.instance.DisablePortalCasting();

                if (MapSelection.currentView == CurrentView.IsoView)
                {
                    //StartCoroutine(DelayExitIso());
                    player.state = data.newState;
                    PlayerManagerUI.Instance.UpdateEnergy();
                    return;
                }
                else if (MapSelection.currentView == CurrentView.MapView && !LocationUIManager.isLocation)
                {
                    DeathState.Instance.ShowDeath();
                }
            }
            if (player.state != "vulnerable" && data.newState == "vulnerable")
            {
                //						print ("Vulnerable!");
                PlayerManagerUI.Instance.ShowElixirVulnerable(false);
            }

            if (player.state == "dead" && data.newState != "dead")
            {
                DeathState.Instance.Revived();
            }
            player.state = data.newState;
            //				SpellCarouselManager.Instance.WSStateChange ();
            PlayerManagerUI.Instance.UpdateEnergy();
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                ShowSelectionCard.Instance.ChangeSelfEnergy();
            }
        }
        if (MarkerSpawner.instanceID == data.instance)
        {
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.portal)
                {
                    if (data.newState != "dead")
                    {
                        //								print ("Energy Change Portal to " + data.newEnergy);
                        IsoPortalUI.instance.PortalFX(data.newEnergy);
                        MarkerSpawner.SelectedMarker.energy = data.newEnergy;
                        IsoTokenSetup.Instance.ChangeEnergy();
                    }
                    else
                    {
                        IsoPortalUI.instance.Destroyed();
                    }
                    return;
                }
                if (MarkerSpawner.SelectedMarker.state != "dead" && data.newState == "dead")
                {
                    if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.spirit)
                    {
                        HitFXManager.Instance.TargetDead(true);
                        //							print ("Closing Spirit Off!");
                        return;
                    }
                    else
                    {
                        HitFXManager.Instance.TargetDead();
                    }
                }
                else if (MarkerSpawner.SelectedMarker.state == "dead" && data.newState != "dead")
                {
                    HitFXManager.Instance.TargetRevive();
                }
                string oldState = MarkerSpawner.SelectedMarker.state;
                MarkerSpawner.SelectedMarker.state = data.newState;
                MarkerSpawner.SelectedMarker.energy = data.newEnergy;
                if (oldState != data.newState)
                {
                    SpellManager.Instance.StateChanged();
                }

                IsoTokenSetup.Instance.ChangeEnergy();
            }
            if (MapSelection.currentView == CurrentView.IsoView && ShowSelectionCard.selectedType == MarkerSpawner.MarkerType.witch)
            {
                if (ShowSelectionCard.currCard != null)
                    ShowSelectionCard.currCard.GetComponent<PlayerSelectionCard>().ChangeEnergy();
            }

        }
    }
}
