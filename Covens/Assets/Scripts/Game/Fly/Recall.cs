using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recall : MonoBehaviour
{
    Vector2 pos, oldPos;
    float t;
    public float speed = 1;
    bool move;



    public void RecallHome()
    {
        MarkerManagerAPI.GetMarkers(true, true, () =>
        {

        }, 
        true,
        false);
    }
}
