using Godot;
using System.Linq;

public partial class Trap : Node3D
{
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
        Area3D area3D = this.GetChildByType<Area3D>();


        TrapObject trapObject = new(GetChild(0) as Timer, GetChild(1) as Timer, soundSource, audioStreamPlayer, animationTree, weapon, area3D, alwaysRun);
        manager.trapObjects.Add(trapObject);
        
        this.SetScript("");
    }
}
