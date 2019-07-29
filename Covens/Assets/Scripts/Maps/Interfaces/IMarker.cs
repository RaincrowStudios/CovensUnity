using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMarker
    {
        GameObject gameObject { get; }
        object customData { get; set; }
        Vector2 coords { get; set; }
        bool inMapView { get; set; }
        bool interactable { get; set; }
        System.Action<IMarker> OnClick { get; set; }
        MarkerSpawner.MarkerType type { get; }
        Token token { get; }
        bool IsPlayer { get; }

        bool IsShowingIcon { get; }
        bool IsShowingAvatar { get; }
        void Setup(Token data);
        void EnablePortait();
        void EnableAvatar();
        void SetStats();
        void UpdateEnergy();
        void SetCharacterAlpha(float t, float time = 0, System.Action onComplete = null);
        void SetTextAlpha(float a);
        void SetAlpha(float a, float time = 0, System.Action onComplete = null);
        void SetWorldPosition(Vector3 worldPos, float time = 0, System.Action onComplete = null);
        Transform characterTransform { get; }

        //void AddChild(Transform t, Transform parent, SimplePool<Transform> pool);
        //void RemoveChild(Transform t);
        //void SpawnFX(SimplePool<Transform> fxPool, bool character, float duration, bool queued, System.Action<Transform> onSpawn);
        bool isNull { get; }
        void OnDespawn();
        void UpdateRenderers();
    }
}
