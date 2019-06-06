using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObeliskTextureManager : MonoBehaviour
{
    public Texture[] Normal;
    public Texture[] Emissive;
    public Material mat;
    MeshRenderer o_Renderer;
    private int age;

    [Range(0f,4.1f)]
    public float mySlider;
    public TextMeshProUGUI[] timer;
    
    public double TimeP;


    // Start is called before the first frame update
    void Start()
    {
        TimeP = 1559683502000; //get from backend
        StartCoroutine(timerUpdate());
        o_Renderer = GetComponent<MeshRenderer>();
        o_Renderer.material.SetTexture("_EmissionMap", null);
        o_Renderer.material.SetTexture("_BumpMap", null);
        o_Renderer.material.DisableKeyword("_EMISSION");

    }
    IEnumerator timerUpdate()
    {
        while (true) 
            {
                var t = Utilities.GetTimeRemaining(TimeP);
                if (t == "00:00:00")
                {
                    break;    
                }
                foreach (var item in timer)
                    {
                        item.text = t;
                    }
                yield return new WaitForSeconds(1f);
                
            }
    }
    // Update is called once per frame
    void Update()
    {   
        age = (int)mySlider;
       // mySlider = (float)age;
        if (age != 0)
        {
            o_Renderer.material.SetTexture("_EmissionMap", Emissive[age]);
            o_Renderer.material.SetTexture("_BumpMap", Normal[age]);
             o_Renderer.material.EnableKeyword("_EMISSION");
        }
        else 
        {
            o_Renderer.material.SetTexture("_EmissionMap", null);
            o_Renderer.material.SetTexture("_BumpMap", null);
             o_Renderer.material.DisableKeyword("_EMISSION");
        }
        
    }
}
