using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractCharacterGameObjectFactory : MonoBehaviour
    {
        public abstract IEnumerator Create(Transform cellTransform);
    }
}