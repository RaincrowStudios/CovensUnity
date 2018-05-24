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

    public string CovenName = "coven-okthugo-015";
    public string CovenTitle = "coven-newtitle";
    public string CovenNameInvite = "coven-okthugo-016";

    private void OnGUI()
    {
        int i = 0;


        if (GUI.Button(rects[i++], "+ Create " + CovenName))
        {
            CovenManagerAPI.CreateCoven(CovenName, null, null);
        }
        if (GUI.Button(rects[i++], "Display"))
        {
            CovenManagerAPI.CovenDisplay(CovenName, null, null);
        }
        if (GUI.Button(rects[i++], "RequestInvite " + CovenNameInvite))
        {
            CovenManagerAPI.CovenRequest(CovenName, null, null);
        }



        if (GUI.Button(rects[i++], "Title " + CovenTitle))
        {
            //CovenManagerAPI.Title(CovenTitle, null, null);
        }




        /*if (GUI.Button(rects[i++], "Ally"))
        {
            CovenManagerAPI.GetCovenData();
        }*/
    }



}