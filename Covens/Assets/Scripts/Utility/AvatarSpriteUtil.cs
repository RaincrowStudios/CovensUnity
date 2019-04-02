using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSpriteUtil : MonoBehaviour
{
    private struct SpriteGenerationSetting
    {
        public bool male;
        public List<EquippedApparel> equips;
        public System.Action<Sprite>[] callbacks;
        public Vector2[] sizes;
        public Vector2[] pivots;
        public Type[] types;
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
    [SerializeField] private Mask m_PortraitMask;
    [SerializeField] private Image m_PortraitImage;
    [SerializeField] private Image m_PortraitBackground;
    [SerializeField] private Image m_PortraitFrame;
    [SerializeField] private Camera m_PortraitCamera;

    [Header("Wardrobe Portrait")]
    [SerializeField] private Image m_PortraitFrame_Wardrobe;
    [SerializeField] private Camera m_PortraitCamera_Wardrobe;

    List<SpriteGenerationSetting> m_Schedule = new List<SpriteGenerationSetting>();

    private Coroutine m_Current;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("MULTIPLE INSTANCES IN SCENE");
        }

        Instance = this;

        if (m_PortraitImage == null)
            m_PortraitImage = m_PortraitMask.GetComponent<Image>();

        m_PortraitMask.enabled = false;
        m_PortraitImage.enabled = false;
        m_PortraitFrame.enabled = false;
        m_PortraitBackground.enabled = false;
        m_PortraitFrame_Wardrobe.enabled = false;

        m_PortraitCamera.enabled = false;
        m_FullbodyCamera.enabled = false;
        m_PortraitCamera_Wardrobe.enabled = false;
    }

    public void GenerateFullbodySprite(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback)
    {
        SpriteGenerationSetting prop = new SpriteGenerationSetting
        {
            male = male,
            equips = equips,
            callbacks = new System.Action<Sprite>[] { callback },
            sizes = new Vector2[] { new Vector2(128, 512) },
            pivots = new Vector2[] { new Vector2(0.5f, 0.0f) },
            types = new Type[] { Type.Avatar }
        };

        if (m_Current == null)
            m_Current = StartCoroutine(GenerateSpriteCoroutine(prop));
        else
            m_Schedule.Add(prop);
    }

    public void GeneratePortrait(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback)
    {
        SpriteGenerationSetting prop = new SpriteGenerationSetting
        {
            male = male,
            equips = equips,
            callbacks = new System.Action<Sprite>[] { callback },
            sizes = new Vector2[] { new Vector2(256f/2, 256f/2) },
            pivots = new Vector2[] { new Vector2(0.5f, 0.5f) },
            types = new Type[] { Type.Portrait }
        };

        if (m_Current == null)
            m_Current = StartCoroutine(GenerateSpriteCoroutine(prop));
        else
            m_Schedule.Add(prop);
    }

    //public void GeneratePortraitAndFullbody(bool male, List<EquippedApparel> equips, System.Action<Sprite> portraitCallback, System.Action<Sprite> avatarCallback)
    //{
    //    SpriteGenerationSetting prop = new SpriteGenerationSetting
    //    {
    //        male = male,
    //        equips = equips,
    //        callbacks = new System.Action<Sprite>[] { portraitCallback, avatarCallback },
    //        sizes = new Vector2[] { new Vector2(256, 256), new Vector2(256, 500) },
    //        pivots = new Vector2[] { new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.0f) },
    //        types = new Type[] { Type.Portrait, Type.Avatar }
    //    };

    //    if (m_Current == null)
    //        m_Current = StartCoroutine(GenerateSpriteCoroutine(prop));
    //    else
    //        m_Schedule.Add(prop);
    //}

    public void GenerateWardrobePortrait(System.Action<Sprite> callback)
    {
        SpriteGenerationSetting sett = new SpriteGenerationSetting
        {
            male = PlayerDataManager.playerData.male,
            equips = PlayerDataManager.playerData.equipped,
            callbacks = new System.Action<Sprite>[] { callback },
            sizes = new Vector2[] { new Vector2(256, 256) },
            pivots = new Vector2[] { new Vector2(0.5f, 0.5f) },
            types = new Type[] { Type.WardrobePortrait }
        };

        if (m_Current == null)
            m_Current = StartCoroutine(GenerateSpriteCoroutine(sett));
        else
            m_Schedule.Add(sett);
    }

    private IEnumerator GenerateSpriteCoroutine(SpriteGenerationSetting properties)
    {
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
        characterView.InitializeChar(properties.equips);

        //generate the sprites for each camera passed
        for (int i = 0; i < properties.callbacks.Length; i++)
        {
            yield return new WaitForEndOfFrame();
                                   
            if (properties.types[i] == Type.Portrait)
            {
                m_PortraitImage.enabled = true;
                m_PortraitMask.enabled = true;
                m_PortraitFrame.enabled = true;
                m_PortraitBackground.enabled = true;

                cam = m_PortraitCamera;
            }
            else if (properties.types[i] == Type.WardrobePortrait)
            {
                m_PortraitImage.enabled = true;
                m_PortraitMask.enabled = true;
                m_PortraitFrame_Wardrobe.enabled = true;

                cam = m_PortraitCamera_Wardrobe;
            }
            else
            {
                cam = m_FullbodyCamera;
            }

            cam.enabled = true;

            //generate the texture and sprite
            int width = (int)properties.sizes[i].x;
            int height = (int)properties.sizes[i].y;
            RenderTexture rt = new RenderTexture(width, height, 32);
            cam.targetTexture = rt;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            cam.Render();
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            RenderTexture.active = prev;
            cam.targetTexture = null;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), properties.pivots[i]);
            cam.enabled = false;

            properties.callbacks[i]?.Invoke(sprite);

