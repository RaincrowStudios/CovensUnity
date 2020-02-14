using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    [System.Serializable]
    public class CharacterGameObjectModel
    {
        // Serializable variables
        [SerializeField] private GameObject _characterPrefab; // Character Prefab

        public GameObject CharacterPrefab { get => _characterPrefab; set => _characterPrefab = value; }
    }

    public class CharacterModel : ICharacterModel
    {
        public string Id { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int BaseEnergy { get; set; }
        public int Energy { get; set; }
        public int Power { get; set; }
        public int Resilience { get; set; }
        public string CharacterType { get; set; }
        public BattleSlot Slot { get; set; }
        public string Model { get; set; }

        public CharacterModel(CharacterBuilder builder)
        {

        }

    }

    public class WitchModel : IWitchModel
    {
        // Properties

        /// <summary>
        /// Player ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Position X on Grid
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Position Y on Grid
        /// </summary>
        public int Column { get; set; }
        public int BaseEnergy { get; set; }
        public int Energy { get; set; }
        public int Power { get; set; }
        public int Resilience { get; set; }
        public string CharacterType { get; set; }
        public int Degree { get; set; }
        public BattleSlot Slot { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        public string Model { get; set; }

        public WitchModel(CharacterBuilder builder)
        {

        }
    }

    public class SpiritModel : ISpiritModel
    {
        public bool Wild { get; set; }
        public string Id { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int BaseEnergy { get; set; }
        public int Energy { get; set; }
        public int Power { get; set; }
        public int Resilience { get; set; }
        public string CharacterType { get; set; }
        public BattleSlot Slot { get; set; }
        public string Spirit { get; set; }
        public string Model { get; set; }

        public SpiritModel(CharacterBuilder builder)
        {

        }
    }

    public class BattleSlot : IBattleSlot
    {
        public string Id { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }

    public class ItemModel : IItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class CharacterBuilder
    {

    }

}