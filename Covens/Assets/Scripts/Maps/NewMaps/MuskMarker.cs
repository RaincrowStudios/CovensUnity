using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Raincrow.Maps
{
    public class MuskMarker : MonoBehaviour, IMarker
    {
        [Header("Base Marker")]
        private GameObject _m_GameObject;
        public new GameObject gameObject { get { return _m_GameObject; } }

        private object m_CustomData;
        public object customData
        {
            get { return m_CustomData; }
            set
            {
                m_CustomData = value;
                m_Data = m_CustomData as Token;
            }
        }

        public Vector2 coords { get; set; }

        public System.Action<IMarker> m_OnClick;
        public System.Action<IMarker> OnClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        public float scale
        {
            get { return transform.localScale.x; }
            set { transform.localScale = new Vector3(value, value, value); }
        }

        public bool inMapView { get; set; }

        public bool isNull { get { return this == null || this.gameObject == null; } }
        
        public virtual void OnDespawn()
        {
            LeanTween.cancel(m_AlphaTweenId);
            LeanTween.cancel(m_MoveTweenId);

            for (int i = 0; i < m_FXTweenIds.Count; i++)
                LeanTween.cancel(m_FXTweenIds[i]);

            foreach (var t in m_ParentedObjects)
                t.Value.Despawn(t.Key);

            m_ParentedObjects.Clear();

            gameObject.SetActive(false);
        }


        /******* NEW MARKER METHODS **********/

        protected bool m_Interactable = true;
        public Token m_Data;
        MarkerSpawner.MarkerType m_Type;

        public bool IsShowingIcon { get; protected set; }
        public bool IsShowingAvatar { get; protected set; }
        public virtual Transform characterTransform { get { return base.transform; } }
        public MarkerSpawner.MarkerType type { get { return m_Type; } }
        public Token token { get { return m_Data; } }

        protected const string m_ShadowColor = "#C100C8";
        protected const string m_GreyColor = "#00AFE4";
        protected const string m_WhiteColor = "#F48D00";
        
        public float characterAlpha { get; protected set; }
        public float textAlpha { get; protected set; }
        public float alpha { get; protected set; }

        [SerializeField] protected SpriteRenderer m_AvatarRenderer;
        [SerializeField] protected SpriteRenderer m_NameBanner;
        [SerializeField] protected Transform m_StatsContainer;
        [SerializeField] private SpriteRenderer m_EnergyRing;
        [SerializeField] protected SpriteRenderer[] m_Shadows;

        protected List<SpriteRenderer> m_Renderers;
        protected List<SpriteRenderer> m_CharacterRenderers;
        protected TextMeshPro[] m_TextMeshes;
        protected ParticleSystem[] m_Particles;

        private Dictionary<Transform, SimplePool<Transform>> m_ParentedObjects = new Dictionary<Transform, SimplePool<Transform>>();

        protected float m_CharacterAlphaMul = 1f;

        protected int m_MoveTweenId;
        protected int m_AlphaTweenId;
        protected int m_CharacterAlphaTweenId;
        private int m_EnergyRingTweenId;
        private float m_EnergyFill = 0;

        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                if (_m_GameObject != null)
                {
                    Collider[] colliders = _m_GameObject.GetComponentsInChildren<Collider>(true);
                    foreach (Collider _col in colliders)
                        _col.enabled = value;
                }
                m_Interactable = value;
            }
        }

        public bool IsPlayer { get { return (Object)PlayerManager.marker == this; } }

        private void Awake()
        {
            enabled = false;
            characterAlpha = 1;
            alpha = 1;
            textAlpha = 1;
            _m_GameObject = base.gameObject;

            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
            m_Particles = GetComponentsInChildren<ParticleSystem>(true);

            m_CharacterRenderers = new List<SpriteRenderer> { m_AvatarRenderer };
            UpdateRenderers();
        }

        public virtual void Setup(Token data)
        {
            m_Data = data;
            m_Type = data.Type;
        }

        public virtual void EnablePortait() { }

        public virtual void EnableAvatar() { }

        public virtual void SetStats() { }

        public virtual void UpdateEnergy()
        {
            if (m_EnergyRing == null)
                return;

            if (token == null)
                return;

            m_EnergyFill = (float)(token as CharacterToken).energy;
            m_EnergyFill /= (token as CharacterToken).baseEnergy;

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

            textAlpha = a;

            for (int i = 0; i < m_TextMeshes.Length; i++)
                m_TextMeshes[i].alpha = textAlpha * alpha;
        }

        public void SetCharacterAlpha(float a, float time = 0, System.Action onComplete = null)
        {
            if (this == null)
                return;

            LeanTween.cancel(m_CharacterAlphaTweenId);

            Color aux;

            if (time == 0)
            {
                characterAlpha = a;

                for (int i = 0; i < m_CharacterRenderers.Count; i++)
                {
                    aux = m_CharacterRenderers[i].color;
                    aux.a = alpha * characterAlpha * m_CharacterAlphaMul;
                    m_CharacterRenderers[i].color = aux;
                }
                onComplete?.Invoke();
            }
            else
            {
                m_CharacterAlphaTweenId = LeanTween.value(characterAlpha, a, time)
                      .setEaseOutCubic()
                      .setOnUpdate((float t) =>
                      {
                          characterAlpha = t;

                          for (int i = 0; i < m_CharacterRenderers.Count; i++)
                          {
                              aux = m_CharacterRenderers[i].color;
                              aux.a = alpha * characterAlpha * m_CharacterAlphaMul;
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
            LeanTween.cancel(m_EnergyRingTweenId);

            //fade spriterenderers and textmeshes
            if (time == 0)
            {
                alpha = a;

                Color aux;
                for (int i = 0; i < m_Renderers.Count; i++)
                {
                    aux = m_Renderers[i].color;
                    aux.a = alpha;
                    m_Renderers[i].color = aux;
                }

                for (int i = 0; i < m_CharacterRenderers.Count; i++)
                {
                    aux = m_CharacterRenderers[i].color;
                    aux.a = alpha * characterAlpha * m_CharacterAlphaMul;
                    m_CharacterRenderers[i].color = aux;
                }

                for (int i = 0; i < m_Shadows.Length; i++)
                {
                    aux = m_Shadows[i].color;
                    aux.a = alpha * 0.5f;
                    m_Shadows[i].color = aux;
                }

                for (int i = 0; i < m_TextMeshes.Length; i++)
                {
                    m_TextMeshes[i].alpha = textAlpha * alpha;
                }

                onComplete?.Invoke();
            }
            else
            {
                m_AlphaTweenId = LeanTween.value(alpha, a, time)
                    .setEaseOutCubic()
                    .setOnUpdate((float t) =>
                    {
                        alpha = t;

                        Color aux;
                        for (int i = 0; i < m_Renderers.Count; i++)
                        {
                            aux = m_Renderers[i].color;
                            aux.a = alpha;
                            m_Renderers[i].color = aux;
                        }

                        for (int i = 0; i < m_Shadows.Length; i++)
                        {
                            aux = m_Shadows[i].color;
                            aux.a = alpha * 0.5f;
                            m_Shadows[i].color = aux;
                        }

                        for (int i = 0; i < m_TextMeshes.Length; i++)
                            m_TextMeshes[i].alpha = textAlpha * alpha;


                        for (int i = 0; i < m_CharacterRenderers.Count; i++)
                        {
                            aux = m_CharacterRenderers[i].color;
                            aux.a = alpha * characterAlpha * m_CharacterAlphaMul;
                            m_CharacterRenderers[i].color = aux;
                        }
                    })
                    .setOnComplete(onComplete)
                    .uniqueId;
            }

            //fade particles and energy ring
            if (Mathf.Approximately(a, 0))
            {
                for (int i = 0; i < m_Particles.Length; i++)
                {
                    if (m_Particles[i].isEmitting && m_Particles[i].main.loop)
                        m_Particles[i].Stop(false);
                }

                //hide the energy ring
                if (m_EnergyRing != null)
                    m_EnergyRingTweenId = LeanTween.alpha(m_EnergyRing.gameObject, a, time / 2).uniqueId;
            }
            else
            {
                for (int i = 0; i < m_Particles.Length; i++)
                {
                    if (!m_Particles[i].isEmitting && m_Particles[i].main.loop)
                        m_Particles[i].Play(false);
                }

                //show the ring at the energy amount
                if (m_EnergyRing != null)
                    m_EnergyRingTweenId = LeanTween.alpha(m_EnergyRing.gameObject, m_EnergyFill, time / 2).uniqueId;
            }
        }

        protected virtual void UpdateRenderers()
        {
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
            m_Renderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));

            foreach (SpriteRenderer sr in m_CharacterRenderers)
                m_Renderers.Remove(sr);
            foreach (SpriteRenderer sr in m_Shadows)
                m_Renderers.Remove(sr);
            m_Renderers.Remove(m_EnergyRing);
        }

        public void AddChild(Transform t, Transform parent, SimplePool<Transform> pool)
        {
            m_ParentedObjects.Add(t, pool);

            t.SetParent(parent);
            t.localPosition = new Vector3(0, 0, -0.5f);
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;

            UpdateRenderers();
        }

        public void RemoveChild(Transform t)
        {
            if (m_ParentedObjects.ContainsKey(t))
            {
                m_ParentedObjects[t]?.Despawn(t);
                m_ParentedObjects.Remove(t);
            }

            UpdateRenderers();
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

        private struct FXQueueItem
        {
            public SimplePool<Transform> pool;
            public float duration;
            public System.Action<Transform> onSpawn;
            public bool character;

            public FXQueueItem(SimplePool<Transform> pool, bool character, float duration, System.Action<Transform> onSpawn)
            {
                this.pool = pool;
                this.duration = duration;
                this.onSpawn = onSpawn;
                this.character = character;
            }
        }

        private List<FXQueueItem> m_CharacterFXQueue = new List<FXQueueItem>();
        private List<FXQueueItem> m_RootFXQueue = new List<FXQueueItem>();
        private List<int> m_FXTweenIds = new List<int>();

        public void SpawnFX(SimplePool<Transform> fxPool, bool character, float duration, bool queued, System.Action<Transform> onSpawn)
        {
            if (queued)
            {
                List<FXQueueItem> queue = character ? m_CharacterFXQueue : m_RootFXQueue;

                //queue the fx
                queue.Add(new FXQueueItem(fxPool, character, duration, onSpawn));

                //show if nothing else is showing
                if (queue.Count == 1)
                    SpawnFX(queue);
            }
            else if (IsShowingAvatar && inMapView)
            {
                SimplePool<Transform> pool = fxPool;
                Transform instance = pool.Spawn();

                if (character)
                    AddChild(instance, characterTransform, pool);
                else
                    AddChild(instance, transform, pool);

                onSpawn?.Invoke(instance);
                int tweenId = 0;
                tweenId = LeanTween.value(0, 0, duration)
                    .setOnComplete(() =>
                    {
                        RemoveChild(instance);
                        m_FXTweenIds.Remove(tweenId);
                    })
                    .uniqueId;
                m_FXTweenIds.Add(tweenId);
            }
        }

        private void SpawnFX(List<FXQueueItem> queue)
        {
            if (queue.Count > 0)
            {
                int tweenId = 0;
                //dont show if the marker is not visible or in portrait mode
                if (IsShowingIcon || !inMapView)
                {
                    tweenId = LeanTween.value(0, 0, queue[0].duration)
                        .setOnComplete(() =>
                        {
                            m_FXTweenIds.Remove(tweenId);
                            queue.RemoveAt(0);
                            SpawnFX(queue);
                        })
                        .uniqueId;
                    m_FXTweenIds.Add(tweenId);
                    return;
                }

                SimplePool<Transform> pool = queue[0].pool;
                Transform instance = pool.Spawn();

                if (queue[0].character)
                    AddChild(instance, characterTransform, pool);
                else
                    AddChild(instance, transform, pool);

                queue[0].onSpawn?.Invoke(instance);
                tweenId = LeanTween.value(0, 0, queue[0].duration)
                    .setOnComplete(() =>
                    {
                        m_FXTweenIds.Remove(tweenId);
                        RemoveChild(instance);
                        queue.RemoveAt(0);

                        //show the next
                        SpawnFX(queue);
                    })
                    .uniqueId;
                m_FXTweenIds.Add(tweenId);
            }
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
            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(token, Newtonsoft.Json.Formatting.Indented));
        }

        [ContextMenu("Focus on")]
        private void FocusOnMarker()
        {
            MapCameraUtils.FocusOnPosition(transform.position, false);
        }
#endif
    }
}