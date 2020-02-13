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
        int Line { get; }
        int Column { get; }
        int BaseEnergy { get; }
        int Energy { get; set; }
        int Power { get; set; }
        int Resilience { get; set; }
        CharacterType Type { get; }
    }

    public interface IWitchModel : ICharacterModel
    {
        
}

    public interface ISpiritModel : ICharacterModel
    {

    }

    public interface IItemModel
    {
        string Id { get; set; }
        string Name { get; set; }
        int Count { get; set; }
    }    
}