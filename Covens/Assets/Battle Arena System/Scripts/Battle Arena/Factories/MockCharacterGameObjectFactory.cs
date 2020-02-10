using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class MockCharacterGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private GameObject _characterPrefab;

        public override IEnumerator<GameObject> Create(Transform cellTransform)
        {            
            CharacterBuilder builder = new CharacterBuilder();
            ICharacterModel characterModel = new CharacterModel(builder);
            
            yield return Instantiate(_characterPrefab, cellTransform);
        }
    }
}