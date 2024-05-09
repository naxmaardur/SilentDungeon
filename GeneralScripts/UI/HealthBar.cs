using Godot;

public partial class HealthBar : ProgressBar
{
    public override void _Ready()
    {
        PlayerController player = GetTree().GetNodesInGroup("player")[0] as PlayerController;
        player.healthUpdate += UpdateHealth;
        player.healthDisplay += SetHealthBarDisplayStatus;
    }

    public void UpdateHealth(float health)
    {
		Value = health;

		float r = Mathf.Abs(health * 0.01f - 1.0f);

        float g = health * 0.01f;

        float b = 0.0f;

		StyleBoxFlat styleBox =  (StyleBoxFlat)Get("theme_override_styles/fill");
		styleBox.BgColor = new Color(r, g, b);
    }

	public void SetHealthBarDisplayStatus(bool b)
	{
		Visible = b;
	}
}
