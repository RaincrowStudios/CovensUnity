using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public string m_ClickSound = "ButtonClick";


    public virtual void OnClickButton()
    {
        SoundManager.PlayRandomPitch(m_ClickSound);
    }

}