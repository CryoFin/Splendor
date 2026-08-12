using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Splendor;

public struct Move(int _victoryPoints, int[] _resources, Card _card, Noble _noble, int id)
{
    private static System.Random rand = new System.Random();
    private static readonly Dictionary<byte, int[]> rawDoubleData = new()
    {
        {0, [2, 0, 0, 0, 0, 0]},
        {32, [0, 2, 0, 0, 0, 0]},
        {96, [0, 0, 2, 0, 0, 0]},
        {64, [0, 0, 0, 2, 0, 0]},
        {128, [0, 0, 0, 0, 2, 0]}
    };

    public static readonly ReadOnlyDictionary<byte, int[]> DoubleDictionary = new(rawDoubleData);

    public readonly int VictoryPoints { get; } = _victoryPoints;
    public readonly int[] Resources { get; } = _resources;
    public readonly Card card { get; } = _card;
    public readonly Noble noble { get; } = _noble;
    //Resource Changes are represented by POV of player
    // + signifies player gain
    // - signifies player loss (return to board)

    public static Move GenerateRandomMove(ref Player currentPlayer, ref int[] resourceBank, ref Card[] levelOneCards, ref Card[] levelTwoCards, ref Card[] levelThreeCards, ref Noble[] nobles)
    {
        int totalCardMoves = 0;
        int totalReservedCardMoves = 0;
        int totalReservationMoves = (currentPlayer.Resources.Sum() < 10 && currentPlayer.ReservedCards.Count < 3) ? 15 : 0;
        int totalResourceMoves = (currentPlayer.Resources.Sum() <= 8) ? resourceBank.Count(n => n >= 4) : 0;

        List<int> validCardMoves = new List<int>(12);

        for (int i = 0; i < 12; i++)
        {
            Card candidateCard = i switch
            {
                >= 0 and < 4 => levelOneCards[i],
                >= 4 and < 8 => levelTwoCards[i % 4],
                >= 8 and < 12 => levelThreeCards[i % 4]
            };
            int difference = 0;
            for (int j = 0; j < 5; j++)
            {
                difference += System.Math.Max(0, candidateCard.Costs[j] - currentPlayer.Resources[j] - currentPlayer.CardResources[j]);
            }
            if (difference < currentPlayer.Resources[5])
            {
                totalCardMoves++;
                validCardMoves.Add(i);
            }
        }

        List<int> validReservedCardMoves = new List<int>(3);

        for (int j = 0; j < currentPlayer.ReservedCards.Count; j++)
        {
            Card candidateCard = currentPlayer.ReserverdCards[j];

            int difference = 0;
            for (int i = 0; i < 5; i++)
            {
                difference += System.Math.Max(0, candidateCard.Costs[i] - currentPlayer.Resources[i] - currentPlayer.CardResources[i]);
            }
            if (difference > currentPlayer.Resources[5])
            {
                totalReservedCardMoves++;
                validReservedCardMoves.Add(j);
            }
        }

        int n = resourceBank.Take(5).Count(n => n > 0);
        int r = System.Math.Min(3, System.Math.Max(10 - currentPlayer.Resources.Sum(), 0));
        int nCr = nCr(n, r);
        totalResourceMoves += nCr;

        int randomMoveIndex = rand.Next(totalCardMoves + totalReservedCardMoves + totalReservationMoves + totalResourceMoves);

        switch (randomMoveIndex)
        {
            case >= 0 and < totalCardMoves:
                int cardIndex = validCardMoves[randomMoveIndex];
                Card card = cardIndex switch
                {
                    >= 0 and < 4 => levelOneCards[cardIndex % 4],
                    >= 4 and < 8 => levelTwoCards[cardIndex % 4],
                    >= 8 and < 12 => levelThreeCards[cardIndex % 4]
                };
                return GenerateCardMove(currentPlayer, card, cardIndex);
            case >= totalCardMoves and < totalCardMoves + totalReservedCardMoves:
                int cardIndex = validReservedCardMoves[randomMoveIndex - totalCardMoves];
                Card card = currentPlayer.ReservedCards[cardIndex];
                return GenerateCardMove(currentPlayer, card, cardIndex + 12);
            case >= totalCardMoves + totalReservedCardMoves and < totalCardMoves + totalReservedCardMoves + totalReservationMoves:
                int cardIndex = randomMoveIndex - totalCardMoves - totalReservedCardMoves;
                card card = cardIndex switch
                {
                    >= 0 and < 4 => levelOneCards[cardIndex % 4],
                    >= 4 and < 8 => levelTwoCards[cardIndex % 4],
                    >= 8 and < 12 => levelThreeCards[cardIndex % 4],
                    _ => null
                };
                return GenerateReservationMove(currentPlayer, card, cardIndex);
            case >= totalCardMoves + totalReservedCardMoves + totalReservationMoves and < totalCardMoves + totalReservedCardMoves + totalReservationMoves + totalResourceMoves:
        }
    }

