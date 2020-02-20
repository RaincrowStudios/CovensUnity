using UnityEngine;

namespace Raincrow.BattleArena.Model
{
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
        CharacterInfo Info { get; set; }
        InventoryModel Inventory { get; set; }
    }

    public interface ISpiritModel : ICharacterModel
    {
        string OwnerId { get; set; }
        string Texture { get; set; }
    }    

    public interface ICharacterViewModel
    {
        Texture Texture { get; set; }
    }

    public interface IWitchViewModel : ICharacterViewModel
    {
        Color AlignmentColor { get; set; }
    }

    public interface ISpiritViewModel : ICharacterViewModel
    {
        
    }
}