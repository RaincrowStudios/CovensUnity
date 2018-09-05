using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SoundKit))]
public class SoundManager : MonoBehaviour
{

    #region aux struct
    [System.Serializable]
    public struct SoundRef
    {
        public string m_Name;
        public AudioClip m_AudioClip;
        public SoundRef(string sName, AudioClip pClip)
        {
            m_Name = sName;
            m_AudioClip = pClip;
        }
    }

    [HideInInspector]
    public List<SoundRef> m_SoundName;

    #endregion

    static private bool m_SoundEnabled = true;
    static SoundManager m_pInstance;

    [SerializeField] protected string StartBG;
    [SerializeField] protected AudioSource[] m_SoundSourceList;

    private Dictionary<string, float> m_LastTimePlay = new Dictionary<string, float>();
    private Dictionary<string, AudioClip> m_AudioCache = null;

    #region gets/sets


    public static bool SoundEnabled
    {
        get
        {
            return m_SoundEnabled;
        }
        set
        {
            m_SoundEnabled = value;
        }
    }

    public static bool Enabled
    {
        get { return (m_pInstance != null && SoundEnabled); }
    }


    public static float MusicVolume
    {
        get { return PlayerPrefs.GetFloat("SoundList.MusicVolume", .5f); }
        set { PlayerPrefs.SetFloat("SoundList.MusicVolume", value); }
    }
    public static float SoundVolume
    {
        get { return PlayerPrefs.GetFloat("SoundList.SoundVolume", 1); }
        set { PlayerPrefs.SetFloat("SoundList.SoundVolume", value); }
    }



    private Dictionary<string, AudioClip> CachedAudio
    {
        get
        {
            if (m_AudioCache == null)
            {
                m_AudioCache = new Dictionary<string, AudioClip>();
                foreach (SoundRef pSound in m_SoundName)
                {
                    m_AudioCache.Add(pSound.m_Name, pSound.m_AudioClip);
                }
            }
            return m_AudioCache;
        }
    }

    #endregion


    void Awake()
    {
        m_pInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        PlayBgMusic();
    }

    /// <summary>
    /// adds a sound
    /// </summary>
    /// <param name="pClip"></param>
    public void AddSound(AudioClip pClip)
    {
        if (m_SoundName == null)
        {
            m_SoundName = new List<SoundRef>();
        }
        else
        {
            foreach (SoundRef pSound in m_SoundName)
            {
                if (pSound.m_AudioClip == pClip)
                {
                    Debug.LogError("Sound already exists: " + pClip.name);
                    return;
                }
                if (pClip.name == pSound.m_Name)
                {
                    Debug.LogError("Name already exists: " + pClip.name);
                    return;
                }
            }
        }
        // add it
        m_SoundName.Add(new SoundRef(pClip.name, pClip));
    }


    #region static methods

    /// <summary>
    /// adds an audio to cached audio
    /// </summary>
    /// <param name="pSound"></param>
    public static void AddCachedAudio(SoundRef pSound)
    {
        if (m_pInstance)
        {
            if (m_pInstance.CachedAudio.ContainsKey(pSound.m_Name))
            {
                Debug.LogError("Already has[" + pSound.m_Name + "]");
                return;
            }
            m_pInstance.CachedAudio.Add(pSound.m_Name, pSound.m_AudioClip);
        }
    }

    public static SoundKit.SKSound Play(string sSoundName, float iPitch = 1, float fDelay = 0)
    {
        if (!Enabled || string.IsNullOrEmpty(sSoundName))
            return null;
        if (!m_pInstance.m_LastTimePlay.ContainsKey(sSoundName))
            m_pInstance.m_LastTimePlay.Add(sSoundName, 0);
        float fLastTime = 0;
        m_pInstance.m_LastTimePlay.TryGetValue(sSoundName, out fLastTime);
        // do not wais my time with accumulated sounds
        if (Time.time - fLastTime < .1f)
            return null;
        m_pInstance.m_LastTimePlay[sSoundName] = Time.time;
        AudioClip pSound = GetSound(sSoundName);
        if (pSound)
        {
            if (fDelay > 0)
                SoundKit.instance.playSoundDelay(pSound, 1, iPitch, 0, fDelay);
            else
                return SoundKit.instance.playSound(pSound, 1, iPitch, 0);
        }
        return null;
    }

    public static void PlayRandomPitch(string sSoundName)
    {
        Play(sSoundName, Random.Range(0.85f, 1.15f));
    }

    public static void PlayOneShot(string sSoundName)
    {
        if (!Enabled)
            return;

        AudioClip pSound = GetSound(sSoundName);
        if (pSound)
        {
            SoundKit.instance.playOneShot(pSound);
        }
    }

    public static AudioClip GetSound(string sSoundName)
    {
        if (!Enabled)
            return null;

        AudioClip pAudio = null;
        m_pInstance.CachedAudio.TryGetValue(sSoundName, out pAudio);
        if (pAudio == null)
            Debug.LogWarning("Sound Not Found[" + sSoundName + "]");
        return pAudio;
    }
    public AudioClip FindAudio(string sSoundName)
    {
        foreach (SoundRef pSound in m_SoundName)
        {
            if (pSound.m_Name == sSoundName)
                return pSound.m_AudioClip;
        }
        return null;
    }

    public static void Play3D(string sSoundName)
    {
        if (!Enabled)
            return;

        AudioSource pSound = GetSoundSource(sSoundName);
        if (pSound)
        {
            pSound.Play();
        }
    }

    public static void Play3DAt(string sSoundName, Vector3 pPosition)
    {
        if (!Enabled)
            return;

        AudioSource pSound = GetSoundSource(sSoundName);
        if (pSound)
        {
            pSound.transform.position = pPosition;
            pSound.Play();
        }
    }

    public static AudioSource GetSoundSource(string sSoundName)
    {
        if (m_pInstance == null) return null;
        foreach (AudioSource pSound in m_pInstance.m_SoundSourceList)
        {
            if (pSound != null && pSound.name == sSoundName)
                return pSound;
        }
        Debug.LogWarning("Sound Not Found[" + sSoundName + "]");
        return null;
    }


    public static void PlayBgMusic()
    {
        if (m_pInstance.StartBG != null)
            PlayBgMusic(m_pInstance.StartBG);
    }
    public static void PlayBgMusic(string sMusicName)
    {
        AudioClip pAudio = GetSound(sMusicName);
        if (pAudio)
        {
            SoundKit.instance.playBackgroundMusic(pAudio, MusicVolume, true);
        }
    }
    public static void StopBgMusic(float fFade)
    {
        if (fFade > 0)
        {
            if (SoundKit.instance.backgroundSound != null)
                SoundKit.instance.backgroundSound.fadeOutAndStop(fFade);
        }
        else
        {
            if (SoundKit.instance.backgroundSound != null)
                SoundKit.instance.backgroundSound.stop();
        }
    }

    #endregion


}