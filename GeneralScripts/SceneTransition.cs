using Godot;
using System;

public partial class SceneTransition : Area3D
{
    private GameManager manager;
	[Export]
	private int levelIndex;
	
	public override void _Ready()
	{
        manager = GetTree().Root.GetChildByType<GameManager>();
	}

	public override void _Process(double delta)
	{
		foreach (Node n in GetOverlappingBodies())
		{
			PlayerController p = n as PlayerController;
			if (p == null) { continue; }
			manager.LoadSceneByID(levelIndex);
        }
	}
}
