using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Maps;
public class DeathState : MonoBehaviour
{

    public static DeathState Instance { get; set; }

    public Transform[] ScaleDownObjects;
    public Transform[] ScaleUpObjects;
    public CanvasGroup[] FadeButtons;
    public GameObject[] DisableItems;
    public Camera UICamera;
    public Camera MainCamera;
    public float speed = 1;
    //	public GameObject Particles;
    public GameObject DeathContainer;
    public GameObject FlightGlowFX;
    bool isDead = false;
    public GameObject DeathPersist;
    public GameObject flyDead;
    // public GameObject mapDarkBox;
    public Button[] turnOffInteraction;
    public static bool IsDead { get; private set; }
    IMaps map;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        if (isDead)
        {
            StartCoroutine(BeginDeathState());
            isDead = false;
        }
    }

    public void ShowDeath()
    {
        IsDead = true;
                
        foreach (var se in PlayerDataManager.playerData.effects)
            se.Expire();
        PlayerDataManager.playerData.effects.Clear();

        flyDead.SetActive(true);
        foreach (var item in turnOffInteraction)
        {
            if (item != null)
                item.interactable = false;
        }
        SoundManagerOneShot.Instance.PlaySpellFX();
        DeathPersist.SetActive(true);


        PlayerManager.Instance.CancelFlight();
        FlightGlowFX.SetActive(false);
        DeathContainer.SetActive(true);
        if (gameObject.activeInHierarchy)
            StartCoroutine(BeginDeathState());
        else
            isDead = true;
        Invoke("HideDeath", 3f);
        // if (!PlayerDataManager.tutorial)
        //     PlayerManagerUI.Instance.ShowElixirVulnerable(true);
    }

    public void Revived()
    {
        if (!IsDead)
            return;

        HideDeath();
        IsDead = false;
        
        flyDead.SetActive(false);

        foreach (var item in turnOffInteraction)
        {
            if (item != null)
                item.interactable = true;
        }
        DeathPersist.SetActive(false);
        PlayerManagerUI.Instance.Revived();
    }

    void HideDeath()
    {
        //		Particles.SetActive (false);
        //FlightGlowFX.SetActive(true);
        DeathContainer.GetComponent<Fade>().FadeOutHelper();
        StartCoroutine(EndDeathState());
        //MapFlightTransition.Instance.RecallHome();
    }

    public void FTFDeathState(bool show)
    {
        if (show)
        {
            flyDead.SetActive(true);
            DeathContainer.SetActive(true);
            StartCoroutine(BeginDeathState());
        }
        else
        {
            flyDead.SetActive(false);
            DeathContainer.GetComponent<Fade>().FadeOutHelper();
            StartCoroutine(EndDeathState());
        }
    }

    IEnumerator EndDeathState()
    {
        float t = 1;
        while (t >= 0)
        {
            t -= Time.deltaTime * speed;
            ManageState(t);
            yield return null;
        }
    }

    IEnumerator BeginDeathState()
    {
        foreach (var item in DisableItems)
        {
            if (item == null)
                continue;
            item.SetActive(false);
        }
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * speed;
            ManageState(t);
            yield return null;
        }
    }

    void ManageState(float t)
    {

        //mainCamSettings.basic.saturation = Mathf.SmoothStep(1, 0, t);

        //mainCamSettings.basic.contrast = Mathf.SmoothStep(1, 2, t);


        foreach (var item in FadeButtons)
        {
            item.alpha = Mathf.SmoothStep(1, .4f, t);
        }


        foreach (var item in ScaleDownObjects)
        {
            item.localScale = Vector3.one * Mathf.SmoothStep(1, 0, t);
        }

        foreach (var item in ScaleUpObjects)
        {
            item.localScale = Vector3.one * Mathf.SmoothStep(0, 1, t);
        }
    }
    public void FlightGlowOn()
    {
        FlightGlowFX.SetActive(true);
    }
    public void FlightGlowOff()
    {
        FlightGlowFX.SetActive(false);
    }
}
