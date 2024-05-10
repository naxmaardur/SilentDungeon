using Godot;
using System;

public partial class DropSeller : Node3D
{
	Area3D area;
	AudioStreamPlayer streamPlayer;
	ScoreTracker scoreTracker;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area = this.GetChildByType<Area3D>();
		streamPlayer = this.GetChildByType<AudioStreamPlayer>();
        scoreTracker = GetTree().GetNodesInGroup("scoreTracker")[0] as ScoreTracker;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach(Node3D node in area.GetOverlappingBodies())
		{
			PickUpItem pickUpItem = node as PickUpItem;
			if (pickUpItem != null)
			{
				streamPlayer.Play();
				scoreTracker.scoreObject.AddScore(pickUpItem.item.Value);
				pickUpItem.Free();
			}
		}
	}
}
