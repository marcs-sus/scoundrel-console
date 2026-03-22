using ScoundrelGame.Enums;

namespace ScoundrelGame.Classes;

// Game class, manages the core game logic
public class Game
{
    public Deck Dungeon { get; private set; }
    public List<Card> DiscardPile { get; private set; }
    public Player Player { get; private set; }
    public Room CurrentRoom { get; private set; }

    public bool previousRoomAvoided { get; private set; } = false;
    public bool isGameOver { get; private set; } = false;

    public Game()
    {
        Dungeon = new Deck();
        Dungeon.Shuffle();
        DiscardPile = new List<Card>();
        Player = new Player();
        CurrentRoom = new Room();
    }

    // Avoid the current room, and add all cards to the bottom of the deck
    private void AvoidRoom()
    {
        foreach (Card card in CurrentRoom.cards)
        {
            Dungeon.AddCard(card);
        }
        CurrentRoom.cards.Clear();
    }

    #region Console Display
    // Display the current game state, printing all the information
    public void DisplayGame()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("========== SCOUNDREL ==========\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Health          : {Player.Health}/{Player.MaxHealth}");
        Console.WriteLine($"Equipped Weapon : {(Player.EquippedWeapon != null ? Player.EquippedWeapon.Name : "None")}" +
            $"{(Player.LastSlainMonsterValue.HasValue ? " (< " + Player.LastSlainMonsterValue.Value + ")" : "")}");
        Console.WriteLine($"Dungeon Cards   : {Dungeon.CardsRemaining}");
        Console.WriteLine();
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("---- Current Room ----");
        for (int i = 0; i < CurrentRoom.cards.Count; i++)
        {
            Card card = CurrentRoom.cards[i];
            Console.WriteLine($"{i + 1}. {card.Name} | Type: {card.Type} | Value: {card.Value}");
        }
        Console.WriteLine("----------------------\n");
        Console.ResetColor();
    }
    #endregion

    #region Tutorial Display
    // Display the tutorial, if requested by the player
    public void PrintTutorial()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n********** SCOUNDREL TUTORIAL **********\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Setup:");
        Console.WriteLine("  - Jokers, Red Face Cards, and Red Ace are not present in the deck.");
        Console.WriteLine("  - The deck, in which we call the Dungeon, is shuffled at the start of the game.");
        Console.WriteLine("  - Start with 20 Health. Your health may not go past 20.\n");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Rules:");
        Console.WriteLine("  - The Clubs and Spades represent Monsters. Their damage equals the card's value (2 to Ace = 14).");
        Console.WriteLine("  - The Diamonds are Weapons. When picked up, you equip them (discarding any previous weapon).");
        Console.WriteLine("  - The Hearts are Health Potions. Use one per turn to restore health.");
        Console.WriteLine("  - Each turn is compose of 4 cards, called a Room, which are drawn from the Dungeon.");
        Console.WriteLine("    * You may choose to avoid the Room (cannot avoid two in a row), placing all 4 cards at the bottom of the Dungeon.");
        Console.WriteLine("    * Otherwise, select 3 cards from the Room to interact with; the remaining card carries over to the next turn.\n");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Combat:");
        Console.WriteLine("  - When a Monster is interacted with, you may fight it barehanded or with your equipped Weapon.");
        Console.WriteLine("    * Barehanded: You take damage equal to the monster's full value.");
        Console.WriteLine("    * With Weapon: Subtract your weapon's value from the monster's value.");
        Console.WriteLine("       - Any remaining damage reduces your Health.");
        Console.WriteLine("       - Once used, a weapon can only defeat monsters with a lower value than the last one slain.\n");
        Console.WriteLine("           * The last slain monster's value is displayed right next to the equipped weapon.\n");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Game End:");
        Console.WriteLine("  - The game ends when your Health reaches 0 or you clear the entire Dungeon.");
        Console.WriteLine("  - Your win by clearing the entire Dungeon.\n");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Press any key to start the game...");
        Console.ResetColor();
        Console.ReadKey();
        Console.Clear();
    }
    #endregion

    #region Player Turn
    // Represents a player's turn, where all core interactions happen
    public void PlayerTurn()
    {
        Player.healthPotionUsed = false;
        CurrentRoom.DrawRoom(Dungeon);

        Console.Clear();
        DisplayGame();

        // A room is only refiled when only 1 card remains
        while (CurrentRoom.cards.Count > 1)
        {
            // Cannot avoid two rooms in a row
            if (!previousRoomAvoided && CurrentRoom.cards.Count == 4)
            {
                Console.WriteLine("Do your wish to avoid this room? (y/N)");
                string? avoidInput = Console.ReadLine()?.Trim().ToLower();

                if (avoidInput == "y")
                {
                    AvoidRoom();

                    previousRoomAvoided = true;
                    Console.WriteLine("Room avoided!");
                    return;
                }
            }

            // Card interaction choice input
            Console.WriteLine("Choose a card to interact with (enter the number):");
            string? choice = Console.ReadLine()?.Trim();

            if (int.TryParse(choice, out int cardIndex) && cardIndex >= 1 && cardIndex <= CurrentRoom.cards.Count)
            {
                Card chosenCard = CurrentRoom.cards[cardIndex - 1];
                Console.WriteLine($"\nInteracting with {chosenCard.Name}...");

                // Verify chosen card type
                switch (chosenCard.Type)
                {
                    case CardType.Health:
                        CardHealth(chosenCard);
                        break;

                    case CardType.Weapon:
                        CardWeapon(chosenCard);
                        break;

                    case CardType.Monster:
                        CardMonster(chosenCard);
                        break;
                }

                CurrentRoom.cards.RemoveAt(cardIndex - 1);

                // Game over when player dies
                if (Player.Health <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPlayer died! Game over!");
                    Console.ResetColor();

                    isGameOver = true;
                    return;
                }

                // Game over when dungeon cleared, player wins
                if (Dungeon.CardsRemaining == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nDungeon cleared! You win!");
                    Console.ResetColor();

                    isGameOver = true;
                    return;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Invalid input. Please try again.");
                Console.ResetColor();
            }

            DisplayGame();
        }
        previousRoomAvoided = false;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Room cleared! Press any key to proceed to the next turn.");
        Console.ResetColor();
        Console.ReadKey();
    }
    #endregion

    #region Card Interaction
    // Card interaction methods, called when checking the chosen card type

    // Use a Heart card to restore health
    private void CardHealth(Card card)
    {
        if (!Player.healthPotionUsed)
        {
            int heal = card.Value;
            int previousHealth = Player.Health;

            // Health caped at 20 points in the original game
            Player.Health = Math.Min(Player.MaxHealth, Player.Health + heal);
            Player.healthPotionUsed = true;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Healed for {heal} points (Health: {previousHealth} -> {Player.Health}).");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Health potion already used this turn! Discarding card.");
            Console.ResetColor();
        }
        DiscardPile.Add(card);
    }

    // Equip a Weapon card, discarding the previous one
    private void CardWeapon(Card card)
    {
        if (Player.EquippedWeapon != null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Discarding previous weapon: {Player.EquippedWeapon.Name}.");
            Console.ResetColor();

            DiscardPile.Add(Player.EquippedWeapon);
        }

        Player.EquippedWeapon = card;
        Player.LastSlainMonsterValue = null;

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"Equipped new weapon: {card.Name}.");
        Console.ResetColor();
    }

    // Fight a Monster card, either with the equipped weapon or barehanded
    private void CardMonster(Card card)
    {
        // Automatically fight barehanded if it's the only option
        if (Player.EquippedWeapon == null || (Player.LastSlainMonsterValue.HasValue && Player.LastSlainMonsterValue < card.Value))
        {
            int damage = card.Value;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n--- Combat Result ---");
            Console.WriteLine("Fighting barehanded!");
            Console.WriteLine($"Monster: {card.Name} (Value: {card.Value})");
            Console.WriteLine($"Damage Taken: {damage}");
            Console.WriteLine("---------------------\n");
            Console.ResetColor();

            Player.Health -= damage;
            DiscardPile.Add(card);
            return;
        }

        // Combat choice, weapon or barehanded
        Console.WriteLine("Do you want to fight this monster with your weapon or barehanded? (w/B)");
        string? combatInput = Console.ReadLine()?.Trim().ToLower();
        if (combatInput == "w")
        {
            int damage = Math.Max(0, card.Value - Player.EquippedWeapon.Value);

            Player.Health -= damage;
            Player.LastSlainMonsterValue = card.Value;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n--- Combat Result ---");
            Console.WriteLine($"Fought with weapon: {Player.EquippedWeapon.Name}");
            Console.WriteLine($"Monster: {card.Name} (Value: {card.Value})");
            Console.WriteLine($"Damage Taken: {damage}");
            Console.WriteLine("---------------------\n");
            Console.ResetColor();

            DiscardPile.Add(card);
        }
        else
        {
            int damage = card.Value;

            Player.Health -= damage;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n--- Combat Result ---");
            Console.WriteLine("Fought barehanded");
            Console.WriteLine($"Monster: {card.Name} (Value: {card.Value})");
            Console.WriteLine($"Damage Taken: {damage}");
            Console.WriteLine("---------------------\n");
            Console.ResetColor();

            DiscardPile.Add(card);
        }
    }
    #endregion
}