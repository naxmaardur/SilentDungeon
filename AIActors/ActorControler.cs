using Godot;
using System;
using AIStates;

public partial class ActorControler : CharacterBody3D, IDamagable, ISoundListner
{
    [Export]
    public float runSpeed;
    [Export]
    public bool randomStartState = false;

	//player referance
	public PlayerController player {  get; private set; }
	public NavigationAgent3D navigationAgent3D { get; private set; }
    public RandomNumberGenerator randomNumberGenerator { get; private set; } = new RandomNumberGenerator();
	private StateMachine<ActorControler> stateMachine;
    public Weapon weapon { get; private set; }

    [Export]
    public Timer backOffTimer { get; private set; }
    [Export]
    public Timer inRangeTimer {  get; private set; }
    [Export]
    public AnimationTree tree { get; private set; }


    public LerpVaule runLerp { get; private set; }
    public LerpVaule MovementX { get; private set; }
    public LerpVaule MovementZ { get; private set; }

    public float alertValue;

    public Vector3 positionOfIntrest;

    private Vector3 scale;


    // Called through GD script
    public override void _Ready()
	{
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
        tree.Set("parameters/MoveDirection/blend_position", moveVelocity);
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
            new AttackState()
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
        tree.Set("parameters/Run/blend_amount", runLerp.getCurrent(delta));
    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine.OnPhysicsUpdate( delta );

    }

    public virtual void TakeDamage(float damage)
    {
        tree.Set("parameters/RandomHit/blend_position", randomNumberGenerator.RandfRange(-1, 1));
        tree.Set("parameters/OneShotHit/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        throw new NotImplementedException();
    }

    public void AddSoundImpulse(float value, Vector3 position)
    {
        GD.Print("Sound Value: " + value);
        alertValue += value;
        if(value > 4 || alertValue > 4)
        {
            positionOfIntrest = position;
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