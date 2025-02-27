﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConditionButtonData : MonoBehaviour
{
    public Text counterText;
    public Text desc;
    public GameObject counterObject;
    public Image spell;
    bool isClicked = false;
    int increment = 0;
    public GameObject fx;
    public GameObject fxTrigger;
    public GameObject onSelect;
    public Transform descObject;
    void Update()
    {
        if (isClicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                close();
            }
        }
    }

    public void Setup(Conditions data)
    {
        try
        {
            increment = 1;
            counterText.text = increment.ToString();
            DownloadedAssets.GetSprite(data.baseSpell, spell);

            desc.text = DownloadedAssets.conditionsDictData[data.id].conditionDescription;
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    public void Add(Conditions data)
    {
        increment++;
        counterText.text = increment.ToString();
    }

    public void Remove(Conditions data)
    {
        //		print ("Value Removed!");
        increment--;
        if (increment == 0)
        {
            ConditionsManager.Instance.conditionButtonDict.Remove(data.id);
            try
            {
                Destroy(this.gameObject);
            }
            catch
            {
            }
            //			print ("Condition Value Destroyed!");

        }
        else
        {
            if (counterText != null)
                counterText.text = increment.ToString();
            //			print ("Condition Value Removed!");
        }
    }

    public void Animate()
    {
        if (!isClicked)
        {
            onSelect.SetActive(true);
            StartCoroutine(scaleUp());
            isClicked = true;
        }
    }

    public void ConditionTrigger()
    {
        fxTrigger.SetActive(true);
    }

    public void ConditionChange()
    {
        fx.SetActive(true);
    }


    void close()
    {
        onSelect.SetActive(false);
        StartCoroutine(scaleDown());
    }

    IEnumerator scaleUp()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 3.5f;
            descObject.localScale = Vector3.one * Mathf.SmoothStep(0, 1, t);
            yield return 0;
        }
    }

    IEnumerator scaleDown()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 5.5f;
            descObject.localScale = Vector3.one * Mathf.SmoothStep(1, 0, t);
            yield return 0;
        }
        isClicked = false;
    }
}
