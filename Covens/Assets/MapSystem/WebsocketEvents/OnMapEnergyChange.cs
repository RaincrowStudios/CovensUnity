using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using Raincrow.Maps;

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

    public static void HandleEvent(WSData data)
    {
        PlayerDataDetail player = PlayerDataManager.playerData;
        IMarker marker;
        int energy;

        if (data.instance == player.instance) //update the players energy
        {
            if (player.lastEnergyUpdate > data.timeStamp)
                return;

            marker = PlayerManager.marker;
            player.lastEnergyUpdate = data.timeStamp;
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
                    OnPlayerDead?.Invoke();
                    DeathState.Instance.ShowDeath();
                }
                // else if (data.newState == "vulnerable")
                // {
                //     PlayerManagerUI.Instance.ShowElixirVulnerable(false);
                // }

                player.state = data.newState;
            }

            //Making sure energy not over 2x base
            if (player.energy >= (2 * player.baseEnergy))
                player.energy = player.baseEnergy * 2;
            
            PlayerManagerUI.Instance.UpdateEnergy();
        }
        else //update another witch's energy
        {
            marker = MarkerManager.GetMarker(data.instance);
            if (marker == null)
                return;

            if (marker.token.lastEnergyUpdate > data.timeStamp)
                return;
            
            Token token = marker.customData as Token;
            token.lastEnergyUpdate = data.timeStamp;
            energy = token.energy = data.newEnergy;
            marker.UpdateEnergy(token.energy, token.baseEnergy);

            //update the state
            if (data.newState == "dead" || data.newEnergy <= 0)
            {
                token.state = "dead";

                if (token.Type == MarkerSpawner.MarkerType.witch)
                    SpellcastingFX.SpawnDeathFX(token.instance, marker);
                else if (marker.type == MarkerSpawner.MarkerType.spirit)
                    OnMapTokenRemove.ForceEvent(data.instance);
            }
            else
            {
                if (token.state == "dead")
                {
                    token.state = data.newState;
                    SpellcastingFX.DespawnDeathFX(token.instance, marker);
                }
            }
        }

        OnEnergyChange?.Invoke(data.instance, energy);
    }

    public static void ForceEvent(IMarker marker,  int newEnergy, double timestamp)
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
            instance = marker.token.instance;
            baseEnergy = marker.token.baseEnergy;
        }

        string newState;
        if (newEnergy < baseEnergy * 0.2f)
            newState = "vulnerable";
        else if (newEnergy <= 0)
            newState = "dead";
        else
            newState = "";

        WSData data = new WSData
        {
            instance = instance,
            newEnergy = newEnergy,
            newState = newState,
            timeStamp = timestamp,
        };

        HandleEvent(data);
    }
}