    private static Move GenerateCardMove(Player currentPlayer, Card card, int id)
    {
        int[] resources = new int[6];

        int difference = 0;
        for (int i = 0; i < 5; i++)
        {
            int currentGemPayout = card.Costs[i] - currentPlayer.CardResources[i];
            int currentGemDifference = currentPlayer.Resources[i] - currentGemPayout;
            if (currentGemDifference < 0)
            {
                difference += currentGemDifference;
                currentGemPayout += currentGemDifference;
            }
            resources[i] = -currentGemPayout;
        }
        resources[5] = -difference;

        return new Move(card.VictoryPoints, resources, card, null, id);
    }

    private static Move GenerateReservationMove(Player currentPlayer, Card card, int id)
    {
        int[] resources = [0, 0, 0, 0, 0, 1];
        return new Move(0, resources, card, null, id);
    }

    private static Move GenerateRandomResourceMove(Player currentPlayer, ref int[] resourceBank, int totalResourceMoves, int nCr, int r)
    {
        List<byte> validResourceMoves = new List<byte>(totalResourceMoves);
        for (int i = 0; i < 5; i++)
        {
            if (resourceBank[i] == 0)
            {
                continue;
            }
            if (resourceBank[i] >= 4)
            {
                byte id = i << 5;
                validResourceMoves.Add(id);
            }
            if (r == 1)
            {
                byte id = 1 << (4 - i);
                validResourceMoves.Add(id);
            }
            else if (r == 2 && i < 4)
            {
                byte tempId = 1 << (4 - i);
                for (int j = i + 1; j < 5; j++)
                {
                    if (resourceBank[i] == 0)
                    {
                        continue;
                    }
                    byte id = tempId | (1 << (4 - j));
                    validResourecMoves.add(id);
                }
            }
            else if (r == 3 && i < 3)
            {
                byte tempIdOne = 1 << (4 - i);
                for (int j = i + 1; j < 4; j++)
                {
                    if (resourceBank[j] == 0)
                    {
                        continue;
                    }
                    byte tempIdTwo = tempIdOne | (1 << (4 - j));
                    for (int k = j + 1; k < 5; k++)
                    {
                        if (resourceBank[i] == 0)
                        {
                            continue;
                        }
                        byte id = tempIdTwo | (1 << (4 - k));
                        validResourceMoves.Add(id);
                    }
                }
            }
        }

        int randomMoveIndex = rand.Next(totalResourceMoves);
        byte id = validResourceMoves[randomMoveIndex];
        int[] resources =
        return new Move(0, async, null, null, id);
    }

    private static int nCr(int n, int r)
    {
        if (r * 2 < n)
        {
            r = n - r;
        }

        int numerator = 1;
        for (int i = n; i > r; i--)
        {
            numerator *= i;
        }

        int denominator = 1;
        for (int i = 1; i <= r; i++)
        {
            denominator *= i;
        }

        return numerator / denominator;
    }
}