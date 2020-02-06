using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractGridGameObjectFactory : MonoBehaviour
    {
        public abstract GameObject[,] Create();
    }
}