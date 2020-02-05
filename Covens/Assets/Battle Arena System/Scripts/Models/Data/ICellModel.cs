namespace Raincrow.BattleArena.Model
{
    public interface ICellModel
    {
        int Column { get; }
        int Line { get; }
        int Height { get; }
    }
}