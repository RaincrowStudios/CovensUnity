using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.View;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterGameObjectFactory<T, U> : MonoBehaviour where T : ICharacterModel where U : ICharacterViewModel
    {
        public abstract IEnumerator<AbstractCharacterView<T, U>> Create(Transform cellTransform, T model);
    }
}