using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recall : MonoBehaviour
{
    public void RecallHome()
    {
        double dist = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.coords, GetGPS.coordinates);

        if (dist < 0.1f)
        {
            PlayerManager.Instance.atLocationUIShow();
            return;
        }

        if (BanishManager.isBind || DeathState.IsDead)
            return;

        MapFlightTransition.Instance.RecallHome();
    }
}
