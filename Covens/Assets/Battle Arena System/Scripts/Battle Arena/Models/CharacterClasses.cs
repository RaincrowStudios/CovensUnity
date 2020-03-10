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

    //public class CharacterModel : ICharacterModel
    //{
    //    public string Id { get; set; }
    //    public int BaseEnergy { get; set; }
    //    public int Energy { get; set; }
    //    public int Power { get; set; }
    //    public int Resilience { get; set; }
    //    public string ObjectType { get; set; }

    //    public CharacterModel()
    //    {

    //    }

    //    public Color GetAligmentColor()
    //    {

    //    }
    //}

    public class WitchModel : IWitchModel, ICloneable<IWitchModel>
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
        public BattleSlot? BattleSlot { get; set; }

        public WitchModel()
        {
            Inventory = new InventoryModel();
            Info = new CharacterInfo();
        }

        public IWitchModel Clone()
        {
            WitchModel destination = new WitchModel()
            {
                Id = Id,
                BaseEnergy = BaseEnergy,
                Energy = Energy,
                Power = Power,
                Resilience = Resilience,
                ObjectType = ObjectType,
                Degree = Degree,
                Name = Name,
                Level = Level,
                Inventory = Inventory.Clone(),
                Info = Info.Clone()
            };

            if (BattleSlot.HasValue)
            {
                destination.BattleSlot = BattleSlot.Value.Clone();
            }
            return destination;
        }

        public Color GetAlignmentColor()
        {
            if (Degree > 0)
            {
                return Utilities.Orange;
            }
            else if (Degree < 0)
            {
                return Utilities.Purple;
            }
            return Utilities.Blue;
        }
    }

    public class SpiritModel : ISpiritModel, ICloneable<ISpiritModel>
    {
        public string Id { get; set; }
        public int BaseEnergy { get; set; }
        public int Energy { get; set; }
        public int Power { get; set; }
        public int Resilience { get; set; }
        public string ObjectType { get; set; }
        public string Texture { get; set; }
        public string OwnerId { get; set; }
        public BattleSlot? BattleSlot { get; set; }

        public SpiritModel() { }

        public ISpiritModel Clone()
        {
            SpiritModel destination = new SpiritModel()
            {
                Id = Id,
                BaseEnergy = BaseEnergy,
                Energy = Energy,
                Power = Power,
                Resilience = Resilience,
                ObjectType = ObjectType,
                Texture = Texture,
                OwnerId = OwnerId,
            };

            if (BattleSlot.HasValue)
            {
                destination.BattleSlot = BattleSlot.Value.Clone();
            }
            return destination;
        }

        public Color GetAlignmentColor()
        {
            return Color.white;
        }
    }

    public struct BattleSlot : ICloneable<BattleSlot>
    {
        [JsonProperty("row")]
        public int Row { get; set; }
        [JsonProperty("col")]
        public int Col { get; set; }

        public BattleSlot Clone()
        {
            BattleSlot clone = new BattleSlot()
            {
                Row = Row,
                Col = Col
            };
            return clone;
        }
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
        public string Position { get; set; }
        public string[] Assets { get; set; }
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

    public struct CharacterInfo : ICloneable<CharacterInfo>
    {
        [JsonProperty("male")] public bool Gender { get; set; }
        public int BodyType { get; set; }

        public CharacterInfo Clone()
        {
            CharacterInfo destination = new CharacterInfo()
            {
                Gender = Gender,
                BodyType = BodyType
            };
            return destination;
        }
    }

    public class InventoryModel : ICloneable<InventoryModel>
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

        public InventoryModel Clone()
        {
            InventoryModel destination = new InventoryModel();
            destination.Tools.AddRange(Tools);
            destination.Herbs.AddRange(Herbs);
            destination.Gems.AddRange(Gems);
            destination.Consumables.AddRange(Consumables);
            destination.Equipped.AddRange(Equipped);
            return destination;
        }
    }

    public class WitchUIModel : IWitchUIModel
    {
        public Texture Texture { get; set; }
        public Material AlignmentMaterial { get; set; }
        public Transform Transform { get; private set; }

        public WitchUIModel(Transform transform)
        {
            Transform = transform;
        }
    }

    public class SpiritUIModel : ISpiritUIModel
    {
        public Texture Texture { get; set; }
        public Material AlignmentMaterial { get; set; }
        public Transform Transform { get; private set; }

        public SpiritUIModel(Transform transform)
        {
            Transform = transform;
        }
    }
}