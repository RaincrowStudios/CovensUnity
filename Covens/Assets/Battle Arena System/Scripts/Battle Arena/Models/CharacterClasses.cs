using Newtonsoft.Json;
using System.Collections.Generic;
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
        public InventoryModel Inventory { get; set; }

        public WitchModel()
        {

        }
    }

    public class SpiritModel : ISpiritModel
    {        
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

    public struct InventoryItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public struct InventoryApparelModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
    }

    public class InventoryModel
    {        
        public List<InventoryItemModel> Items { get; }
        [JsonProperty("equipped")] public List<InventoryApparelModel> Apparels { get; }
    }
}