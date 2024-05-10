using Godot;
using System;

public partial class ToolTip : Control
{
    [Export]
    private Label nameLable;
    [Export]
    private Label ProtectionLable;
    [Export]
    private Label SpeedLable;
    [Export]
    private Label RunLable;
    [Export]
    private Label SneakLable;
    [Export]
    private Label SoundLable;
    [Export]
    private Label TypeLable;

    private float yHight;


    public override void _Ready()
    {

        CustomMinimumSize = new Vector2(200, yHight);

    }
    // Called when the node enters the scene tree for the first time.
	public void SetValues(InventoryItem item)
	{
        yHight = 90;        
        nameLable.Text = item.Name;
        SpeedLable.Text = "";
        RunLable.Text = "";
        SneakLable.Text = "";
        SoundLable.Text = "";

        switch (item.SlotType)
        {
            case 1:
                TypeLable.Text = "Weapon";
                break;
            case 2:
                TypeLable.Text = "Head";
                break;
            case 3:
                TypeLable.Text = "Body";
                break;
            case 4:
                TypeLable.Text = "Feet";
                break;
            case 5:
                TypeLable.Text = "Consumable";
                break;
            case 6:
                TypeLable.Text = "???";
                break;
            default:
                TypeLable.Text = "Miscellaneous";
                break;
        }

        if (item.WeaponID != 0)
        {
            ProtectionLable.Text = "Damage: " + item.weaponDamage;
        }
        else if (item.Value != 0 && item.Protection == 0)
        {
            ProtectionLable.Text = "Value: " + item.Value;
        }
        else
        { 
            ProtectionLable.Text = "Defence: " + item.Protection; 
        }

        if(item.SpeedMod != 0)
        {
            if (item.SpeedMod > 0) { SpeedLable.Text = "Speed: +" + item.SpeedMod*100+"%"; }
            else
            {
                SpeedLable.Text = "Speed: " + item.SpeedMod * 100 + "%";
            }
            yHight += 30;
        }
        else
        {
            SpeedLable.Free();
        }
        if (item.RunSpeedMod != 0)
        {
            if (item.RunSpeedMod > 0) { RunLable.Text = "Run Speed: +" + item.RunSpeedMod * 100 + "%"; }
            else
            {
                RunLable.Text = "Run Speed: " + item.RunSpeedMod * 100 + "%";
            }
            yHight += 30;
        }
        else
        {
            RunLable.Free();
        }
        if (item.SneakSpeedMod != 0)
        {
            if (item.SneakSpeedMod > 0) { SneakLable.Text = "Sneak Speed: +" + item.SneakSpeedMod * 100 + "%"; }
            else
            {
                SneakLable.Text = "Sneak Speed: " + item.SneakSpeedMod * 100 + "%";
            }
            yHight += 30;
        }
        else
        {
            SneakLable.Free();
        }
        if (item.SoundMod != 0)
        {
            if (item.SoundMod > 0) { SoundLable.Text = "Noise: +" + item.SoundMod * 100 + "%"; }
            else
            {
                SoundLable.Text = "Noise: " + item.SoundMod * 100 + "%";
            }
            yHight += 30;
        }
        else
        {
            SoundLable.Free();
        }
    }
}
