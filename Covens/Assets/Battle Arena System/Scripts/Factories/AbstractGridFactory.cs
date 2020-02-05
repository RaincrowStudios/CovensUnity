using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractGridFactory : MonoBehaviour
    {
        public abstract IGridModel Create();
    }
}