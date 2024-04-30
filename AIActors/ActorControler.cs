using Godot;
using System;
using AIStates;

public partial class ActorControler : CharacterBody3D, IDamagable, ISoundListner
{
    [Export]
    float health = 15;
    [Export]
    public float runSpeed;
    [Export]
    public bool randomStartState = false;
    [Export]
    public Node3D visuals {  get; private set; }

	//player referance
	public PlayerController player {  get; private set; }
	public NavigationAgent3D navigationAgent3D { get; private set; }
    public RandomNumberGenerator randomNumberGenerator { get; private set; } = new();
	private StateMachine<ActorControler> stateMachine;
    public Weapon weapon { get; private set; }

    [Export]
    public Timer backOffTimer { get; private set; }
    [Export]
    public Timer inRangeTimer {  get; private set; }
    [Export]
    public Timer damagedTimer { get; private set; }
    [Export]
    public AnimationTree tree { get; private set; }


    public LerpVaule runLerp { get; private set; }
    public LerpVaule MovementX { get; private set; }
    public LerpVaule MovementZ { get; private set; }

    public float alertValue;

    public Vector3 positionOfIntrest;

    private Vector3 scale;
    public PackedScene LootScene { get; private set; }

    [Export]
    private bool hasSight = true;

    public Vector3 startLocation;

    

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
    }

    private void NavigationAgent3D_VelocityComputed(Vector3 safeVelocity)
    {
        Velocity = safeVelocity;
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
            new DeadState()
            );
        SetupTransitions();

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
        stateMachine.OnUpdate( delta );
        tree.Set("parameters/Alive/Run/blend_amount", runLerp.getCurrent(delta));
        if(alertValue > 0)
        {
            if (!seeingPlayer() || !hasSight)
            {
                alertValue -= (float)delta * 0.3f;
                if (alertValue < 0)
                {
                    alertValue = 0;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine.OnPhysicsUpdate( delta );

    }

    public virtual void TakeDamage(float damage)
    {
        if(stateMachine.CurrentState.GetType() == typeof(DeadState)) { return; }
        damagedTimer.Start();
        tree.Set("parameters/Alive/RandomHit/blend_position", randomNumberGenerator.RandfRange(-1, 1));
        tree.Set("parameters/Alive/OneShotHit/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        alertValue = 10;
        health -= damage;
        if(health <= 0)
        {
            stateMachine.ChangeState(typeof(DeadState));
        }
    }

    public void AddSoundImpulse(float value, Vector3 position)
    {
        alertValue += value;
        if(value > 3 || alertValue > 2)
        {
            GD.Print(alertValue);
            if(alertValue > 9)
            {
                positionOfIntrest = player.GlobalPosition;
            }
            else
            {
                positionOfIntrest = position;
            }
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