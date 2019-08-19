using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

public static class OnMapEnergyChange
{
    /// <summary>
    /// triggered when any marker has its energy updated
    /// <para>
    ///     params (string:instance, int:energy)
    /// </para>
    /// </summary>
    public static event System.Action<string, int> OnEnergyChange;

    /// <summary>
    /// triggered when the local player dies
    /// </summary>
    public static event System.Action OnPlayerDead;

    public static void HandleEvent(string instance, int newEnergy, string newState, double timestamp)
    {
        PlayerData player = PlayerDataManager.playerData;
        IMarker marker;
        int energy;

        if (instance == player.instance) //update the players energy
        {
            if (player.lastEnergyUpdate > timestamp)
                return;

            marker = PlayerManager.marker;
            player.lastEnergyUpdate = timestamp;
            energy = player.energy = newEnergy;

            if (player.state == "dead" && newState != "dead")
            {
                player.state = newState;
                player.energy = newEnergy;
                PlayerManager.witchMarker.RemoveDeathFX();
                DeathState.Instance.Revived();
            }

            if (newState != player.state)
            {
                if (newState == "dead")
                {
                    if (!LocationIslandController.isInBattle)
                    {
                        PlayerManager.witchMarker.AddDeathFX();
                        DeathState.Instance.ShowDeath();
                        OnPlayerDead?.Invoke();
                    }
                    else
                    {
                        // dead in a location
                        LoadPOPManager.UnloadScene(null);
                    }
                }
                else if (newState == "vulnerable")
                {
                    if (LowEnergyPopup.Instance == null)
                    {
                        if (!LocationIslandController.isInBattle)
                        {
                            Utilities.InstantiateObject(Resources.Load<GameObject>("UILowEnergyPopUp"), DeathState.Instance.transform);
                        }
                    }
                }

                player.state = newState;
            }

            //Making sure energy not over 2x base
            if (player.energy >= (2 * player.baseEnergy))
                player.energy = player.baseEnergy * 2;

            PlayerManagerUI.Instance.UpdateEnergy();
        }
        else //update another witch's energy
        {
            marker = MarkerManager.GetMarker(instance);
            if (marker == null)
                return;

            if (marker.Token.lastEnergyUpdate > timestamp)
                return;

            CharacterToken token = marker.Token as CharacterToken;
            token.lastEnergyUpdate = timestamp;
            energy = token.energy = newEnergy;
            marker.UpdateEnergy();

            //update the state
            if (newState == "dead" || newEnergy <= 0)
            {
                token.state = "dead";

                if (token.Type == MarkerSpawner.MarkerType.WITCH)
                    (marker as WitchMarker).AddDeathFX();
                else if (marker.Type == MarkerSpawner.MarkerType.SPIRIT)
                    RemoveTokenHandler.ForceEvent(instance);
            }
            else
            {
                if (token.state == "dead")
                {
                    token.state = newState;

                    if (token.Type == MarkerSpawner.MarkerType.WITCH)
                        (marker as WitchMarker).RemoveDeathFX();
                }
            }
        }

        OnEnergyChange?.Invoke(instance, energy);
    }

    public static void ForceEvent(IMarker marker, int newEnergy, double timestamp)
    {
        if (marker == null || marker.isNull)
            return;

        string instance;
        int baseEnergy;

        if (marker == PlayerManager.marker)
        {
            instance = PlayerDataManager.playerData.instance;
            baseEnergy = PlayerDataManager.playerData.baseEnergy;
        }
        else
        {
            instance = (marker.Token as CharacterToken).instance;
            baseEnergy = (marker.Token as CharacterToken).baseEnergy;
        }

        string newState; if (newEnergy <= 0)
            newState = "dead";
        else if (newEnergy < baseEnergy * 0.2f)
            newState = "vulnerable";
        else
            newState = "";

        HandleEvent(instance, newEnergy, newState, timestamp);
    }
}
