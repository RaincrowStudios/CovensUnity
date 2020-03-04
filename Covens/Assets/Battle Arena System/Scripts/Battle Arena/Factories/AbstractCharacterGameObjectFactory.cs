using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterGameObjectFactory<T, U> : MonoBehaviour where T : ICharacterModel where U : ICharacterUIModel
    {
        public abstract IEnumerator<ICharacterController<T, U>> Create(Transform cellTransform, T model);
    }
}