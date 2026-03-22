namespace ScoundrelGame.Classes;

// Room class, represents a room of the dungeon
public class Room
{
    public List<Card> cards { get; private set; } = new List<Card>();
    public int MaxRoomSize => 4;

    // A room consistis of 4 cards in the original game
    public void DrawRoom(Deck dungeon)
    {
        while (cards.Count < MaxRoomSize && dungeon.CardsRemaining > 0)
        {
            cards.Add(dungeon.DrawCard());
        }
    }
}