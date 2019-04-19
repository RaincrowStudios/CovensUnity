using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class SoundManagerOneShot : MonoBehaviour
{
    public static SoundManagerOneShot Instance { get; set; }

    public AudioClip whisper;
    public AudioClip itemAdded;
    public float itemAddedSound;
    public AudioClip Error;
    public float errorSound;
    public AudioClip buttonTap;
    public AudioClip collectSound;
    public float buttonTapSound;


    public AudioClip LevelChange;
    public AudioClip[] WhiteAlign;
    public AudioClip[] ShadowAlign;
    public float statsChangeSound;

    public AudioClip[] menuSounds;

    public AudioClip Spirit;

    public AudioClip[] critSounds;

    public AudioClip[] AllWhisperSounds;

    public AudioClip loginButtonPress;

    public AudioClip BigDrum;

    public AudioClip[] darknessSounds;

    public AudioClip summonRiser;
    public AudioClip landingSound;

    public AudioClip barghestSound;
    public AudioClip fowlerSound;
    public AudioClip spiritFoundSound;
    public AudioClip witchImmune;
    public AudioClip brigidLaugh;

    public AudioClip welcomeWitch;

    public AudioClip claimRewards;
    public AudioClip flySoundStart;

    public AudioClip[] soundsBG;
	public AudioClip [] spellbookSpiritSelected;
    AudioSource ASBG;

    AudioSource AS;
    void Awake()
    {
        Instance = this;
        AS = GetComponent<AudioSource>();
    }
    void Start()
    {
        ASBG = WebSocketClient.Instance.GetComponent<AudioSource>();
    }
    public void SetBGTrack(int i)
    {
        ASBG.clip = soundsBG[Mathf.Clamp(i, 0, soundsBG.Length - 1)];
        ASBG.Play();
    }
    public void PlayEnergyCollect(float s = 1)
    {
        AS.PlayOneShot(collectSound, s);
    }

    public void PlayWhisper(float s = 1)
    {
        AS.PlayOneShot(whisper, s);
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
		playSound (spellbookSpiritSelected [Random.Range (0, spellbookSpiritSelected.Length)], 1);
	}

    public void SummonRiser()
    {
        playSound(summonRiser, .1f);
    }

    public void LandingSound(float s = .5f)
    {
        playSound(landingSound, s);
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
        playSound(witchImmune, .5f);

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

