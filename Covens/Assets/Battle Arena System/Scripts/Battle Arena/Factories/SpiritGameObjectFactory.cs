using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.View;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class SpiritGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        [SerializeField] private BattleSpiritView _battleSpiritViewPrefab;

        public override IEnumerator<AbstractCharacterView> Create(Transform cellTransform, ICharacterModel character)
        {
            BattleSpiritView spiritMarker = Instantiate(_battleSpiritViewPrefab, cellTransform);
            yield return spiritMarker;
        }
    }
}
