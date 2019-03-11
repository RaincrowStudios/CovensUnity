using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using Raincrow.Maps;

public static class OnMapEnergyChange
{
    public static System.Action<string, int> OnEnergyChange;
    
    public static void HandleEvent(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;
        IMarker marker;
        int level;
        int energy;

        if (data.instance == player.instance) //update the players energy
        {
            marker = PlayerManager.marker;
            level = PlayerDataManager.playerData.level;
            energy = player.energy = data.newEnergy;

            if (player.state == "dead" && data.newState != "dead")
            {
                player.state = data.newState;
                player.energy = data.newEnergy;
                DeathState.Instance.Revived();
            }

            if (data.newState != player.state)
            {
                if (data.newState == "dead")
                {
                    DeathState.Instance.ShowDeath();
                }
                else if (data.newState == "vulnerable")
                {
                    PlayerManagerUI.Instance.ShowElixirVulnerable(false);
                }

                player.state = data.newState;
            }

            PlayerManagerUI.Instance.UpdateEnergy();
        }
        else //update another witch's energy
        {
            marker = MarkerManager.GetMarker(data.instance);
            if (marker == null)
                return;

            Token token = marker.customData as Token;
            level = token.level;
            energy = token.energy = data.newEnergy;
            marker.SetStats(token.level, token.energy);

            //update the state
            if (data.newState == "dead")
            {
                if (token.state != "dead")
                {
                    token.state = data.state;
                    SpellcastingFX.SpawnDeathFX(token.instance, marker);
                }
            }
            else
            {
                if (token.state == "dead")
                {
                    token.state = data.state;
                    SpellcastingFX.DespawnDeathFX(token.instance, marker);
                }
            }
        }


        marker.SetStats(level, energy);

        OnEnergyChange?.Invoke(data.instance, energy);

        return;

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
