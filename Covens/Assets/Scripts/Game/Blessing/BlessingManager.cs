using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlessingManager
{
    private struct DailyBlessingResponse
    {
        public double lastBlessing;
        public bool updated;
    }

    private static int m_TimerId;

    public static double LastBlessing
    {
        get => double.Parse(PlayerPrefs.GetString("LastBlessing." + PlayerDataManager.playerData.instance, "0"));
        set => PlayerPrefs.SetString("LastBlessing." + PlayerDataManager.playerData.instance, value.ToString());
    }

    public static void CheckDailyBlessing()
    {
        System.DateTime lastBlessingDate = Utilities.FromJavaTime(LastBlessing);
        System.DateTime nextBlessingDate = lastBlessingDate.AddHours(24);
        System.TimeSpan nextBlessing = nextBlessingDate - System.DateTime.UtcNow;
                
        if (nextBlessing.TotalSeconds > 0)
        {
            StartTimer((float)nextBlessing.TotalSeconds);
            return;
        } 

        APIManager.Instance.Get("dailies/blessing", (response, result) =>
        {
            if (result == 200)
            {
                DailyBlessingResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<DailyBlessingResponse>(response);

                LastBlessing = data.lastBlessing;
                
                if (data.updated)
                {
                    int energy = Mathf.Max(PlayerDataManager.playerData.energy, PlayerDataManager.playerData.baseEnergy);
                    int energyGained = energy - PlayerDataManager.playerData.energy;
                    OnMapEnergyChange.ForceEvent(PlayerManager.marker, energy, data.lastBlessing);
                    UIDailyBlessing.Show(energyGained);

                    Debug.Log("<color=magenta>daily blessing received</color>");
                }
                else
                {
                    Debug.Log("<color=magenta>daily blessing already claimed</color>");
                }

                lastBlessingDate = Utilities.FromJavaTime(LastBlessing);
                nextBlessingDate = lastBlessingDate.AddHours(24);
                nextBlessing = nextBlessingDate - System.DateTime.UtcNow;
                StartTimer((float)nextBlessing.TotalSeconds);
            }
            else
            {
                Debug.LogError("dailies/blessing\n["+result+"] " + response);
            }
        });

    }

    private static void StartTimer(float totalSeconds)
    {
        Debug.Log("next blessing check in " + totalSeconds + " seconds");
        LeanTween.cancel(m_TimerId);
        m_TimerId = LeanTween.value(0, 0, 0).setDelay(totalSeconds).setOnStart(CheckDailyBlessing).uniqueId;
    }

    //IEnumerator CheckTime()
    //{
    //    while (true)
    //    {
    //        if (System.DateTime.Now.Hour == 0 && System.DateTime.Now.Minute == 0 && System.DateTime.Now.Second == 0)
    //        {
    //            //TODO add daily blessing check
    //            yield return new WaitForSeconds(1);
    //            Debug.Log("Checking Reset");
    //            APIManager.Instance.Get("character/get", (string s, int r) =>
    //            {

    //                if (r == 200)
    //                {
    //                    var rawData = JsonConvert.DeserializeObject<PlayerData>(s);
    //                    if (rawData.dailyBlessing)
    //                    {
    //                        PlayerDataManager.playerData.blessing = rawData.blessing;
    //                        ShowBlessing();
    //                    }
    //                }

    //            });
    //        }

    //        yield return new WaitForSeconds(1);

    //        if (EnergyElixir.activeSelf)
    //        {
    //            if (PlayerDataManager.playerData.energy > PlayerDataManager.playerData.baseEnergy * 0.6f)
    //            {
    //                elixirButton.onClick.RemoveAllListeners();
    //                Hide(EnergyElixir, true, 6);
    //            }
    //        }
    //    }
    //}
}
