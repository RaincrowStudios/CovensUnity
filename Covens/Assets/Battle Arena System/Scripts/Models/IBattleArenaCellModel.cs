namespace Raincrow.BattleArena.Model
{
    public interface IBattleArenaCellModel
    {
        int Column { get; }
        int Line { get; }
        int Height { get; }
    }
}