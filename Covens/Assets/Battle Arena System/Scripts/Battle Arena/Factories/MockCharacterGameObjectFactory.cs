using Raincrow.BattleArena.Marker;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class MockCharacterGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private AbstractCharacterMaker _characterPrefab;

        public override IEnumerator<AbstractCharacterMaker> Create(Transform cellTransform, ICharacterModel character)
        {            
            ICharacterModel characterModel = new CharacterModel();
            
            yield return Instantiate(_characterPrefab, cellTransform);
        }
    }
}