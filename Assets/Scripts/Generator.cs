using System;
using System.IO;

namespace Splendor;

class Generator
{
    static System.Random rand = new System.Random();

    private static readonly Card[] ALL_LEVEL_ONE_CARDS = new Card[(int)DeckSize.LevelOne];
    private static readonly Card[] ALL_LEVEL_TWO_CARDS = new Card[(int)DeckSize.LevelTwo];
    private static readonly Card[] ALL_LEVEL_THREE_CARDS = new Card[(int)DeckSize.LevelThree];
    private static readonly Noble[] ALL_NOBLES = new Noble[(int)DeckSize.Nobles];

    static Generator()
    {
        const string cardDataFile = "Assets/Resources/CardData.csv";

        using (StreamReader reader = new StreamReader(cardDataFile))
        {
            if (!reader.EndOfStream)
            {
                reader.ReadLine();
            }

            int idxLevelOne = 0;
            int idxLevelTwo = 0;
            int idxLevelThree = 0;
            int idxNobles = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] fields = line.Split(',');

                int victoryPoints = int.Parse(fields[1]);
                Enum.TryParse<ResourceType>(fields[2], out ResourceType resource);
                int[] costs = [int.Parse(fields[2]), int.Parse(fields[3]), int.Parse(fields[4]), int.Parse(fields[5]), int.Parse(fields[6])];

                switch (fields[0])
                {
                    case "Level One":
                        ALL_LEVEL_ONE_CARDS[idxLevelOne++] = new Card(victoryPoints, resource, costs);
                        break;
                    case "Level Two":
                        ALL_LEVEL_TWO_CARDS[idxLevelTwo++] = new Card(victoryPoints, resource, costs);
                        break;
                    case "Level Three":
                        ALL_LEVEL_THREE_CARDS[idxLevelThree++] = new Card(victoryPoints, resource, costs);
                        break;
                    case "Noble":
                        ALL_NOBLES[idxNobles++] = new Noble(victoryPoints, costs);
                        break;
                }
            }
        }
    }

    private static int[] FisherYates(int n)
    {
        int[] result = new int[n];
        for (int i = 0; i < n; i++)
        {
            result[i] = i;
        }

        for (int i = n - 1; i >= 1; i--)
        {
            int j = rand.Next(i + 1);
            int temp = result[j];
            result[j] = result[i];
            result[i] = temp;
        }

        return result;
    }

    public static void InitNobles(out Noble[] nobles)
    {
        int n = (int)DeckSize.Nobles;
        int idxOne = rand.Next(n);
        int idxTwo = rand.Next(n);
        while (idxTwo == idxOne)
        {
            idxTwo = rand.Next(n);
        }
        int idxThree = rand.Next(n);
        while (idxThree == idxOne || idxThree == idxTwo)
        {
            idxThree = rand.Next(n);
        }

        nobles = new Noble[3];

        nobles[0] = ALL_NOBLES[idxOne];
        nobles[1] = ALL_NOBLES[idxTwo];
        nobles[2] = ALL_NOBLES[idxThree];
    }

    public static void InitCards(out Card[] levelOneCards, out Card[] levelTwoCards, out Card[] levelThreeCards)
    {
        levelOneCards = InitLevelOneCards();
        levelTwoCards = InitLevelTwoCards();
        levelThreeCards = InitLevelThreeCards();
    }

    private static Card[] InitLevelOneCards()
    {
        int n = (int)DeckSize.LevelOne;
        int[] idx = FisherYates(n);
        Card[] result = new Card[n];

        for (int i = 0; i < n; i++)
        {
            result[i] = ALL_LEVEL_ONE_CARDS[idx[i]];
        }

        return result;
    }

    private static Card[] InitLevelTwoCards()
    {
        int n = (int)DeckSize.LevelTwo;
        int[] idx = FisherYates(n);
        Card[] result = new Card[n];

        for (int i = 0; i < n; i++)
        {
            result[i] = ALL_LEVEL_TWO_CARDS[idx[i]];
        }

        return result;
    }

    private static Card[] InitLevelThreeCards()
    {
        int n = (int)DeckSize.LevelThree;
        int[] idx = FisherYates(n);
        Card[] result = new Card[n];

        for (int i = 0; i < n; i++)
        {
            result[i] = ALL_LEVEL_THREE_CARDS[idx[i]];
        }

        return result;
    }
}