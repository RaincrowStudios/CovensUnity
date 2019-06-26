using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWebSpellcastingPoP : MonoBehaviour
{
    public SpriteRenderer[] path1;
    public SpriteRenderer[] path2;
    public GameObject path1Cast;
    public GameObject path1Hit;
    public GameObject path2Cast;
    public GameObject path2Hit;
    public int i;
    public int c;

    public void CastPath1()
    {
        path1Cast.SetActive(true);
        i = 0;
        c = 0;
        LeanTween.value(0f, 1f, 0.5f).setOnComplete(() =>
          {
              it();
          });
    }
    public void it()
    {
        LeanTween.color(path1[i].gameObject, Utilities.Orange, 0.5f);
        //path1[i].color = Utilities.Orange;
        LeanTween.value(0f, 1f, 0.25f).setOnComplete(() =>
          {
              i = i + 1;
              it();
          });
        if (i == path1.Length - 1)
        {
            LeanTween.value(0f, 1f, 1f).setOnComplete(() =>
            {
                path1Hit.SetActive(true);
                path1Cast.SetActive(false);
            });
            Invoke("EndCastPath1", 1f);
        }
    }
    public void EndCastPath1()
    {
        LeanTween.color(path1[c].gameObject, new Color(1f, 1f, 1f, 1f), 0.5f);
        // path1[c].color = new Color(1f, 1f, 1f, 1f);
        LeanTween.value(0f, 1f, 0.25f).setOnComplete(() =>
          {
              c = c + 1;
              EndCastPath1();
          });
        if (c == path1.Length - 1)
        {
            path1Hit.SetActive(false);
        }
    }



}
