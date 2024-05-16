
using Godot;
using System;

public partial class Score: Resource
{
    [Export]
    public long recordScore;
    [Export] 
    public long currentscore;
    [Export]
    public long lastScore;

    public bool newRecord;

    public Action scoreUpdated;
    private const string SAVEPATH = "user://Score.tres";
    public Score()
    {
    }

    public void AddScore(int value)
    {
        currentscore += value;
        scoreUpdated?.Invoke();
    }

    public void FinalizeScore()
    {
        if (currentscore > recordScore)
        {
            recordScore = currentscore;
            newRecord = true;
        }
        lastScore = currentscore;
        currentscore = 0;
        Save();
        scoreUpdated?.Invoke();
    }

    public void Save()
    {
        ResourceSaver.Save(this, SAVEPATH);
    }

    public void Load()
    {
        if (ResourceLoader.Exists(SAVEPATH))
        {
            Score score = (Score)ResourceLoader.Load(SAVEPATH);
            currentscore = score.currentscore;
            lastScore = score.lastScore;
            recordScore = score.recordScore;
        }
        scoreUpdated?.Invoke();

    }

    
}
