using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetMaterial
{
    public static void SetMaterials(MeshRenderer[] renderers, Material[] mats)
    {
        if (mats.Length == 0)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {            
            renderers[i].material = mats[Random.Range(0, mats.Length)];
        }
    }
}
