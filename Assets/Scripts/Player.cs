using System.Collections.Generic;

namespace Splendor;

public class Player
{
    public int VictoryPoints { get; set; }
    private int[] resources = new int[6];
    private int[] cardResources = new int[5];
    private List<Card> reservedCards = new List<Card>(3);

    public int[] Resources => resources;
    public int[] CardResources => cardResources;
    public List<Card> ReservedCards => reservedCards;
}