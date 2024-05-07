using Godot;

[GlobalClass]
public partial class LootDorp : Resource
{
    [Export]
    public InventoryItem item;
    [Export]
    public float dropChance;
}
