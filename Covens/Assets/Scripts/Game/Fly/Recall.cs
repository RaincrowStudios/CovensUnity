using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recall : MonoBehaviour
{
    public void RecallHome()
    {
        MapFlightTransition.Instance.RecallHome();
    }
}
