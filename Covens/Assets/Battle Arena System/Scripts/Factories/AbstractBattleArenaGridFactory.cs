using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractBattleArenaGridFactory : MonoBehaviour
    {
        public abstract IBattleArenaGridModel Create();
    }
}