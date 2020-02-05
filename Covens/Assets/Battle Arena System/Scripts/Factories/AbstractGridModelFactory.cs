using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractGridModelFactory : MonoBehaviour
    {
        public abstract IGridModel Create(AbstractCharacterModelFactory characterFactory);
    }
}