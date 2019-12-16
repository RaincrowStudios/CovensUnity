using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class SoundManagerOneShot : MonoBehaviour
{
    private static SoundManagerOneShot m_Instance;
    public static SoundManagerOneShot Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = FindObjectOfType<SoundManagerOneShot>();
            return m_Instance;
        }
    }
    public AudioClip returnToPhysical;

    public AudioClip witchImmune;
    public AudioClip spiritForm;
    public AudioClip witchDead;
    public AudioClip summonFamiliar;

    public AudioClip whisper;
    public AudioClip itemAdded;
    public float itemAddedSound;
    public AudioClip Error;
    public float errorSound;
    public AudioClip buttonTap;
    public AudioClip collectSound;
    public AudioClip LandFX;

    public float buttonTapSound;


    public AudioClip LevelChange;
    public AudioClip[] WhiteAlign;
    public AudioClip[] ShadowAlign;
    public float statsChangeSound;

    public AudioClip[] menuSounds;

    public AudioClip Spirit;

    public AudioClip[] critSounds;

    public AudioClip[] AllWhisperSounds;

    public AudioClip EnYaSa;
    public AudioClip flightButtonPress;
    public AudioClip wooshShort;

    public AudioClip loginButtonPress;

    public AudioClip BigDrum;

    public AudioClip AHSAWhisper;

    public AudioClip[] darknessSounds;

    public AudioClip summonRiser;
    public AudioClip landingSound;

    public AudioClip barghestSound;
    public AudioClip fowlerSound;
    public AudioClip spiritFoundSound;
    public AudioClip brigidLaugh;
    public AudioClip makeYourOffering;

    public AudioClip welcomeWitch;

    public AudioClip claimRewards;
    public AudioClip flySoundStart;

    public AudioClip[] soundsBG;
    public int BGnum;
    public AudioClip[] spellbookSpiritSelected;
    public AudioClip Cloaking;

    public AudioClip PostEffect2;
    AudioSource ASBG;

    AudioSource AS;

    void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        m_Instance = this;
        AS = GetComponent<AudioSource>();
    }

    void Start()
    {
        //ASBG = SocketClient.Instance.GetComponent<AudioSource>();
        ASBG = this.GetComponent<AudioSource>();

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        returnToPhysical.UnloadAudioData();

        witchImmune.UnloadAudioData();
        spiritForm.UnloadAudioData();
        witchDead.UnloadAudioData();
        summonFamiliar.UnloadAudioData();

        whisper.UnloadAudioData();
        itemAdded.UnloadAudioData();
        Error.UnloadAudioData();
        buttonTap.UnloadAudioData();
        collectSound.UnloadAudioData();
        LandFX.UnloadAudioData();

        LevelChange.UnloadAudioData();

        EnYaSa.UnloadAudioData();
        flightButtonPress.UnloadAudioData();
        wooshShort.UnloadAudioData();

        loginButtonPress.UnloadAudioData();

        BigDrum.UnloadAudioData();

        AHSAWhisper.UnloadAudioData();

        summonRiser.UnloadAudioData();
        landingSound.UnloadAudioData();

        barghestSound.UnloadAudioData();
        fowlerSound.UnloadAudioData();
        spiritFoundSound.UnloadAudioData();
        brigidLaugh.UnloadAudioData();
        makeYourOffering.UnloadAudioData();

        welcomeWitch.UnloadAudioData();

        claimRewards.UnloadAudioData();
        flySoundStart.UnloadAudioData();

        Cloaking.UnloadAudioData();

        PostEffect2.UnloadAudioData();

        //foreach (var clip in soundsBG)
        //    clip.UnloadAudioData();
        foreach (var clip in spellbookSpiritSelected)
            clip?.UnloadAudioData();
        foreach (var clip in darknessSounds)
            clip?.UnloadAudioData();
        foreach (var clip in WhiteAlign)
            clip?.UnloadAudioData();
        foreach (var clip in ShadowAlign)
            clip?.UnloadAudioData();
        foreach (var clip in menuSounds)
            clip?.UnloadAudioData();
        foreach (var clip in critSounds)
            clip?.UnloadAudioData();
        foreach (var clip in AllWhisperSounds)
            clip?.UnloadAudioData();
    }

    public void SetBGTrack(int i)
    {
        ASBG.clip = soundsBG[Mathf.Clamp(i, 0, soundsBG.Length - 1)];
        ASBG.Play();
        ASBG.volume = 0.7f;
        BGnum = i;
    }

    public void FadeOutBGTrack()
    {
        LeanTween.value(0.7f, 0.05f, 1f).setOnUpdate((float v) =>
        {
            ASBG.volume = v;
        });
    }
    public void FadeInBGTrack()
    {
        LeanTween.value(0.05f, 0.7f, 1f).setOnUpdate((float v) =>
        {
            ASBG.volume = v;
        });
    }
    public void PlayCloakingSFX(bool start)
    {
        AS.clip = Cloaking;
        if (start)
        {
            AS.Play();
        }
        else
        {
            LeanTween.value(1f, 0f, 1f).setOnUpdate((float v) =>
              {
                  AS.volume = v;
              }).setOnComplete(() =>
              {
                  AS.Stop();
              });
        }
    }
    public void PlayEnergyCollect(float s = 1)
    {
        AS.PlayOneShot(collectSound, s);
    }
    public void PlayMakeYourOffering(float s = 1)
    {
        AS.PlayOneShot(makeYourOffering, s);
    }
    public void PlayPostEffect2(float s = 1)
    {
        AS.PlayOneShot(PostEffect2, s);
    }
    public void PlaySummonFamiliarRead(float s = 1)
    {
        AS.PlayOneShot(summonFamiliar, s);
    }
    public void PlayEnYaSa(float s = 1)
    {
        AS.PlayOneShot(EnYaSa, s);
    }
    public void PlaySpiritForm(float s = 1)
    {
        AS.PlayOneShot(spiritForm, s);
    }
    public void PlayAHSAWhisper(float s = 1)
    {
        AS.PlayOneShot(AHSAWhisper, s);
    }
    public void PlayWhisper(float s = 1)
    {
        AS.PlayOneShot(whisper, s);
    }
    public void PlayLandFX(float s = 1)
    {
        AS.PlayOneShot(LandFX, s);
    }
    public void PlayItemAdded()
    {
        AS.volume = itemAddedSound;

        AS.PlayOneShot(itemAdded);
    }

    public void PlayError()
    {
        AS.PlayOneShot(Error, errorSound);
    }

    public void PlayFlightStart()
    {
        AS.PlayOneShot(flySoundStart, 1);
    }

    public void PlayReturnPhysical()
    {
        AS.PlayOneShot(returnToPhysical, 1);
    }

    public void PlayButtonTap()
    {
        AS.PlayOneShot(buttonTap, .3f);
    }

    public void PlayLevel()
    {

        AS.PlayOneShot(LevelChange, 1);
    }

    public void PlayShadow()
    {
        AS.PlayOneShot(ShadowAlign[Random.Range(0, ShadowAlign.Length)], statsChangeSound);
    }

    public void PlayWhite()
    {

        AS.PlayOneShot(ShadowAlign[Random.Range(0, ShadowAlign.Length)], statsChangeSound);
    }

    public void SpiritSummon()
    {
        AS.volume = statsChangeSound;
        AS.PlayOneShot(Spirit);
    }

    public void MenuSound()
    {
        playSound(menuSounds[Random.Range(0, menuSounds.Length)]);
    }

    void playSound(AudioClip clip, float volume = 1)
    {
        AS.PlayOneShot(clip, volume);
    }

    public void PlayCrit()
    {
        Invoke("critHelper", 1f);
    }

    void critHelper()
    {
        playSound(critSounds[Random.Range(0, critSounds.Length)]);

    }

    public void PlayWhisperFX(float volume = 1)
    {
        playSound(AllWhisperSounds[Random.Range(0, AllWhisperSounds.Length)], volume);
    }

    public void PlayLoginButton()
    {
        playSound(loginButtonPress, 1);
        PlayButtonTap();
    }

    public void PlayWitchDead()
    {
        playSound(witchDead, 1);
    }
    public void PlayWooshShort()
    {
        playSound(wooshShort, 1);
    }
    public void IngredientAdded()
    {
        playSound(BigDrum, 1);
    }

    public void PlaySpellFX()
    {
        playSound(darknessSounds[Random.Range(0, darknessSounds.Length)], .4f);
    }
    public void PlaySpiritSelectedSpellbook()
    {
        playSound(spellbookSpiritSelected[Random.Range(0, spellbookSpiritSelected.Length)], 1);
    }

    public void SummonRiser()
    {
        playSound(summonRiser, .1f);
    }

    public void LandingSound(float s = .5f)
    {
        playSound(landingSound, s);
    }
    public void FlightButtonPress()
    {
        playSound(flightButtonPress, 1);
    }
    public void PlayBarghest()
    {
        playSound(barghestSound, .55f);
    }

    public void PlayFowler()
    {
        playSound(fowlerSound);
    }

    public void SpiritDiscovered()
    {
        playSound(spiritFoundSound, .5f);
    }

    public void WitchImmune()
    {
        Invoke("playImmuneDelayed", 2f);
    }

    void playImmuneDelayed()
    {
        playSound(witchImmune, 1f);

    }

    public void PlayBrigidLaugh()
    {
        playSound(brigidLaugh, .5f);
    }

    public void PlayWelcome()
    {
        playSound(welcomeWitch);
    }

    public void PlayReward()
    {
        playSound(claimRewards);
    }
}

