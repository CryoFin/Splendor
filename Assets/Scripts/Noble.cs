namespace Splendor;

public class Noble(int _victoryPoints, int[] _requirements)
{
    public int VictoryPoints { get; } = _victoryPoints;
    public int[] Requirements { get; } = _requirements;
}