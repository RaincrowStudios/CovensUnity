using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterFactory : MonoBehaviour
    {
        public abstract ICharacterModel Create();
    }
}