using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSpriteUtil : MonoBehaviour, IWitchAvatarFactory
{
    private class MarkerSpriteCollection
    {
        private Dictionary<WitchMarker, Action<Sprite>> m_Dict;
        private List<WitchMarker> m_List;

        public int Count => m_List.Count;

        public MarkerSpriteCollection()
        {
            m_Dict = new Dictionary<WitchMarker, Action<Sprite>>();
            m_List = new List<WitchMarker>();
        }

        ~MarkerSpriteCollection()
        {
            m_Dict.Clear();
            m_List.Clear();
            m_Dict = null;
            m_List = null;
        }

        public void Add(WitchMarker marker, Action<Sprite> callback)
        {
            if (!m_Dict.ContainsKey(marker))
            {
                m_List.Add(marker);
            }
            m_Dict[marker] = callback;
        }

        public bool Remove(WitchMarker marker)
        {
            if (m_Dict.ContainsKey(marker))
            {
                for(int i = 0; i < m_List.Count; i++)
                {
                    if (m_List[i] == marker)
                    {
                        m_List.RemoveAt(i);
                        break;
                    }
                }
                m_Dict.Remove(marker);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            m_Dict.Clear();
            m_List.Clear();
        }

        public bool GetFirst(out WitchMarker marker, out Action<Sprite> callback)
        {
            if (m_List.Count > 0)
            {
                marker = m_List[0];
                callback = m_Dict[marker];

                m_List.RemoveAt(0);
                m_Dict.Remove(marker);

                return true;
            }
            else
            {
                marker = null;
                callback = null;
                return false;
            }
        }
    }

    private struct SpriteGenerationSetting
    {
        public bool male;
        public List<EquippedApparel> equips;
        public System.Action<Sprite> callback;
        public Vector2 size;
        public Vector2 pivot;
        public Type type;
    }

    private enum Type
    {
        Portrait,
        Avatar,
        WardrobePortrait,
    }

    public static AvatarSpriteUtil Instance { get; private set; }

    [SerializeField] private ApparelView m_MaleView;
    [SerializeField] private ApparelView m_FemaleView;
    [SerializeField] private Camera m_FullbodyCamera;

    [Header("Portrait")]
    [SerializeField] private Image m_PortraitBackground;
    [SerializeField] private Image m_PortraitFrame;
    [SerializeField] private Camera m_PortraitCamera;
    [SerializeField, Range(0f,1f)] private float m_PortraitRadiusFactor = 1;

    [Header("Wardrobe Portrait")]
    [SerializeField] private Image m_PortraitFrame_Wardrobe;
    [SerializeField] private Camera m_PortraitCamera_Wardrobe;

    List<SpriteGenerationSetting> m_GenQueue = new List<SpriteGenerationSetting>();
    MarkerSpriteCollection m_AvatarQueue;
    MarkerSpriteCollection m_PortraitQueue;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        m_PortraitFrame.enabled = false;
        m_PortraitBackground.enabled = false;
        m_PortraitFrame_Wardrobe.enabled = false;

        m_PortraitCamera.enabled = false;
        m_FullbodyCamera.enabled = false;
        m_PortraitCamera_Wardrobe.enabled = false;

        m_AvatarQueue = new MarkerSpriteCollection();
        m_PortraitQueue = new MarkerSpriteCollection();
    }

    //public void GenerateHighResSprite(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback)
    //{
    //    SpriteGenerationSetting prop = new SpriteGenerationSetting
    //    {
    //        male = male,
    //        equips = equips,
    //        callbacks = new System.Action<Sprite>[] { callback },
    //        sizes = new Vector2[] { new Vector2(256, 512) },
    //        pivots = new Vector2[] { new Vector2(0.5f, 0.035f) },
    //        types = new Type[] { Type.Avatar }
    //    };

    //    if (m_Current == null)
    //        m_Current = StartCoroutine(GenerateSpriteCoroutine(prop));
    //    else
    //        m_Schedule.Add(prop);
    //}

    public void GenerateAvatar(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback)
    {
        SpriteGenerationSetting prop = new SpriteGenerationSetting
        {
            male = male,
            equips = equips,
            callback = callback,
            size = new Vector2(128, 256),
            pivot = new Vector2(0.5f, 0.035f),
            type = Type.Avatar
        };

        m_GenQueue.Add(prop);

        if (m_GenQueue.Count == 1)
            GenerateSprite();
    }

    public void GeneratePortrait(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback)
    {
        SpriteGenerationSetting prop = new SpriteGenerationSetting
        {
            male = male,
            equips = equips,
            callback = callback,
            size = new Vector2(256f / 2, 256f / 2),
            pivot = new Vector2(0.5f, 0.5f),
            type = Type.Portrait
        };
        
        m_GenQueue.Add(prop);

        if (m_GenQueue.Count == 1)
            GenerateSprite();
    }

    public void GenerateWardrobePortrait(System.Action<Sprite> callback)
    {
        SpriteGenerationSetting prop = new SpriteGenerationSetting
        {
            male = PlayerDataManager.playerData.male,
            equips = PlayerDataManager.playerData.equipped,
            callback = callback,
            size = new Vector2(256, 256),
            pivot = new Vector2(0.5f, 0.5f),
            type = Type.WardrobePortrait
        };

        m_GenQueue.Add(prop);

        if (m_GenQueue.Count == 1)
            GenerateSprite();
    }

    public void AddToAvatarQueue(WitchMarker marker, System.Action<Sprite> callback)
    {
        if (m_GenQueue.Count == 0)
            GenerateAvatar(marker.witchToken.male, marker.witchToken.equipped, callback);
        else
            m_AvatarQueue.Add(marker, callback);
    }

    public void RemoveFromAvatarQueue(WitchMarker marker)
    {
        if (m_AvatarQueue.Remove(marker))
        {
            //Debug.Log(marker.name + " removed : " + m_AvatarQueue.Count);
        }
    }
    
    public void AddToPortraitQueue(WitchMarker marker, System.Action<Sprite> callback)
    {
        if (m_GenQueue.Count == 0)
            GeneratePortrait(marker.witchToken.male, marker.witchToken.equipped, callback);
        else
            m_PortraitQueue.Add(marker, callback);
    }

    public void RemoveFromPortraitQueue(WitchMarker marker)
    {
        m_PortraitQueue.Remove(marker);
    }

    public void ClearQueues()
    {
        //string debug = "clear avatar queue " + m_AvatarQueue.Count; ;
        //foreach (var pair in m_AvatarQueue)
        //    debug += "\n\t" + pair.Item1.gameObject.name;
        //debug += "\nclear portrait queue " + m_PortraitQueue.Count;
        //foreach (var pair in m_PortraitQueue)
        //    debug += "\n\t" + pair.Item1.gameObject.name;
        //Debug.LogError(debug);

        m_AvatarQueue.Clear();
        m_PortraitQueue.Clear();
    }

    private void GenerateSprite()
    {
        SpriteGenerationSetting properties = m_GenQueue[0];

        //initialize vars
        ApparelView characterView;
        if (properties.male)
            characterView = m_MaleView;
        else
            characterView = m_FemaleView;
        Transform root = characterView.transform.root;
        bool prevRootState = root.gameObject.activeSelf;
        Vector3 prevRootPos = root.transform.position;
        bool prevState = characterView.gameObject.activeSelf;
        Camera cam;

        //move the character out of screen, and set it up
        root.transform.position = new Vector3(-1000, 0, 0);
        root.gameObject.SetActive(true);
        characterView.gameObject.SetActive(true);

        characterView.InitCharacter(properties.equips, true, () =>
        {
            //setup which assets to use
            if (properties.type == Type.Portrait)
            {
                m_PortraitFrame.enabled = true;
                m_PortraitBackground.enabled = true;

                cam = m_PortraitCamera;
            }
            else if (properties.type == Type.WardrobePortrait)
            {
                m_PortraitFrame_Wardrobe.enabled = true;

                cam = m_PortraitCamera_Wardrobe;
            }
            else
            {
                cam = m_FullbodyCamera;
            }

            cam.enabled = true;

            //generate the texture and sprite
            int width = (int)properties.size.x;
            int height = (int)properties.size.y;

            RenderTexture rt = new RenderTexture(width, height, 0);
            cam.targetTexture = rt;
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            cam.Render();
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            if (properties.type != Type.Avatar)
            {
                float cx = width / 2f;
                float cy = height / 2f;
                float r2 = width / 2f;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (Mathf.Sqrt(Mathf.Pow(x - cx, 2) + Mathf.Pow(y - cy, 2)) >= r2 * m_PortraitRadiusFactor)
                            texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                    }
                }
            }

            //generate the sprite
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), properties.pivot);

            //#if UNITY_EDITOR
            //            SaveTexture(texture);
            //#endif

            //reset values
            RenderTexture.active = prev;
            Destroy(rt);
            cam.targetTexture = null;
            cam.enabled = false;

            if (properties.type == Type.Portrait)
            {
                m_PortraitFrame.enabled = false;
                m_PortraitBackground.enabled = false;
            }
            else if (properties.type == Type.WardrobePortrait)
            {
                m_PortraitFrame_Wardrobe.enabled = false;
            }
            
            //reset character to initial state
            root.transform.position = prevRootPos;
            root.gameObject.SetActive(prevRootState);
            characterView.ResetApparelView();
            characterView.gameObject.SetActive(prevState);
                       
            properties.callback?.Invoke(sprite);
            m_GenQueue.RemoveAt(0);
            
            //when all is over, check the map avatar and potrait queue
            if (m_GenQueue.Count == 0)
            {
                WitchMarker marker = null;
                Action<Sprite> callback = null;

                if (m_AvatarQueue.GetFirst(out marker, out callback))
                    GenerateAvatar(marker.witchToken.male, marker.witchToken.equipped, callback);

                if (m_PortraitQueue.GetFirst(out marker, out callback))
                    GeneratePortrait(marker.witchToken.male, marker.witchToken.equipped, callback);
            }
            else
            {
                GenerateSprite();
            }
        });       
    }

    public IEnumerator<AvatarRequest> CreateWitchAvatar(IWitchModel witchModel)
    {
        bool isMale = (witchModel.Info.Gender == CharacterGender.Male);

        AvatarRequest request = new AvatarRequest();

        // Convert inventory to equipped apparel
        List<EquippedApparel> equippedApparels = new List<EquippedApparel>();
        foreach (InventoryApparelModel apparel in witchModel.Inventory.Equipped)
        {
            EquippedApparel equippedApparel = new EquippedApparel()
            {
                id = apparel.Id,
                assets = new List<string>(apparel.Assets)
            };
            equippedApparels.Add(equippedApparel);
            yield return request;
        }
        
        GenerateAvatar(isMale, equippedApparels, (sprite) =>
        {
            request.Avatar = sprite.texture;
            request.IsDone = true;
        });

        // wait for texture to be generated
        while (!request.IsDone)
        {
            yield return request;
        }

        yield return request;
    }

#if UNITY_EDITOR
    private void SaveTexture(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        if (System.IO.Directory.Exists(Application.dataPath + $"/../../Tools/AvatarSpriteUtils") == false)
            System.IO.Directory.CreateDirectory(Application.dataPath + $"/../../Tools/AvatarSpriteUtils");
        System.IO.File.WriteAllBytes(Application.dataPath + $"/../../Tools/AvatarSpriteUtils/{System.DateTime.Now.Ticks}.png", bytes);
    }    
#endif
}
