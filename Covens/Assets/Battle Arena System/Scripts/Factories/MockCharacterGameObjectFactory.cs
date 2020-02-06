using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class MockCharacterGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private GameObject _characterPrefab;

        public override IEnumerator Create(GameObject cellInstance)
        {            
            CharacterBuilder builder = new CharacterBuilder();
            ICharacterModel characterModel = new CharacterModel(builder);
            
            yield return Instantiate(_characterPrefab, cellInstance.transform);
        }
    }
}