using Godot;

public class TrapObject
{
    private bool activated;
    private bool playerDetected;
    private bool alwaysRun;

    private Weapon weapon;
    private Timer delaytimer;
    private Timer activetimer;
    private AnimationTree animationTree;
    private SoundSource soundSource;
    private AudioStreamPlayer3D audioStreamPlayer;
    private Area3D area3D;
    private Godot.RandomNumberGenerator randomNumberGenerator;

    public TrapObject(Timer timer1, Timer timer2, SoundSource sound, AudioStreamPlayer3D streamPlayer, AnimationTree tree, Weapon weapon, Area3D area3D, bool alwaysRun)
    {
        delaytimer = timer1;
        activetimer = timer2;
        soundSource = sound;
        animationTree = tree;
        audioStreamPlayer = streamPlayer;
        this.weapon = weapon;
        this.area3D = area3D;
        this.alwaysRun = alwaysRun;
        randomNumberGenerator = new();


        area3D.BodyEntered += bodyEntered;
        delaytimer.Timeout += timerDone;
        activetimer.Timeout += ActiveTimerDone;



        if (alwaysRun)
        {
            playerDetected = true;
            delaytimer.Start();
        }
    }

    public void bodyEntered(Node3D node)
    {
        PlayerController playerController = node as PlayerController;
        if (playerController != null)
        {
            if (delaytimer.TimeLeft <= 0 && !activated && !playerDetected)
            {
                playerDetected = true;
                delaytimer.Start();
            }
        }
    }

    private void timerDone()
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

    private void ActiveTimerDone()
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
