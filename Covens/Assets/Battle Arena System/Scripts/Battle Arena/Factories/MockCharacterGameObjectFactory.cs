using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class MockCharacterGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private AbstractCharacterView _characterPrefab;

        public override IEnumerator<AbstractCharacterView> Create(Transform cellTransform, ICharacterModel character)
        {            
            ICharacterModel characterModel = new CharacterModel();
                        
            yield return Instantiate(_characterPrefab, cellTransform);
        }
    }
}