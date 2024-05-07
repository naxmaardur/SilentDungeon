using Godot;
using System;

[GlobalClass]
public partial class LootTable : Resource
{
	[Export]
	public LootDorp[] lootDorps;

	public InventoryItem RollDrop()
	{
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
		float roll = randomNumberGenerator.RandfRange(0, 100);

		float checkRange = 0;
		for (int i = 0; i < lootDorps.Length; i++)
		{
			checkRange += lootDorps[i].dropChance;
			if(roll <= checkRange)
			{
				return lootDorps[i].item;
			}
		}
		return lootDorps[0].item;
	}
}
