using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [ExportGroup("Walk Mode")]
    [Export] public float walkSpeed = 5.0f;
    [Export] public float sprintSpeed = 8.0f;
    [Export] public float jumpVelocity = 4.8f;
    [Export] public float gravity = 9.81f;

    [ExportGroup("References")]
    [Export] public Node3D pivot;

    private bool active = false;


    public Vector2 inputDir { get; private set; }

    public float horizontalFlySpeed;
    public float verticalFlySpeed;

    private StateMachine<PlayerController> stateMachine;

    public void ChangeState(Type type)
    {
        stateMachine.ChangeState(type);
    }

    private void SetupStateMachine()
    {
        stateMachine = new StateMachine<PlayerController>(
            this,
            new WalkState()
            );
        ChangeState(typeof(WalkState));
    }


    public override void _Ready()
    {
        SetupStateMachine();
        CameraSetup();
        SetupTransitions();
        //temp
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        GD.Print(inputDir);
        if (Input.IsKeyPressed(Key.Ctrl)) 
        { 
            active = false; 
        }
        else
        {
            stateMachine.OnUpdate(delta);
            active = true;
        }
        CameraInput();
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
