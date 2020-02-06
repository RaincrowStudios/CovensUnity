using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class MockCharacterGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private GameObject _characterPrefab;

        public override GameObject Create(GameObject cellInstance)
        {            
            CharacterBuilder builder = new CharacterBuilder();
            ICharacterModel characterModel = new CharacterModel(builder);
            
            return Instantiate(_characterPrefab, cellInstance.transform);
        }
    }
}