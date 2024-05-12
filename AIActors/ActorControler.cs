using Godot;
using System;
using AIStates;
using System.Runtime.InteropServices;

public partial class ActorControler : CharacterBody3D, IDamagable, ISoundListner
{
    [Export]
    float health = 15;
    [Export]
    public float runSpeed;
    [Export]
    public bool randomStartState = false;
    [Export]
    public Node3D visuals { get; private set; }

    //player referance
    public PlayerController player { get; private set; }
    public NavigationAgent3D navigationAgent3D { get; private set; }
    public RandomNumberGenerator randomNumberGenerator { get; private set; } = new();
    private StateMachine<ActorControler> stateMachine;
    public Weapon weapon { get; private set; }

    [Export]
    public Timer backOffTimer { get; private set; }
    [Export]
    public Timer inRangeTimer { get; private set; }
    [Export]
    public Timer damagedTimer { get; private set; }
    [Export]
    public AnimationTree tree { get; private set; }


    public LerpVaule runLerp { get; private set; }
    public LerpVaule MovementX { get; private set; }
    public LerpVaule MovementZ { get; private set; }

    private float alertValue;
    public float AlertValue { get { return alertValue; } set { alertValue = value; AlertUpdated?.Invoke(value); } }

    public Vector3 positionOfIntrest;

    private Vector3 scale;
    public PackedScene LootScene { get; private set; }

    [Export]
    private bool hasSight = true;

    public Vector3 startLocation;

    [Export]
    public bool isWarden;
    [Export]
    public double attackTime = 0.7;

    [Export]
    public LootTable lootTable;
    [Export]
    public int minLootRolls;
    [Export]
    public int maxLootRolls;

    public Action<float> AlertUpdated;
    public Action PlayerAgrod;


    [Export]
    public float playerDetectedValue = 12;
    [Export]
    private float PlayerLostValue = 4;
    [Export]
    private float playerDetectedNearValue = 9;

    [Export]
    private SoundSource agroSource;
    [Export]
    private SoundSource hurtSource;
    [Export]
    private SoundSource livingSource;
    [Export]
    private Timer livingSoundTimer;
    


    [ExportGroup("Steps")]
    private float bobTime;
    [Export] public float StepFrequency = 2;
    [Export] public float StepFrequencyNormal = 2;
    [Export] public float StepFrequencyRunning = 2;
    [Export] private float StepAmplitude = 0.06f;
    private bool canPlay;

    [Export] public SoundSource stepSource;
    [Export]
    private AudioStream[] stepSounds;
    private int stepIndex = -1;
    private double deltaSinceLastUpdate;

    // Called through GD script
    public override void _Ready()
    {
        startLocation = GlobalPosition;
        if (visuals == null)
        {
            GD.PrintErr(this + "Has no visuals referance");
        }
        LootScene = GD.Load<PackedScene>("res://Prefabs/actor_loot_object.tscn");
        scale = new Vector3(Transform.Basis.X.Length(), Transform.Basis.Y.Length(), Transform.Basis.Z.Length());

        player = GetTree().GetNodesInGroup("player")[0] as PlayerController;
        navigationAgent3D = this.GetChildByType<NavigationAgent3D>();
        navigationAgent3D.VelocityComputed += NavigationAgent3D_VelocityComputed;

        SetupStateMachine();
        runLerp = new(0.1f);
        weapon = this.GetChildByType<Weapon>();
        weapon.SetOwner(this);
        AlertValue = 0;
        PlayerAgrod += AgroRoar;
        livingSoundTimer.WaitTime = randomNumberGenerator.RandfRange(1.5f, 4.5f);
        livingSoundTimer.Start();
    }

    private void NavigationAgent3D_VelocityComputed(Vector3 safeVelocity)
    {
        Velocity = safeVelocity;

        bobTime += (float)deltaSinceLastUpdate * Velocity.Length();
        deltaSinceLastUpdate = 0;
        Step(bobTime);

        Vector3 direction = Velocity.Project(this.Forward().Cross(Vector3.Up));
        if (Velocity == Vector3.Zero)
        {
            direction = Vector3.Zero;
        }
        Vector2 moveVelocity = new Vector2(direction.X, direction.Z).Normalized();
        tree.Set("parameters/Alive/MoveDirection/blend_position", moveVelocity);
        MoveAndSlide();
    }

