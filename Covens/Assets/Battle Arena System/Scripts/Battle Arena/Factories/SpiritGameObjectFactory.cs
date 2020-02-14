using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Marker;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class SpiritGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private BattleSpiritMarker _characterPrefab;

        public override IEnumerator<AbstractCharacterMaker> Create(Transform cellTransform, ICharacterModel character)
        {
            BattleSpiritMarker spiritMarker = Instantiate(_characterPrefab, cellTransform);
            spiritMarker.Init(character);

            yield return spiritMarker;
        }
    }
}
