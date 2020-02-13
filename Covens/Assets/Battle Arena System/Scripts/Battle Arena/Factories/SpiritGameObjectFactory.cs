using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class SpiritGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private GameObject _characterPrefab;

        public override IEnumerator<GameObject> Create(Transform cellTransform, ICharacterModel character)
        {
            CharacterBuilder builder = new CharacterBuilder();
            ISpiritModel spiritModel = new SpiritModel(builder);

            yield return Instantiate(_characterPrefab, cellTransform);
        }
    }
}
