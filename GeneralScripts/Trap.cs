using Godot;
using System.Linq;

public partial class Trap : Area3D
{
    [Export]
    private Timer delaytimer;
    [Export]
    private Timer activetimer;
    [Export]
    private bool alwaysRun;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GameManager manager = GetTree().Root.GetChildByType<GameManager>();

        Weapon weapon = this.GetChildByType<Weapon>();
        weapon.SetOwner(this);
        AnimationTree animationTree = this.GetChildByType<AnimationTree>();
        SoundSource soundSource = this.GetChildByType<SoundSource>();
        AudioStreamPlayer3D audioStreamPlayer = this.GetChildByType<AudioStreamPlayer3D>();

        TrapObject trapObject = new(delaytimer, activetimer, soundSource, audioStreamPlayer, animationTree, weapon, this, alwaysRun);
        manager.trapObjects.Add(trapObject);
        this.SetScript("");
    }
}
