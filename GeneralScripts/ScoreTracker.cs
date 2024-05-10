using Godot;
using System;

public partial class ScoreTracker : Control
{
	public Score scoreObject;
	[Export]
	private Label scoreLabel;
    [Export]
    private Label scoreLabel2;
    [Export]
	private Label lastScore;
	[Export]
	private Label recordScore;

	// Called when the node enters the scene tree for the first time.
	public void Setup()
	{
		scoreObject = new Score();
		scoreObject.Load();
		scoreObject.scoreUpdated += UpdateScoreUI;
		UpdateScoreUI();
	}
	
	public void UpdateScoreUI()
	{
		scoreLabel.Text = ""+scoreObject.currentscore;
		scoreLabel2.Text = "" + scoreObject.currentscore;
		lastScore.Text = "" + scoreObject.lastScore;
		recordScore.Text = "" + scoreObject.recordScore;
	}

	public void OutofRun(bool b)
	{
		if (b)
		{
			scoreLabel2.GetParent<Control>().Visible = true;
            lastScore.GetParent<Control>().Visible = true;
            recordScore.GetParent<Control>().Visible = true;
            scoreLabel.GetParent<Control>().Visible = false;
        }
		else
		{
            scoreLabel2.GetParent<Control>().Visible = false;
            lastScore.GetParent<Control>().Visible = false;
            recordScore.GetParent<Control>().Visible = false;
            scoreLabel.GetParent<Control>().Visible = true;
        }
    }
    
}
