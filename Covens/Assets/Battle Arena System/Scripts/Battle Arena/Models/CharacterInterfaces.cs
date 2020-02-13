namespace Raincrow.BattleArena.Model
{
    public enum CharacterType
    {
        character,
        spirit
    }

    public interface ICharacterModel
    {
        string Id { get;  }
        BattleSlot Slot { get; }
        int BaseEnergy { get; }
        int Energy { get; set; }
        int Power { get; set; }
        int Resilience { get; set; }
        CharacterType Type { get; }
        string Model { get; }
    }

    public interface IBattleSlot
    {
        string Id { get; }
        int Row { get; }
        int Col { get; }
    }

    public interface IWitchModel : ICharacterModel
    {
        string Name { get; set; }
        int Degree { get; set; }
        int Level { get; set; }
    }

    public interface ISpiritModel : ICharacterModel
    {
        bool Wild { get; set; }
        string Spirit { get; }
    }

    public interface IItemModel
    {
        string Id { get; set; }
        string Name { get; set; }
        int Count { get; set; }
    }    
}