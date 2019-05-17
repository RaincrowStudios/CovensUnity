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
                    OnPlayerDead?.Invoke();
                    DeathState.Instance.ShowDeath();
                }
                // else if (data.newState == "vulnerable")
                // {
                //     PlayerManagerUI.Instance.ShowElixirVulnerable(false);
                // }

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
            marker.UpdateEnergy(token.energy, token.baseEnergy);

            //update the state
            if (data.newState == "dead")
            {
                if (token.state != "dead")
                {
                    token.state = data.newState;
                    if (token.Type == MarkerSpawner.MarkerType.witch)
                        SpellcastingFX.SpawnDeathFX(token.instance, marker);
                }
                if (marker.type == MarkerSpawner.MarkerType.spirit)
                {
                    var remove_data = new WSData
                    {
                        instance = data.instance
                    };
                    Debug.LogError("spirit died, forcing removal");
                    OnMapTokenRemove.HandleEvent(remove_data);
                }
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
}
