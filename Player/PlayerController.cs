using Godot;
using System;

public partial class PlayerController : CharacterBody3D, IDamagable
{
    [ExportGroup("Walk Mode")]
    [Export] public float walkSpeed = 5.0f;
    [Export] public float sprintSpeed = 8.0f;
    [Export] public float jumpVelocity = 4.8f;
    [Export] public float gravity = 9.81f;

    [ExportGroup("References")]
    [Export] public Node3D pivot;
    [Export] public CollisionShape3D standingCollisionShape;
    [Export] public CollisionShape3D crouchingCollisionShape;
    [Export] public Area3D headBox;

    private bool active = true;

    private bool crouchToggle;

    public float horizontalFlySpeed;
    public float verticalFlySpeed;

    private StateMachine<PlayerController> stateMachine;
    private RandomNumberGenerator numberGenerator;
    public bool sneaking;


    public Inventory inventory {  get; private set; }

    public void ChangeState(Type type)
    {
        stateMachine.ChangeState(type);
    }

    private void SetupStateMachine()
    {
        stateMachine = new StateMachine<PlayerController>(
            this,
            new WalkState(),
            new CrouchState()
            );
        SetupTransitions();
        ChangeState(typeof(WalkState));
    }

    public override void _Ready()
    {
        numberGenerator = new RandomNumberGenerator();
        AttackSetup();
        SetupStateMachine();
        CameraSetup();
        //temp
        //Input.MouseMode = Input.MouseModeEnum.Captured;
        inventory = new Inventory();
    }

    public override void _Process(double delta)
    {
        HandleInputs();
        stateMachine.OnUpdate(delta);
        CameraInput();
        attackProcess(delta);
        CheckForInteractable();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (active)
        {
            stateMachine.OnPhysicsUpdate(delta);
            CameraPhysicsProcess(delta);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (active)
        {
            stateMachine.CurrentState.UnHandledInput(@event);
            CameraInput(@event);
        }
    }
}
