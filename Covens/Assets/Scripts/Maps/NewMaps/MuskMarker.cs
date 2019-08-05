using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Raincrow.Maps
{
    public class MuskMarker : MonoBehaviour, IMarker
    {
        [Header("Base Marker")]
        [SerializeField] protected SpriteRenderer m_AvatarRenderer;
        [SerializeField] protected SpriteRenderer m_NameBanner;
        [SerializeField] protected Transform m_StatsContainer;
        [SerializeField] protected SpriteRenderer m_EnergyRing;
        [SerializeField] protected SpriteRenderer[] m_Shadows;

        protected bool m_Interactable = true;

        public GameObject GameObject { get { return gameObject; } }
        public virtual Transform AvatarTransform { get { return transform; } }

        public MarkerManager.MarkerType Type { get; private set; }
        public bool IsShowingIcon { get; protected set; }
        public bool IsShowingAvatar { get; protected set; }

        public Vector2 Coords { get; set; }
        public System.Action<IMarker> OnClick { get; set; }

        public float AvatarAlpha { get; protected set; }
        public float TextAlpha { get; protected set; }
        public float Alpha { get; protected set; }

        public Token Token { get; protected set; }

        public bool IsPlayer { get { return (Object)PlayerManager.marker == this; } }
        

        protected List<SpriteRenderer> m_Renderers;
        protected List<SpriteRenderer> m_CharacterRenderers;
        protected TextMeshPro[] m_TextMeshes;
        protected ParticleSystem[] m_Particles;
        
        protected float m_CharacterAlphaMul = 1f;
        protected int m_MoveTweenId;
        protected int m_AlphaTweenId;
        protected int m_CharacterAlphaTweenId;
        private int m_EnergyRingTweenId;

        protected Color m_SchoolColor = Color.white;
        private float m_EnergyFill = 0;
        
        public float scale
        {
            get { return transform.localScale.x; }
            set { transform.localScale = new Vector3(value, value, value); }
        }

        public bool inMapView { get; set; }

        public bool isNull { get { return this == null || this.GameObject == null; } }
        
        public virtual void OnDespawn()
        {
            LeanTween.cancel(m_AlphaTweenId);
            LeanTween.cancel(m_MoveTweenId);
            
            OnClick = null;

            GameObject.SetActive(false);
        }
                     
        public bool Interactable
        {
            get { return m_Interactable; }
            set
            {
                if (GameObject != null)
                {
                    Collider[] colliders = GameObject.GetComponentsInChildren<Collider>(true);
                    foreach (Collider _col in colliders)
                        _col.enabled = value;
                }
                m_Interactable = value;
            }
        }

        private void Awake()
        {
            enabled = false;
            AvatarAlpha = 1;
            Alpha = 1;
            TextAlpha = 1;

            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
            m_Particles = GetComponentsInChildren<ParticleSystem>(true);

            m_CharacterRenderers = new List<SpriteRenderer> { m_AvatarRenderer };
            if (m_Shadows == null)
                m_Shadows = new SpriteRenderer[0];

            UpdateRenderers();
        }

        public virtual void Setup(Token data)
        {
            Token = data;
            Type = data.Type;
        }

        public virtual void EnablePortait() { }

        public virtual void EnableAvatar() { }

        public virtual void SetStats() { }

        public virtual void UpdateEnergy()
        {
            if (m_EnergyRing == null)
                return;

            if (Token == null)
                return;

            m_EnergyFill = (float)(Token as CharacterToken).energy;
            m_EnergyFill /= (Token as CharacterToken).baseEnergy;

            LeanTween.cancel(m_EnergyRingTweenId);
            m_EnergyRingTweenId = LeanTween.alpha(m_EnergyRing.gameObject, m_EnergyFill, 1f).uniqueId;
        }


        public virtual void UpdateNameplate(float preferredWidth)
        {
            if (m_NameBanner == null)
                return;
            
            Vector2 bannerSize = new Vector2(MapUtils.scale(2.2f, 9.5f, .86f, 8f, preferredWidth), m_NameBanner.size.y);
            m_NameBanner.size = bannerSize;

            if (m_StatsContainer == null)
                return;

            Vector3 statPos = new Vector3(
                -MapUtils.scale(0f, 3.6f, 2.2f, 9.5f, m_NameBanner.size.x),
                m_StatsContainer.localPosition.y,
                m_StatsContainer.localPosition.z
            );
            m_StatsContainer.localPosition = statPos;
        }

        public void SetTextAlpha(float a)
        {
            if (this == null)
                return;

            TextAlpha = a;

            for (int i = 0; i < m_TextMeshes.Length; i++)
                m_TextMeshes[i].alpha = TextAlpha * Alpha;
        }

        public void SetCharacterAlpha(float a, float time = 0, System.Action onComplete = null)
        {
            if (this == null)
                return;

            LeanTween.cancel(m_CharacterAlphaTweenId);

            Color aux;

            if (time == 0)
            {
                AvatarAlpha = a;

                for (int i = 0; i < m_CharacterRenderers.Count; i++)
                {
                    aux = m_CharacterRenderers[i].color;
                    aux.a = Alpha * AvatarAlpha * m_CharacterAlphaMul;
                    m_CharacterRenderers[i].color = aux;
                }
                onComplete?.Invoke();
            }
            else
            {
                m_CharacterAlphaTweenId = LeanTween.value(AvatarAlpha, a, time)
                      .setEaseOutCubic()
                      .setOnUpdate((float t) =>
                      {
                          AvatarAlpha = t;

                          for (int i = 0; i < m_CharacterRenderers.Count; i++)
                          {
                              aux = m_CharacterRenderers[i].color;
                              aux.a = Alpha * AvatarAlpha * m_CharacterAlphaMul;
                              m_CharacterRenderers[i].color = aux;
                          }
                      })
                      .setOnComplete(onComplete)
                      .uniqueId;
            }
        }

        public virtual void SetAlpha(float a, float time = 0, System.Action onComplete = null)
        {
            if (isNull)
                return;

            LeanTween.cancel(m_AlphaTweenId, true);

            //fade spriterenderers and textmeshes
            if (time == 0)
            {
                Alpha = a;

                Color aux;
                for (int i = 0; i < m_Renderers.Count; i++)
                {
                    aux = m_Renderers[i].color;
                    aux.a = Alpha;
                    m_Renderers[i].color = aux;
                }

                for (int i = 0; i < m_CharacterRenderers.Count; i++)
                {
                    aux = m_CharacterRenderers[i].color;
                    aux.a = Alpha * AvatarAlpha * m_CharacterAlphaMul;
                    m_CharacterRenderers[i].color = aux;
                }

                for (int i = 0; i < m_Shadows.Length; i++)
                {
                    aux = m_Shadows[i].color;
                    aux.a = Alpha * 0.4f;
                    m_Shadows[i].color = aux;
                }

                for (int i = 0; i < m_TextMeshes.Length; i++)
                {
                    m_TextMeshes[i].alpha = TextAlpha * Alpha;
                }

                if (m_EnergyRing)
                {
                    aux = Color.Lerp(Color.black, m_SchoolColor, a);
                    aux.a = m_EnergyFill;
                    m_EnergyRing.color = aux;
                }

                onComplete?.Invoke();
            }
            else
            {
                m_AlphaTweenId = LeanTween.value(Alpha, a, time)
                    .setEaseOutCubic()
                    .setOnUpdate((float t) =>
                    {
                        Alpha = t;

                        Color aux;
                        for (int i = 0; i < m_Renderers.Count; i++)
                        {
                            aux = m_Renderers[i].color;
                            aux.a = Alpha;
                            m_Renderers[i].color = aux;
                        }

                        for (int i = 0; i < m_Shadows.Length; i++)
                        {
                            aux = m_Shadows[i].color;
                            aux.a = Alpha * 0.5f;
                            m_Shadows[i].color = aux;
                        }

                        for (int i = 0; i < m_TextMeshes.Length; i++)
                            m_TextMeshes[i].alpha = TextAlpha * Alpha;
                        
                        for (int i = 0; i < m_CharacterRenderers.Count; i++)
                        {
                            aux = m_CharacterRenderers[i].color;
                            aux.a = Alpha * AvatarAlpha * m_CharacterAlphaMul;
                            m_CharacterRenderers[i].color = aux;
                        }

                        if (m_EnergyRing)
                        {
                            aux = Color.Lerp(Color.black, m_SchoolColor, Alpha);
                            aux.a = m_EnergyFill;
                            m_EnergyRing.color = aux;
                        }
                    })
                    .setOnComplete(onComplete)
                    .uniqueId;
            }

            //stop particles
            if (Mathf.Approximately(a, 0))
            {
                for (int i = 0; i < m_Particles.Length; i++)
                {
                    if (m_Particles[i].isEmitting && m_Particles[i].main.loop)
                        m_Particles[i].Stop(false);
                }
            }
            else //reenable particles
            {
                for (int i = 0; i < m_Particles.Length; i++)
                {
                    if (!m_Particles[i].isEmitting && m_Particles[i].main.loop)
                        m_Particles[i].Play(false);
                }
            }
        }

        public void UpdateRenderers()
        {
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
            m_Renderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));

            foreach (SpriteRenderer sr in m_CharacterRenderers)
                m_Renderers.Remove(sr);
            foreach (SpriteRenderer sr in m_Shadows)
                m_Renderers.Remove(sr);

            if (m_EnergyRing != null)
                m_Renderers.Remove(m_EnergyRing);
        }
        
        public void SetWorldPosition(Vector3 worldPos, float time = 0, System.Action onComplete = null)
        {
            LeanTween.cancel(m_MoveTweenId);

            if (time == 0)
            {
                transform.position = worldPos;
                MarkerSpawner.Instance.UpdateMarker(this);
                onComplete?.Invoke();
                return;
            }

            Vector3 startPos = transform.position;

            m_MoveTweenId = LeanTween.value(0, 1, time)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    transform.position = Vector3.Lerp(startPos, worldPos, t);
                    MarkerSpawner.Instance.UpdateMarker(this);
                })
                .setOnComplete(() =>
                {
                    transform.position = worldPos;
                    onComplete?.Invoke();
                })
                .uniqueId;
        }
        
        private void OnDisable()
        {
            //in case the marker was disabled while animating the movement
            LeanTween.cancel(m_MoveTweenId, true);
        }

#if UNITY_EDITOR
        [Header("Base Debug")]
        [SerializeField, Range(0,1)] private float m_DebugFloat;

        [ContextMenu("Update energy")]
        private void DebugEnergy()
        {
            UpdateEnergy();
        }


        [ContextMenu("Print token")]
        private void PrintToken()
        {
            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(Token, Newtonsoft.Json.Formatting.Indented));
        }

        [ContextMenu("Focus on")]
        private void FocusOnMarker()
        {
            MapCameraUtils.FocusOnPosition(transform.position, false);
        }
#endif
    }
}