using ScoundrelGame.Classes;

// Adaptation of the Scoundrel game to .NET Console Application
// Se the OriginalRules.pdf for more information on the game, as well as its original creators Zach Gage and Kurt Bieg

namespace ScoundrelGame;
// Main program
class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Game game = new Game();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("********** Welcome to Scoundrel! **********\n");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("PRESS 'Ctrl + C' ANYTIME TO EXIT THE CONSOLE\n");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Do you wish to see the tutorial? (y/N)");
        Console.ResetColor();

        // Print the tutorial if requested
        string? tutorialInput = Console.ReadLine()?.Trim().ToLower();
        if (tutorialInput == "y")
        {
            game.PrintTutorial();
        }

        // Main game loop
        while (!game.isGameOver)
        {
            Console.WriteLine("Player's turn:");
            try
            {
                game.PlayerTurn();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                break;
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\nCards remaining in the dungeon: {game.Dungeon.CardsRemaining}");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Press any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
    }
}