//#if UNITY_EDITOR
//            SaveTexture(texture);
//#endif

            if (properties.types[i] == Type.Portrait)
            {
                m_PortraitImage.enabled = false;
                m_PortraitMask.enabled = false;
                m_PortraitFrame.enabled = false;
                m_PortraitBackground.enabled = false;
            }
            else if (properties.types[i] == Type.WardrobePortrait)
            {
                m_PortraitImage.enabled = false;
                m_PortraitMask.enabled = false;
                m_PortraitFrame_Wardrobe.enabled = false;
            }
        }
        
        //reset character to initial state
        root.transform.position = prevRootPos;
        root.gameObject.SetActive(prevRootState);
        characterView.gameObject.SetActive(prevState);
        
        if( m_Schedule.Count > 0)
        {
            //yield return 1;
            SpriteGenerationSetting prop = m_Schedule[0];
            m_Schedule.RemoveAt(0);
            m_Current = StartCoroutine(GenerateSpriteCoroutine(prop));
        }
        else
        {
            m_Current = null;
        }
    }

#if UNITY_EDITOR
    //////////////DEBUG
    [Header("Debug")]
    [SerializeField] private string m_EquipList;
    [SerializeField] private SpriteRenderer m_Renderer;

    [ContextMenu("Debug portrait")]
    private void GenerateDebugPortrait()
    {
        MarkerDataDetail markerdata = Newtonsoft.Json.JsonConvert.DeserializeObject<MarkerDataDetail>(m_EquipList);
        //GeneratePortraitSprite(markerdata.male, markerdata.equipped, (spr) => m_Renderer.sprite = spr);
    }

    [ContextMenu("Debug avatar")]
    private void GenerateDebugAvatar()
    {
        MarkerDataDetail markerdata = Newtonsoft.Json.JsonConvert.DeserializeObject<MarkerDataDetail>(m_EquipList);
        GenerateFullbodySprite(markerdata.male, markerdata.equipped, (spr) => m_Renderer.sprite = spr);
    }

    private void SaveTexture(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        if (System.IO.Directory.Exists(Application.dataPath + $"/../../Tools/AvatarSpriteUtils") == false)
            System.IO.Directory.CreateDirectory(Application.dataPath + $"/../../Tools/AvatarSpriteUtils");
        System.IO.File.WriteAllBytes(Application.dataPath + $"/../../Tools/AvatarSpriteUtils/{System.DateTime.Now.Ticks}.png", bytes);
    }
#endif
}
