using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovenManagerDebug : MonoBehaviour
{
    Rect[] rects = new Rect[]
    {
        //new Rect(0,50,200,50),
        new Rect(0,100,200,50),
        new Rect(0,150,200,50),
        new Rect(0,200,200,50),
        new Rect(0,250,200,50),
        new Rect(0,300,200,50),
    };

    private void OnGUI()
    {
        int i = 0;
        if (GUI.Button(rects[i++], "+ Create Random"))
        {
            CovenManagerAPI.CreateCoven("okt-test-" + UnityEngine.Random.Range(0, 100));
        }
        if (GUI.Button(rects[i++], "GetCovenData"))
        {
            //CovenManagerAPI.GetCovenData();
        }
        /*if (GUI.Button(rects[i++], "Ally"))
        {
            CovenManagerAPI.GetCovenData();
        }*/
    }



}