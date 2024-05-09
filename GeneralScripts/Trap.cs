using Godot;
using System.Linq;

public partial class Trap : Area3D
{
	private Weapon weapon;
	[Export]
	private Timer delaytimer;
	[Export]
	private Timer activetimer;
	private bool activated;
	private bool playerDetected;
	private AnimationTree animationTree;
	private SoundSource soundSource;
	private AudioStreamPlayer3D audioStreamPlayer;
	private RandomNumberGenerator randomNumberGenerator = new();

	[Export]
	private bool alwaysRun;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		weapon = this.GetChildByType<Weapon>();
        weapon.SetOwner(this);
		animationTree = this.GetChildByType<AnimationTree>();
		soundSource = this.GetChildByType<SoundSource>();
        audioStreamPlayer = this.GetChildByType<AudioStreamPlayer3D>();

        if (alwaysRun)
		{
			playerDetected = true;
            delaytimer.Start();
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		Node3D[] nodes = GetOverlappingBodies().ToArray();
		foreach (Node3D node in nodes)
		{
			PlayerController playerController = node as PlayerController;
			if (playerController != null) 
			{ 
				if(delaytimer.TimeLeft <= 0 && !activated && !playerDetected)
				{
                    playerDetected = true;
                    delaytimer.Start();
                }
			}
		}


		if(delaytimer.TimeLeft <= 0 && playerDetected)
		{
			weapon.EnableHitBoxes();
			activetimer.Start();
			activated = true;
			playerDetected = false;
			animationTree.Set("parameters/conditions/Active", true);
            animationTree.Set("parameters/conditions/NotActive", false);
			audioStreamPlayer.PitchScale = randomNumberGenerator.RandfRange(0.9f, 1.1f);

            if (alwaysRun)
			{
				audioStreamPlayer.Play();
			}
			else
			{
				soundSource.PlaySound();
			}
        }

        if (activetimer.TimeLeft <= 0 && activated)
        {
            weapon.DisableHitBoxes();
            activated = false;
            playerDetected = false;
            animationTree.Set("parameters/conditions/Active", false);
            animationTree.Set("parameters/conditions/NotActive", true);
            if (alwaysRun)
            {
                playerDetected = true;
                delaytimer.Start();
            }
        }
    }


    

}
