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
        public int BaseEnergy { get; set; }
        public int Energy { get; set; }
        public int Power { get; set; }
        public int Resilience { get; set; }
        public string ObjectType { get; set; }        
        //public string Texture { get; set; }

        public CharacterModel()
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
        public int BaseEnergy { get; set; }
        public int Energy { get; set; }
        public int Power { get; set; }
        public int Resilience { get; set; }
        public string ObjectType { get; set; }
        public int Degree { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        public WitchModel()
        {

        }
    }

    public class SpiritModel : ISpiritModel
    {
        public bool Wild { get; set; }
        public string Id { get; set; }
        public int BaseEnergy { get; set; }
        public int Energy { get; set; }
        public int Power { get; set; }
        public int Resilience { get; set; }
        public string ObjectType { get; set; }                
        public string Texture { get; set; }
        public string OwnerId { get; set; }

        public SpiritModel()
        {

        }
    }

    public struct BattleSlot
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }

    public struct InventoryItemModel : IInventoryItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}