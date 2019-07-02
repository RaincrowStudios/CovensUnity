using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Raincrow.Maps
{
    public class MuskMarker : MonoBehaviour, IMarker
    {
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
        //{
        //    get
        //    {
        //        //replace for faster method?
        //        Vector2 view = MapController.Instance.camera.WorldToViewportPoint(transform.position);
        //        return view.x >= 0 && view.x <= 1 && view.y >= 0 && view.y <= 1;
        //    }
        //}

        public bool isNull { get { return this == null || this.gameObject == null; } }
        
        public virtual void Destroy()
        {
            LeanTween.cancel(m_AlphaTweenId);
            LeanTween.cancel(m_MoveTweenId);

            for (int i = 0; i < m_FXTweenIds.Count; i++)
                LeanTween.cancel(m_FXTweenIds[i]);

            foreach (var t in m_ParentedObjects)
                t.Value.Despawn(t.Key);

            m_ParentedObjects.Clear();

            gameObject.SetActive(false);
            Destroy(this.gameObject);
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

        public const float defaultTextAlpha = 0.35f;
        public const float highlightTextAlpha = 1f;

        public float characterAlpha { get; protected set; }
        public float textAlpha { get; protected set; }
        public float alpha { get; protected set; }

        [SerializeField] protected SpriteRenderer m_AvatarRenderer;
        [SerializeField] protected SpriteRenderer[] m_Shadows;

        protected SpriteRenderer[] m_Renderers;
        protected SpriteRenderer[] m_CharacterRenderers;
        protected TextMeshPro[] m_TextMeshes;
        protected ParticleSystem[] m_Particles;

        private Dictionary<Transform, SimplePool<Transform>> m_ParentedObjects = new Dictionary<Transform, SimplePool<Transform>>();

        protected int m_MoveTweenId;
        protected int m_AlphaTweenId;
        protected int m_CharacterAlphaTweenId;
        protected float m_CharacterAlphaMul = 1f;

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
            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
            _m_GameObject = base.gameObject;
            m_CharacterRenderers = new SpriteRenderer[] { m_AvatarRenderer };
            m_Particles = GetComponentsInChildren<ParticleSystem>(true);
        }

        public virtual void Setup(Token data)
        {
            m_Data = data;
            m_Type = data.Type;
        }

        public virtual void EnablePortait() { }

        public virtual void EnableAvatar() { }

        public virtual void SetStats(int level) { }

        public virtual void UpdateEnergy(int energy, int baseEnergy) { }


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

                for (int i = 0; i < m_CharacterRenderers.Length; i++)
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

                          for (int i = 0; i < m_CharacterRenderers.Length; i++)
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

            if (time == 0)
            {
                alpha = a;

                Color aux;
                for (int i = 0; i < m_Renderers.Length; i++)
                {
                    aux = m_Renderers[i].color;
                    aux.a = alpha;
                    m_Renderers[i].color = aux;
                }

                for (int i = 0; i < m_CharacterRenderers.Length; i++)
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
                        for (int i = 0; i < m_Renderers.Length; i++)
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


                        for (int i = 0; i < m_CharacterRenderers.Length; i++)
                        {
                            aux = m_CharacterRenderers[i].color;
                            aux.a = alpha * characterAlpha * m_CharacterAlphaMul;
                            m_CharacterRenderers[i].color = aux;
                        }
                    })
                    .setOnComplete(onComplete)
                    .uniqueId;
            }

            if (Mathf.Approximately(a, 0))
            {
                for (int i = 0; i < m_Particles.Length; i++)
                {
                    if (m_Particles[i].isEmitting && m_Particles[i].main.loop)
                        m_Particles[i].Stop(false);
                }
            }
            else
            {
                for (int i = 0; i < m_Particles.Length; i++)
                {
                    if (!m_Particles[i].isEmitting && m_Particles[i].main.loop)
                        m_Particles[i].Play(false);
                }
            }
        }

        public void AddChild(Transform t, Transform parent, SimplePool<Transform> pool)
        {
            m_ParentedObjects.Add(t, pool);

            t.SetParent(parent);
            t.localPosition = new Vector3(0, 0, -0.5f);
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;

            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        }

        public void RemoveChild(Transform t)
        {
            if (m_ParentedObjects.ContainsKey(t))
            {
                m_ParentedObjects[t]?.Despawn(t);
                m_ParentedObjects.Remove(t);
            }

            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
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