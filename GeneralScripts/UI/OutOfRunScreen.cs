using Godot;
using System;

public partial class OutOfRunScreen : Control
{
    public Action OnStartButtonPressed;
    public Action OnQuitButtonPressed;

	[Export]
	private Button startButton;

	[Export]
	private Button quitButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        startButton.Pressed += StartPressed;
		quitButton.Pressed += QuitPressed;
    }

	private void StartPressed()
	{
		OnStartButtonPressed?.Invoke();
		Visible = false;
	}
	private void QuitPressed()
	{
        OnQuitButtonPressed?.Invoke();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Open()
	{
		Visible = true;
	}
}
