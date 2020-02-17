using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public static class ObjectType
    {
        public static readonly string Witch = "character";
        public static readonly string Spirit = "spirit";
        public static readonly string Item = "item";
    }

    public static class InventoryApparelPosition
    {
        public static readonly string None = "none";
        public static readonly string Base = "base";
        public static readonly string Head = "head";
        public static readonly string Hair = "hair";
        public static readonly string Chest = "chest";
        public static readonly string Neck = "neck";
        public static readonly string WristLeft = "wristLeft";
        public static readonly string WristRight = "wristRight";
        public static readonly string Hands = "hands";
        public static readonly string FingerRight = "fingerRight";
        public static readonly string FingerLeft = "fingerLeft";
        public static readonly string Waist = "waist";
        public static readonly string Legs = "legs";
        public static readonly string Feet = "feet";
        public static readonly string CarryOnRight = "carryOnRight";
        public static readonly string CarryOnLeft = "carryOnLeft";
        public static readonly string SkinFace = "skinFace";
        public static readonly string SkinShoulder = "skinShoulder";
        public static readonly string SkinChest = "skinChest";
        public static readonly string SkinArm = "skinArm";
        public static readonly string SpecialSlot = "specialSlot";
        public static readonly string BaseBody = "baseBody";
        public static readonly string BaseHand = "baseHand";
    }

    public static class CharacterGender
    {
        public static readonly bool Male = true;
        public static readonly bool Female = false;
    }

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
        public CharacterInfo Info { get; set; }

        public WitchModel()
        {
            Inventory = new InventoryModel();
            Info = new CharacterInfo();
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

        public SpiritModel() { }
    }

    public struct BattleSlot
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }

    public class InventoryItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class InventoryApparelModel
    {
        public string Id { get; set; }
        public string Position { get; set; }
        public string[] Assets { get; set; }

        public InventoryApparelModel()
        {
            Assets = new string[0];
        }
    }

    public class AssetsApparelModel
    {
        public string[] BaseAsset { get; set; }
        public string[] Grey { get; set; }
        public string[] Shadow { get; set; }
        public string[] White { get; set; }

        public AssetsApparelModel()
        {
            BaseAsset = new string[0];
            Grey = new string[0];
            Shadow = new string[0];
            White = new string[0];
        }
    }

    public struct CharacterInfo
    {
        [JsonProperty("male")] public bool Gender { get; set; }
        public int BodyType { get; set; }
    }

    public class InventoryModel
    {
        public List<InventoryItemModel> Tools { get; }
        public List<InventoryItemModel> Herbs { get; }
        public List<InventoryItemModel> Gems { get; }
        public List<InventoryItemModel> Consumables { get; }
        [JsonProperty("equipped")] public List<InventoryApparelModel> Equipped { get; }

        public InventoryModel()
        {
            Tools = new List<InventoryItemModel>();
            Herbs = new List<InventoryItemModel>();
            Gems = new List<InventoryItemModel>();
            Consumables = new List<InventoryItemModel>();
            Equipped = new List<InventoryApparelModel>();
        }
    }
}