using Godot;
using System;

public partial class RandomizeLevers : Node
{
	[Export]
	private Node3D[] nodes;
	private RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int lever = randomNumberGenerator.RandiRange(0, nodes.Length-1);
		for(int i = 0; i < nodes.Length; i++)
		{
			if(i == lever) { continue; }
			nodes[i].QueueFree();
		}
	}
}
