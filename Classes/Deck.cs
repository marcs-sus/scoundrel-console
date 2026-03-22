using ScoundrelGame.Enums;

namespace ScoundrelGame.Classes;

// Deck class, represents a deck of cards, called the Dungeon
public class Deck
{
    private List<Card> _cards;
    private static readonly Random rng = new Random();

    public Deck()
    {
        _cards = new List<Card>();
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                // Don't include faces and the ace of Hearts
                if (suit == Suit.Hearts && rank > Rank.Ten)
                    continue;

                _cards.Add(new Card(suit, rank));
            }
        }
    }

    // Shuffle the deck based on the Fisher-Yates algorithm
    public void Shuffle()
    {
        int n = _cards.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card temp = _cards[k];
            _cards[k] = _cards[n];
            _cards[n] = temp;
        }
    }

    // Draw a card from the deck pile
    public Card DrawCard()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("Empty deck!");

        Card card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void AddCard(Card card)
    {
        _cards.Add(card);
    }

    public int CardsRemaining => _cards.Count;

    // Print the entire deck, used for !debugging!
    public void PrintDeck()
    {
        foreach (Card card in _cards)
        {
            Console.WriteLine(card.Name);
        }
    }
}