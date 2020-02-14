using Raincrow.BattleArena.View;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterGameObjectFactory : MonoBehaviour
    {
        public abstract IEnumerator<AbstractCharacterView> Create(Transform cellTransform, Model.ICharacterModel characterModel);
    }
}