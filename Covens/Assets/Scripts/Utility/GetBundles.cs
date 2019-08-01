using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GetBundles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] files = Directory.GetFiles(@"E:\CovensUnity\Covens\Assets\UI\wardrobe\All Store Icons", "*.png", SearchOption.TopDirectoryOnly);
        foreach (var item in files)
        {
            var p = item.Split('\\');
            var l = p[p.Length - 1];
            var k = l.Substring(0, l.Length - 4);
            // Debug.Log(k);
            var t = AssetImporter.GetAtPath("Assets/UI/wardrobe/All Store Icons/" + k + ".png");
            t.SetAssetBundleNameAndVariant(k, "");
            t.SaveAndReimport();

            // Debug.Log(item);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
