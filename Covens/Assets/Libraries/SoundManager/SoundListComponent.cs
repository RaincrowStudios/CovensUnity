using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundListComponent : MonoBehaviour
{

    [HideInInspector]
    public List<SoundManager.SoundRef> m_SoundName;

    private void Start()
    {
        foreach (SoundManager.SoundRef pSound in m_SoundName)
        {
            SoundManager.AddCachedAudio(pSound);
        }
    }











    public AudioClip FindAudio(string sSoundName)
    {
        foreach (SoundManager.SoundRef pSound in m_SoundName)
        {
            if (pSound.m_Name == sSoundName)
                return pSound.m_AudioClip;
        }
        return null;
    }

    /// <summary>
    /// adds a sound
    /// </summary>
    /// <param name="pClip"></param>
    public void AddSound(AudioClip pClip)
    {
        if (m_SoundName == null)
        {
            m_SoundName = new List<SoundManager.SoundRef>();
        }
        else
        {
            foreach (SoundManager.SoundRef pSound in m_SoundName)
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
        m_SoundName.Add(new SoundManager.SoundRef(pClip.name, pClip));
    }

}
