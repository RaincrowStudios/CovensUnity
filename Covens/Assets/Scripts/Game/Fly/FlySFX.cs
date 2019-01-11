using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class FlySFX : MonoBehaviour
{

    public static FlySFX Instance { get; set; }

    public AudioMixerSnapshot fullSound;
    public AudioMixerSnapshot FlightSound;
    public AudioMixerSnapshot[] flightVarience;
    bool isFlying = false;
    public GameObject Flight;
    public AudioSource bgMusicSource;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        fullSound.TransitionTo(4f);
    }

    // Update is called once per frame

    public void fly()
    {
        SoundManagerOneShot.Instance.PlayFlightStart();
        Invoke("flyhelper", .1f);
    }

    void flyhelper()
    {
        Flight.SetActive(true);
        isFlying = true;
        FlightSound.TransitionTo(.3f);
        Invoke("randomSounds", 2f);
    }

    void randomSounds()
    {
        if (!isFlying)
            return;

        flightVarience[Random.Range(0, flightVarience.Length)].TransitionTo(Random.Range(1.3f, 3.4f));
        Invoke("randomSounds", Random.Range(3, 7));
    }

    public void EndFly()
    {
        float lastTime = bgMusicSource.time;
        fullSound.TransitionTo(2);
        isFlying = false;
        Flight.SetActive(false);
        bgMusicSource.time = lastTime;
    }
}