    private void SetupStateMachine()
    {
        stateMachine = new StateMachine<ActorControler>(
            this,
            new IdleState(),
            new SleepState(),
            new InvestigateState(),
            new GotToPlayerState(),
            new BackOffFromPlayer(),
            new OrbitPlayer(),
            new AttackState(),
            new DeadState(),
            new WanderState()
            );
        SetupTransitions();

        if (isWarden)
        {
            stateMachine.ChangeState(typeof(WanderState));
            return;
        }

        if (randomStartState)
        {
            switch (randomNumberGenerator.RandiRange(0, 1))
            {
                case 0:
                    stateMachine.ChangeState(typeof(IdleState));
                    break;
                case 1:
                    stateMachine.ChangeState(typeof(SleepState));
                    break;
            }
        }
        else
        {
            stateMachine.ChangeState(typeof(IdleState));
        }
    }

    // Called every frame. through GD script
    public override void _Process(double delta)
    {
        Transform = Transform.Orthonormalized();
        Transform = Transform.Scaled(scale);
        stateMachine.OnUpdate(delta);
        tree.Set("parameters/Alive/Run/blend_amount", runLerp.getCurrent(delta));
        if (AlertValue > 0)
        {
            if (!seeingPlayer() || !hasSight)
            {
                AlertValue -= (float)delta * 0.3f;
                if (AlertValue < 0)
                {
                    AlertValue = 0;
                }
            }
        }


        if(livingSource != null)
        {
            if(livingSoundTimer.TimeLeft <= 0)
            {
                livingSoundTimer.WaitTime = randomNumberGenerator.RandfRange(7, 26);
                livingSoundTimer.Start();
                livingSource.SetRandomPitch(0.8f, 1.2f);
                livingSource.PlaySound();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        deltaSinceLastUpdate += delta;
        stateMachine.OnPhysicsUpdate(delta);
    }

    public virtual void TakeDamage(float damage)
    {
        if (stateMachine.CurrentState.GetType() == typeof(DeadState)) { return; }
        if (hurtSource != null)
        {
            hurtSource.SetRandomPitch(0.8f, 1.2f);
            hurtSource.PlaySound();
        }
        damagedTimer.Start();
        tree.Set("parameters/Alive/RandomHit/blend_position", randomNumberGenerator.RandfRange(-1, 1));
        tree.Set("parameters/Alive/OneShotHit/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        AlertValue = 10;
        health -= damage;
        if (isWarden && alertValue < playerDetectedValue) { AlertValue = playerDetectedValue; }
        if (health <= 0 && !isWarden)
        {
            stateMachine.ChangeState(typeof(DeadState));
        }
    }

    public void AddSoundImpulse(float value, Vector3 position)
    {
        AlertValue += value;
        if (value > 3 || AlertValue > 2)
        {
            if (AlertValue > playerDetectedNearValue)
            {
                positionOfIntrest = player.GlobalPosition;
            }
            else
            {
                positionOfIntrest = position;
            }
        }
    }



    private void Step(float time)
    {
        if (stepSource == null) { return; }
        float Y = Mathf.Sin(time * StepFrequency) * StepAmplitude;
        //GD.Print(time +"*"+StepFrequency);
        float lowPos = StepAmplitude - 0.05f;

        if (Y > -lowPos)
        {
            canPlay = true;
        }
        //GD.Print(Y + " " + -lowPos);
        if (Y < -lowPos && canPlay)
        {
            canPlay = false;
            stepIndex++;
            if (stepIndex > 3)
            {
                stepIndex = 0;
            }
            stepSource.SetAudio(stepSounds[stepIndex]);

            stepSource.SetRandomPitch(0.8f, 1.2f);
            stepSource.PlaySound();
        }


    }

    private void AgroRoar()
    {
        if (agroSource != null)
        {
            agroSource.SetRandomPitch(0.9f, 1.1f);
            agroSource.PlaySound();
        }
    }
}






public class LerpVaule
{
    double start = 0;
    double current = 0;
    double target = 0;
    double BlendElapsedTime = 0;
    double BlendMaxTime = 0.4;

    public LerpVaule(double maxTime, double target = 0, double current = 0)
    {
        BlendMaxTime = maxTime;
        this.target = target;
        this.current = current;
    }

    public void setTarget (double target)
    {
        start = current;
        this.target = target;
        BlendElapsedTime = 0;
    }


    public double getCurrent(double delta)
    {
        BlendElapsedTime += delta;
        current = Mathf.Lerp(start, target, Mathf.Clamp(BlendElapsedTime/BlendMaxTime,0,1));
        return current;
    }
}