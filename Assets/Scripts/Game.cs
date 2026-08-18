namespace Splendor;

public class Game
{
    private Card[] levelOneCards;
    private Card[] levelTwoCards;
    private Card[] levelThreeCards;
    private Card[] levelOneCardsOut = new Card[4];
    private Card[] levelTwoCardsOut = new Card[4];
    private Card[] levelThreeCardsOut = new Card[4];

    private Noble[] nobles;
    private int[] resourceBank = new int[6];

    private readonly int NUM_PLAYERS;
    private Player[] players;
    private int currentPlayer;

    public Game(int _numPlayers)
    {
        NUM_PLAYERS = _numPlayers;
        players = new Player[NUM_PLAYERS];

        Generator.InitCards(out levelOneCards, out levelTwoCards, out levelThreeCards);
        Generator.InitNobles(out nobles);
    }
}