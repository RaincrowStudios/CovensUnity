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

        if (map == null) map = MapsAPI.Instance;
        map.SetPosition(GetGPS.longitude, GetGPS.latitude);

        PlayerManager.marker.gameObject.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
        PlayerManager.marker.SetCharacterAlpha(.56f);

        PlayerDataManager.playerData.effects.Clear();

        if (BanishManager.isBind)
        {
            BanishManager.Instance.Unbind();
        }
        if (BanishManager.isSilenced)
        {
            BanishManager.Instance.UnSilenced();
        }
        flyDead.SetActive(true);
        foreach (var item in turnOffInteraction)
        {
            item.interactable = false;
        }
        SoundManagerOneShot.Instance.PlaySpellFX();
        DeathPersist.SetActive(true);
        //        mapDarkBox.SetActive(true);


        //if (MapSelection.currentView == CurrentView.MapView)
        {
            //if (!PlayerManager.Instance.fly)
            //    PlayerManager.Instance.Fly();
            FlightGlowFX.SetActive(false);
            //		Particles.SetActive (true);
            DeathContainer.SetActive(true);
            if (gameObject.activeInHierarchy)
                StartCoroutine(BeginDeathState());
            else
                isDead = true;
            Utilities.allowMapControl(false);
            Invoke("HideDeath", 3f);
            // if (!PlayerDataManager.tutorial)
            //     PlayerManagerUI.Instance.ShowElixirVulnerable(true);
            if (SummoningManager.isOpen)
            {
                SummoningController.Instance.Close();
            }
        }
    }

    public void Revived()
    {
        if (!IsDead)
            return;

        IsDead = false;
        PlayerManager.marker.gameObject.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
        PlayerManager.marker.SetCharacterAlpha(1);

        MarkerManagerAPI.GetMarkers(true, false, () =>
        {
            flyDead.SetActive(false);
            //        mapDarkBox.SetActive(false);

            foreach (var item in turnOffInteraction)
            {
                item.interactable = true;
            }
            DeathPersist.SetActive(false);
            PlayerManagerUI.Instance.Revived();
        });
    }

    void HideDeath()
    {
        //		Particles.SetActive (false);
        FlightGlowFX.SetActive(true);
        DeathContainer.GetComponent<Fade>().FadeOutHelper();
        StartCoroutine(EndDeathState());
        Utilities.allowMapControl(true);
        MapFlightTransition.Instance.RecallHome();
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
