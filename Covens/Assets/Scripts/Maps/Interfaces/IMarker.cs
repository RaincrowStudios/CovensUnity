using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMarker
    {
        GameObject GameObject { get; }
        Vector2 Coords { get; set; }
        bool inMapView { get; set; }
        bool Interactable { get; set; }
        System.Action<IMarker> OnClick { get; set; }
        MarkerSpawner.MarkerType Type { get; }
        Token Token { get; }
        bool IsPlayer { get; }
        string Name { get; }

        bool IsShowingIcon { get; }
        bool IsShowingAvatar { get; }
        void Setup(Token data);
        void EnablePortait();
        void EnableAvatar();
        void SetStats();
        void UpdateEnergy();
        void ScaleNamePlate(bool scaleUp, float time = 1);
        void EnergyRingFade(float a, float time = 0);
        void SetCharacterAlpha(float t, float time = 0, System.Action onComplete = null);
        void SetTextAlpha(float a);
        void SetAlpha(float a, float time = 0, System.Action onComplete = null);
        void InitializePositionPOP();
        void EnablePopSorting();
        void SetWorldPosition(Vector3 worldPos, float time = 0, System.Action onComplete = null);
        void ApplyStatusEffect(StatusEffect effect);
        void ExpireStatusEffect(StatusEffect effect);
        Transform SpawnItem(SimplePool<Transform> pool);
        Transform AvatarTransform { get; }

        //void AddChild(Transform t, Transform parent, SimplePool<Transform> pool);
        //void RemoveChild(Transform t);
        //void SpawnFX(SimplePool<Transform> fxPool, bool character, float duration, bool queued, System.Action<Transform> onSpawn);
        bool isNull { get; }
        void OnDespawn();
        void UpdateRenderers();
    }
}
