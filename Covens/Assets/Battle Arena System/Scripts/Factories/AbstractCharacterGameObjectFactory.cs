using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterGameObjectFactory : MonoBehaviour
    {
        public abstract GameObject Create(GameObject cellInstance);
    }
}