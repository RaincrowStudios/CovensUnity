using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UIUtilities : MonoBehaviour {

    [MenuItem("Test/UI FIt  %g")]
    public static void Fit()
    {
        
        GameObject pGO = Selection.activeObject as GameObject;
        if (pGO)
        {
            Debug.Log("aaaa");

            // set native size
            Image pImage = pGO.GetComponent<Image>();
            //pImage.SetNativeSize();


            RectTransform rt = (RectTransform)pGO.transform;
            RectTransform rtp = (RectTransform)pGO.transform.parent;

            Vector2 vPosition = rt.localPosition;
            Vector2 vSize = rt.sizeDelta;
            Vector2 vSizeParent = rtp.sizeDelta;

            Vector2 vAnchorMin = new Vector2(
                (vPosition.x - (vSize.x / 2) ) / vSizeParent.x,
                (vPosition.y - (vSize.y / 2)) / vSizeParent.y
                );


            Vector2 vAnchorMax = new Vector2(
                (vPosition.x + (vSize.x / 2)) / vSizeParent.x,
                (vPosition.y + (vSize.y / 2)) / vSizeParent.y
                );
            //Debug.Log("vAnchorMin(" + vAnchorMin + ") vAnchorMax("+ vAnchorMax + ")");
            Debug.Log("vAnchorMin(" + vAnchorMin.x + ", " + vAnchorMin.y + ")");// vAnchorMax(" + vAnchorMax + ")");
            rt.anchorMin = vAnchorMin;
            //rt.anchorMax = vAnchorMax;




        }
    }


}
