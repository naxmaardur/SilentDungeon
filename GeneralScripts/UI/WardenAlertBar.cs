using Godot;
using System;

public partial class WardenAlertBar : Control
{
	private ProgressBar bar;
    private Label label;
    private Timer timer;
    private bool labelAnimation;
    private RandomNumberGenerator randomNumberGenerator = new();

    private float alertValue;
    private int wardens = 0;


    public override void _Ready()
    {
        bar = this.GetChildByType<ProgressBar>();
        label = this.GetChildByType<Label>();
        timer = this.GetChildByType<Timer>();

        ;
        foreach(Node n in GetTree().GetNodesInGroup("Warden"))
        {
            ActorControler warden = n as ActorControler;
            warden.AlertUpdated += AlertStatusUpdated;
            warden.PlayerAgrod += WardenAgro;
            bar.MaxValue = warden.playerDetectedValue;
            warden.wardenAlertBar = this;
            wardens++;
        }
        label.Text = "";
        AlertStatusUpdated(alertValue);
    }

	public override void _Process(double delta)
	{
        if (labelAnimation)
        {
            if(timer.TimeLeft > 0)
            {
                float alpha = (float) (timer.TimeLeft / timer.WaitTime);
                label.Set("theme_override_colors/font_color", new Color(1, 0, 0, alpha));
            }
            else
            {
                label.Set("theme_override_colors/font_color", new Color(1, 0, 0, 0));
                label.Text = "";
                labelAnimation = false;
            }
        }
	}


    public void WardenAgro()
    {
        switch (randomNumberGenerator.RandiRange(0, 4))
        {
            case 0:
                label.Text = "RUN";
                break;
            case 1:
                label.Text = "GET OUT";
                break;
            case 2:
                label.Text = "FOUND YOU";
                break;
            case 3:
                label.Text = "TIME TO DIE";
                break;
            case 4:
                label.Text = "Shouldn't have done that";
                break;
        }
        labelAnimation = true;
        label.Set("theme_override_colors/font_color", new Color(1, 0, 0, 1));
        timer.Start();
    }

    public void addAlertValue(float add)
    {
        alertValue += add / wardens;
        AlertStatusUpdated(alertValue);
    }
    public void removeAlertValue(float remove)
    {
        alertValue -= remove /wardens;
        AlertStatusUpdated(alertValue);
    }

    public float getAlertValue()
    {
        return alertValue;
    }

	public void AlertStatusUpdated(float Alert)
	{
        bar.Value = Alert;

        float ColorProgress = Alert / (float)bar.MaxValue * 100;

        float r = ColorProgress * 0.01f;

        float g = Mathf.Abs(ColorProgress * 0.01f - 1.0f); 

        float b = 0.0f;

        StyleBoxFlat styleBox = (StyleBoxFlat)bar.Get("theme_override_styles/fill");
        styleBox.BgColor = new Color(r, g, b);
    }

}
