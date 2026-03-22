using ScoundrelGame.Enums;

namespace ScoundrelGame.Classes;

// Card class, represents a card in the game
public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }
    public string Name => $"{Rank} of {Suit}";

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    // Value to perform in-game calculations, like damage and heals
    public int Value => (int)Rank;

    public CardType Type
    {
        get
        {
            if (Suit == Suit.Hearts)
                return CardType.Health;
            if (Suit == Suit.Diamonds)
                return CardType.Weapon;
            return CardType.Monster;
        }
    }
}