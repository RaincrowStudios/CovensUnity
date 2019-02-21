using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Raincrow.Maps
{
    public class NewMapsMarker : MonoBehaviour, IMarker
    {
        private object m_CustomData;
        public object customData
        {
            get { return m_CustomData; }
            set { m_CustomData = value; }
        }

        Vector2 m_Position;
        public Vector2 position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;
                transform.position = MapController.Instance.CoordsToWorldPosition(value.x, value.y);
            }
        }

        public System.Action<IMarker> m_OnClick;
        public System.Action<IMarker> OnClick
        {
            get { return m_OnClick; }
            set
            {
                m_OnClick = value;
            }
        }

        public float scale
        {
            get { return transform.localScale.x; }
            set { transform.localScale = new Vector3(value, value, value); }
        }

        public GameObject instance { get { return gameObject; } }

        public bool inMapView
        {
            get
            {
                //replace for faster method?
                Vector2 view = MapController.Instance.camera.WorldToViewportPoint(transform.position);
                return view.x >= 0 && view.x <= 1 && view.y >= 0 && view.y <= 1;
            }
        }

        public void SetPosition(double lng, double lat)
        {
            m_Position = new Vector2((float)lng, (float)lat);
            this.transform.position = MapController.Instance.CoordsToWorldPosition(lng, lat);
        }
        
        private void OnMouseDown()
        {
            //dont trigger if clicking over an UI element
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            if (m_Interactable)
                m_OnClick?.Invoke(this);
        }


        /******* NEW MARKER METHODS **********/

        private Token m_Data;

        private Transform m_Icon;
        private Transform m_Avatar;
        private TextMeshPro m_DisplayName;
        private TextMeshPro m_Stats;

        private int m_TweenId;
        private Transform m_CameraCenter;

        public bool IsShowingIcon { get; private set; }
        public bool IsShowingAvatar { get; private set; }

        private const string m_ShadowColor = "#C100C8";
        private const string m_GreyColor = "#00AFE4";
        private const string m_WhiteColor = "#F48D00";

        private void Awake()
        {
            enabled = false;
        }

        public void Setup(Token data)
        {
            m_Data = data;
            m_CameraCenter = MapController.Instance.m_StreetMap.cameraCenter;

            if (data.Type == MarkerSpawner.MarkerType.witch || data.Type == MarkerSpawner.MarkerType.spirit)
            {
                m_Avatar = transform.GetChild(0);
                m_Icon = transform.GetChild(1);

                m_Avatar.localScale = Vector3.zero;
                m_Icon.localScale = Vector3.zero;

                m_Avatar.gameObject.SetActive(false);
                m_Icon.gameObject.SetActive(false);

                IsShowingAvatar = false;
                IsShowingIcon = false;

                if(data.Type == MarkerSpawner.MarkerType.witch)
                {
                    m_DisplayName   = m_Avatar.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
                    m_Stats         = m_Avatar.GetChild(0).GetChild(2).GetComponent<TextMeshPro>();
                    m_DisplayName.text = data.displayName;
                    SetStats(data.level, data.energy);
                    
                    //setup the school particle fx
                    Transform iconSchools = transform.GetChild(1).GetChild(0).GetChild(0);
                    Transform avatarSchools = transform.GetChild(0).GetChild(1);

                    for (int i = 0; i < 3; i++)
                    {
                        iconSchools.GetChild(i).gameObject.SetActive(false);
                        avatarSchools.GetChild(i).gameObject.SetActive(false);
                    }

                    if (data.degree < 0)
                    {
                        avatarSchools.GetChild(0).gameObject.SetActive(true);
                        iconSchools.GetChild(0).gameObject.SetActive(true);
                    }
                    else if (data.degree == 0)
                    {
                        avatarSchools.GetChild(1).gameObject.SetActive(true);
                        iconSchools.GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        avatarSchools.GetChild(2).gameObject.SetActive(true);
                        iconSchools.GetChild(2).gameObject.SetActive(true);
                    }
                }
                enabled = true;
            }
            else
            {
                IsShowingIcon = true;
                IsShowingAvatar = true;
            }
        }

        public void SetIcon()
        {
            if (IsShowingIcon)
                return;

            IsShowingIcon = true;
            IsShowingAvatar = false;

            LeanTween.cancel(m_TweenId);

            m_TweenId = LeanTween.value(m_Icon.localScale.x, 1, 0.5f)
                .setEaseOutCubic()
                .setOnStart(() =>
                {
                    m_Icon.gameObject.SetActive(true);
                })
                .setOnUpdate((float t) =>
                {
                    m_Icon.localScale = new Vector3(t, t, t);
                    m_Avatar.localScale = new Vector3(1 - t, 1 - t, 1 - t);
                })
                .setOnComplete(() =>
                {
                    m_Avatar.gameObject.SetActive(false);
                })
                .uniqueId;
        }

        public void SetAvatar()
        {
            if (IsShowingAvatar)
                return;

            IsShowingAvatar = true;
            IsShowingIcon = false;

            LeanTween.cancel(m_TweenId);

            m_TweenId = LeanTween.value(m_Avatar.localScale.x, 1, 0.5f)
                .setEaseOutCubic()
                .setOnStart(() =>
                {
                    m_Avatar.gameObject.SetActive(true);
                })
                .setOnUpdate((float t) =>
                {
                    m_Avatar.localScale = new Vector3(t, t, t);
                    m_Icon.localScale = new Vector3(1 - t, 1 - t, 1 - t);
                })
                .setOnComplete(() =>
                {
                    m_Icon.gameObject.SetActive(false);
                })
                .uniqueId;
        }

        public void SetStats(int level, int energy)
        {
            if (m_Stats == null)
                return;

            string color = "";
            if (m_Data.degree < 0)          color = m_ShadowColor;
            else if (m_Data.degree == 0)    color = m_GreyColor;
            else                            color = m_WhiteColor;

            m_Stats.text = 
                $"Energy: <color={color}><b>{energy}</b></color>\n" +
                $"lvl: <color={color}><b>{level}</b></color>";
        }

        public void SetupAvatar(bool male, List<EquippedApparel> equips)
        {
            //shadow scale
            transform.GetChild(0).GetChild(2).localScale = male ? new Vector3(8, 8, 8) : new Vector3(6, 6, 6);
            AvatarSpriteUtil.Instance.GenerateFullbodySprite(male, equips, spr =>
            {
                SpriteRenderer renderer = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
                renderer.transform.localPosition = Vector3.zero;
                renderer.transform.localScale = new Vector3(8, 8, 8);
                renderer.sprite = spr;
            });
        }

        public void SetupAvatarAndPortrait(bool male, List<EquippedApparel> equips)
        {
            //shadow scale
            transform.GetChild(0).GetChild(2).localScale = male ? new Vector3(8, 8, 8) : new Vector3(6, 6, 6);

            //generate the sprites
            AvatarSpriteUtil.Instance.GeneratePortraitAndFullbody(male, equips, 
                portrait =>
                {
                    //portrait
                    SpriteRenderer renderer = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
                    renderer.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
                    renderer.sprite = portrait;
                },
                avatar =>
                {
                    SpriteRenderer renderer = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
                    renderer.transform.localPosition = Vector3.zero;
                    renderer.transform.localScale = new Vector3(8, 8, 8);
                    renderer.sprite = avatar;
                });
        }

        private float m_Distance;
        private void Update()
        {
            m_Distance = Vector2.Distance(
                new Vector2(m_CameraCenter.position.x, m_CameraCenter.position.z), new Vector2(transform.position.x, transform.position.z));

            if (m_Distance < 55)
                SetAvatar();
            else
                SetIcon();
        }

        private bool m_Interactable = true;
        public bool interactable { get { return m_Interactable; } set { m_Interactable = value; } }
    }
}