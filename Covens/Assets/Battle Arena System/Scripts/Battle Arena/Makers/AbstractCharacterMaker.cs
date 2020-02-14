using UnityEngine;

namespace Raincrow.BattleArena.Marker
{
    public abstract class AbstractCharacterMaker : MonoBehaviour
    {
        public abstract Renderer AvatarRenderer { get; set; }
    }
}
