using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterGameObjectFactory : MonoBehaviour
    {
        public abstract IEnumerator<GameObject> Create(Transform cellTransform, Model.ICharacterModel characterModel);
    }
}