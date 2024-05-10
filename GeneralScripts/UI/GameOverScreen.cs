using Godot;
using System;

public partial class GameOverScreen : Control
{
	[Export]
	private Label mostCollected;
	[Export]
	private Label collected;
	[Export]
	private Button button;


	public Func<Score> getScore;
	public Action OnButtonPressed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button.Pressed += ButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	private void ButtonPressed()
	{
		Visible = false;
		OnButtonPressed?.Invoke();
    }

	public void Open()
	{
		Score score = getScore();
		mostCollected.Text = "" + score.recordScore;
		collected.Text = "" + score.lastScore;
		Visible = true;
	}
}
