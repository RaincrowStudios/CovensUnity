using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMarker
    {
        GameObject gameObject { get; }
        object customData { get; set; }
        Vector2 position { get; set; }
        bool inMapView { get; set; }
        bool interactable { get; set; }
        System.Action<IMarker> OnClick { get; set; }
        void SetPosition(double lng, double lat);

        bool IsShowingIcon { get; }
        bool IsShowingAvatar { get; }
        void Setup(Token data);
        void EnablePortaitIcon();
        void EnableAvatar();
        void SetStats(int level, int energy);
        void SetAlpha(float t);
        void SetTextAlpha(float a);
        void SetupAvatar(bool male, List<EquippedApparel> equips);
        void SetupAvatarAndPortrait(bool male, List<EquippedApparel> equips);

        Transform characterTransform { get; }
    }
}
