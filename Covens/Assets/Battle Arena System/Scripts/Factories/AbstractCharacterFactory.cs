using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterModelFactory : MonoBehaviour
    {
        public abstract ICharacterModel Create();
    }
}