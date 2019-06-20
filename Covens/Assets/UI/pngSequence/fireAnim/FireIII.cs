using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireIII : MonoBehaviour
{
    public Sprite[] Fires;
    public int i;
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        image.overrideSprite = Fires[i];
        FlameOn();
    }

    // Update is called once per frame
    private void FlameOn()
    {
        
        
        if (i == 10)
            {
                i=0;
            }
        else {
            i = i+1;
        }
        
        image.overrideSprite = Fires[i];

        LeanTween.value(0f,1f,0.05f).setOnComplete(()=> {
            FlameOn();
        });

    }
}
