using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// stores the sprite sources to be used dynamically
/// </summary>
public class SpriteResources : MonoBehaviour
{

    #region classes

    /// <summary>
    /// stores the sprite source
    /// </summary>
    [System.Serializable]
    public struct SpriteSource
    {
        public string Name;
        public Sprite Source;
        public SpriteSource(string sName, Sprite pSource)
        {
            Name = sName;
            Source = pSource;
        }
    }

    #endregion


    #region fields

    static private SpriteResources m_pInstance;

    [HideInInspector]
    [SerializeField]
    private List<SpriteSource> SpriteResoruces;
    private Dictionary<string, SpriteSource> m_pSpriteResources;

    #endregion


    #region static Methods

    private static SpriteResources Instance
    {
        get
        {
            if (m_pInstance == null)
                m_pInstance = FindObjectOfType<SpriteResources>();
            return m_pInstance;
        }
    }

    public static Sprite GetSprite(string sName)
    {
        if(Instance == null)
        {
			#if OKT_DEBUG
            Debug.LogError("[SpriteResource] You should add the SpriteResource component in any component and fill it to work!");
			#endif
            return null;
        }
        return Instance.GetSpriteByName(sName);
    }

    #endregion


    #region methods

    /// <summary>
    /// faster access
    /// </summary>
    private Dictionary<string, SpriteSource> ResourcesDictionary
    {
        get
        {
            if(m_pSpriteResources == null)
            {
                m_pSpriteResources = new Dictionary<string, SpriteSource>();
                for (int i = 0; i < SpriteResoruces.Count; i++)
                {
                    m_pSpriteResources.Add(SpriteResoruces[i].Name, SpriteResoruces[i]);
                }
            }
            return m_pSpriteResources;
        }
    }

    /// <summary>
    /// gets the sprite based on it name
    /// </summary>
    /// <param name="sName"></param>
    /// <returns></returns>
    private Sprite GetSpriteByName(string sName)
    {
        // just because when playing we need some faster access
        if (Application.isPlaying)
        {
            if (ResourcesDictionary.ContainsKey(sName))
            {
                return ResourcesDictionary[sName].Source;
            }
        }
        // and in editor we should not care about performance and we want to change it at all time
        else
        {
            foreach (SpriteSource sp in SpriteResoruces)
            {
                if (sp.Name == sName) 
                    return sp.Source;
            }
        }
		#if OKT_DEBUG
		Debug.LogError("[SpriteResource] There is no string with \""+sName+"\" name");
		#endif
        return null;
    }

    #endregion

	/// <summary>
	/// preguiça. eval SpriteResources.Instance.RenameReferences()
	/// </summary>
	protected void RenameReferences() {
		for (int i = 0; i < this.SpriteResoruces.Count; i++) {
			Sprite pSprite = this.SpriteResoruces[i].Source;
			this.SpriteResoruces[i] = new SpriteSource(pSprite.name, pSprite);
		}
	}
}