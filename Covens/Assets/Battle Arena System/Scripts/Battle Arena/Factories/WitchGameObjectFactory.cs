using Raincrow.BattleArena.Marker;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class WitchGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private AbstractCharacterMaker _characterPrefab;

        public override IEnumerator<AbstractCharacterMaker> Create(Transform cellTransform, ICharacterModel character)
        {
            IWitchModel witchModel = new WitchModel();

            yield return Instantiate(_characterPrefab, cellTransform);
        }
    }
}