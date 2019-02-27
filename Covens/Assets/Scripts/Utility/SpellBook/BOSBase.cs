using System;
using System.Collections.Generic;
using UnityEngine;

public class BOSBase : MonoBehaviour
{
    // protected GameObject previousScreen;
    protected GameObject CreateScreen(GameObject prefab)
    {
        var g = Utilities.InstantiateUI(prefab, transform);
        return g;
    }

    protected void DestroyPrevious(GameObject g)
    {
        if (g != null)
            Destroy(g);
    }


}