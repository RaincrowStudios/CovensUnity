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
    public static event System.Action<IMarker, int> OnMarkerEnergyChange;

    /// <summary>
    /// triggered when the local player dies
    /// </summary>
    public static event System.Action OnPlayerDead;
    public static event System.Action OnPlayerRevived;
    public static event System.Action OnPlayerVulnerable;
    public static event System.Action<int> OnPlayerEnergyChange;
    public static event System.Action<string> OnPlayerStateChange;

    public static void HandleEvent(string instance, int newEnergy, string newState, double timestamp)
    {
        PlayerData player = PlayerDataManager.playerData;
        IMarker marker;
        int energy;

        if (instance == player.instance) //update the players energy
        {
            marker = PlayerManager.marker;

            if (marker.Token.lastEnergyUpdate > timestamp)
                return;

            marker.Token.lastEnergyUpdate = timestamp;
            energy = (marker.Token as CharacterToken).energy = player.energy = Mathf.Clamp(newEnergy, 0, player.maxEnergy);
            string previousState = player.state;
            player.state = newState;

            OnPlayerEnergyChange?.Invoke(energy);

            if (newState != previousState)
            {
                OnPlayerStateChange?.Invoke(newState);

                if (previousState == "dead")
                    OnPlayerRevived?.Invoke();
                else if (newState == "dead")
                    OnPlayerDead?.Invoke();
                else if (newState == "vulnerable")
                    OnPlayerVulnerable?.Invoke();
            }
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
            energy = token.energy = Mathf.Clamp(newEnergy, 0, token.maxEnergy);
            token.state = newState;
        }

        OnEnergyChange?.Invoke(instance, energy);
        if (marker != null)
            OnMarkerEnergyChange?.Invoke(marker, energy);
    }

    public static void ForceEvent(IMarker marker, int newEnergy)
    {
        Debug.Log("Forcing Update Energy");
        ForceEvent(marker, newEnergy, marker.Token.lastEnergyUpdate + Time.deltaTime);
    }

    public static void ForceEvent(IMarker marker, int newEnergy, double timestamp)
    {
        if (marker == null)
        {
            Debug.LogError("trying to update energy of null marker");
            return;
        }

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
