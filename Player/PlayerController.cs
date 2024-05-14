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
    [Export] public Control healthbar;
    [Export] public SoundSource stepSource;
    [Export] private SoundSource landSource;
    [Export] private SoundSource hurtSource;
    [Export] private Control crosshair;


    private bool crouchToggle;

    public float horizontalFlySpeed;
    public float verticalFlySpeed;

    private StateMachine<PlayerController> stateMachine;
    private RandomNumberGenerator numberGenerator;
    public bool sneaking;


    public Inventory inventory;
    private UIInvetory uiInvetory;
    public UIItemContainer uIContainer;


    public bool inputsActive = false;
    public bool activeActor = false; // for the out of run UI and stuff



    private float protection;
    public float SpeedMod { get; private set; }
    public float SneakSpeedMod { get; private set; }
    public float RunSpeedMod { get; private set; }
    public float SoundMod { get; private set; }

    private bool wasGrounded;
    private bool canPlay;


    [Export]
    private AudioStream[] stepSounds;
    [Export]
    private AudioStream[] SneakSounds;
    private int stepIndex = -1;
    public int StepType;

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
        AttackSetup();
        EquipmentUpdate();
        SetupStateMachine();
        CameraSetup();
        inventory.EquipmentUpdated += EquipmentUpdate;
        healthUpdate?.Invoke(health);
        ToggleActiveActor(false);
        OpenInventory();
    }


    public void Setup()
    {
        inventory = new Inventory(true);
        numberGenerator = new RandomNumberGenerator();
        uiInvetory = this.GetChildByType<UIInvetory>();
        uIContainer = this.GetChildByType<UIItemContainer>();
        uiInvetory.Setup();
        uIContainer.Setup();
    }

    public void EquipmentUpdate()
    {
        if (inventory.EquipedItems[0] != null)
        {
            SetWeaponRight(inventory.EquipedItems[0].WeaponID);
        }
        else
        {
            SetWeaponRight(0);
        }
        SetAttack(rightHandTree, false);

        protection = 0;
        SpeedMod = 1;
        SoundMod = 1;
        SneakSpeedMod = 0;
        RunSpeedMod = 0;

        foreach(InventoryItem item in inventory.EquipedItems)
        {
            if (item != null)
            {
                protection += item.Protection;
                SpeedMod += item.SpeedMod;
                SoundMod += item.SoundMod;
                SneakSpeedMod += item.SneakSpeedMod;
                RunSpeedMod += item.RunSpeedMod;
            }
        }
        protection = 1 - Mathf.Clamp(Mathf.InverseLerp(0, 125, Mathf.Clamp(protection, 0, 125)), 0, 0.8f);
    }


    public override void _Process(double delta)
    {
        if (!activeActor) { return; }
        HandleInputs();
        stateMachine.OnUpdate(delta);
        if (inputsActive)
        {
            CameraInput();
            CheckForInteractable();
        }
        attackProcess(delta);

        if(IsOnFloor() && !wasGrounded)
        {
            landSource.SetRandomPitch(0.8f, 1.05f);
            landSource.PlaySound();
        }
        wasGrounded = IsOnFloor();

    }

    public override void _PhysicsProcess(double delta)
    {
        if (!activeActor) { return; }
        stateMachine.OnPhysicsUpdate(delta);
        CameraPhysicsProcess(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!inputsActive) { return; }
        stateMachine.CurrentState.UnHandledInput(@event);
        CameraInput(@event);
    }

    public void ToggleActiveActor(bool active)
    {
        activeActor = active;
        healthbar.Visible = active;
    }


    public void PlayerInMap()
    {
        ToggleActiveActor(true);
        inputsActive = true;
        CloseInventory();
    }
}
