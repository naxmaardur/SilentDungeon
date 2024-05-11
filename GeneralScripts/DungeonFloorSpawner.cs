using Godot;
using System;

public partial class DungeonFloorSpawner : Node
{
	[Export]
	private PackedScene[] floorScenes;
	private RandomNumberGenerator randomNumberGenerator = new();
	
	public override void _Ready()
	{
        PackedScene scene = floorScenes[randomNumberGenerator.RandiRange(0, floorScenes.Length - 1)];

        Node instance = scene.Instantiate();
        this.AddChild(instance);
    }
}
