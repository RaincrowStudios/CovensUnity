using System.Collections.Generic;

namespace Raincrow.BattleArena.Model
{
    public static class ObjectType
    {
        public static readonly string Witch = "witch";
        public static readonly string Spirit = "spirit";
        public static readonly string Item = "item";
    }

    public static class InventoryApparelPosition
    {
        public static readonly string BaseBody = "baseBody";
        public static readonly string BaseHand = "baseHand";
        public static readonly string Hair = "hair";
        public static readonly string Chest = "chest";
        public static readonly string Legs = "legs";
        public static readonly string Feet = "feet";
        public static readonly string Neck = "neck";
        public static readonly string Wrist = "wrist";
        public static readonly string Hands = "hands";
        public static readonly string CarryOn = "carryOn";
        public static readonly string Tattoo = "tattoo";
    }

    public interface ICharacterModel : IObjectModel
    {
        int BaseEnergy { get; }
        int Energy { get; set; }
        int Power { get; set; }
        int Resilience { get; set; }        
    }

    public interface IObjectModel
    {
        string Id { get; }
        string ObjectType { get; }        
    }

    public interface IWitchModel : ICharacterModel
    {
        string Name { get; set; }
        int Degree { get; set; }
        int Level { get; set; }
        InventoryModel Inventory { get; }
    }    

    public interface ISpiritModel : ICharacterModel
    {
        string OwnerId { get; set; }
        string Texture { get; set; }
    }    
}