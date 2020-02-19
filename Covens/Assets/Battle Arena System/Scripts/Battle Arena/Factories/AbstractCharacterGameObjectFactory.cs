using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.View;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterGameObjectFactory<T> : MonoBehaviour where T : ICharacterModel
    {
        public abstract IEnumerator<AbstractCharacterView<T>> Create(Transform cellTransform, T model);
    }
}