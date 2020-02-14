using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class WitchGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private GameObject _characterPrefab;

        public override IEnumerator<GameObject> Create(Transform cellTransform, ICharacterModel character)
        {
            IWitchModel witchModel = new WitchModel();

            yield return Instantiate(_characterPrefab, cellTransform);
        }
    }
}