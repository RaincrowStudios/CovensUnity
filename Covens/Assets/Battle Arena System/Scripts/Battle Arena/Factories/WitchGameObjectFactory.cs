using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class WitchGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private BattleWitchView _battleWitchViewPrefab;

        public override IEnumerator<AbstractCharacterView> Create(Transform cellTransform, ICharacterModel character)
        {
            IWitchModel witchModel = new WitchModel();
            AbstractCharacterView characterMarker = Instantiate(_battleWitchViewPrefab, cellTransform);
            yield return characterMarker;
        }
    }
}