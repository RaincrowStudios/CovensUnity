namespace Raincrow.BattleArena.Model
{
    public static class ObjectType
    {
        public static readonly string Witch = "witch";
        public static readonly string Spirit = "spirit";
        public static readonly string Item = "item";
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
    }

    public interface ISpiritModel : ICharacterModel
    {
        string OwnerId { get; set; }
        string Texture { get; set; }
    }

    public interface IInventoryItemModel
    {
        string Id { get; set; }
        string Name { get; set; }
        int Count { get; set; }
    }    
}