namespace ScoundrelGame.Classes;

// Player class, contains all the information about the player
public class Player
{
    public int Health { get; set; } = 20;
    public int MaxHealth { get; set; } = 20;
    public Card? EquippedWeapon { get; set; } = null;
    public int? LastSlainMonsterValue { get; set; } = null;
    public bool healthPotionUsed { get; set; } = false;
}