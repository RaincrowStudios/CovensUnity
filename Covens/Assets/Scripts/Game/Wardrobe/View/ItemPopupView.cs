using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopupView : UIBaseAnimated
{
    public Image[] m_pImages;
    public Text[] m_pAmount;
    public Text[] m_pNames;


    // Use this for initialization
    void OnEnable()
    {

        for (int i = 0; i < m_pImages.Length; i++)
        {
            m_pImages[i].sprite = SpriteResources.GetSprite("Aptitude_" + (i + 1).ToString());
            m_pNames[i].text = "Aptitude_" + (i + 1).ToString();
            m_pAmount[i].text = (Random.Range(0,6)).ToString();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
