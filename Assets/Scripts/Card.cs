namespace Splendor;

public class Card(int _victoryPoints, ResourceType _resource, int[] _costs)
{
    public int VictoryPoints { get; } = _victoryPoints;
    public ResourceType Resource { get; } = _resource;
    public int[] Costs { get; } = _costs;
}