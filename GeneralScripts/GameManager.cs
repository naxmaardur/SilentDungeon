using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class GameManager : Node
{
	public PlayerController player { get; set; }
	public ScoreTracker scoreTracker { get; set; }
	public ContainerInteractable container {  get; set; }
	public ItemContainer itemContainer { get; set; }

	[Export]
	private PackedScene dropSellerScene;

    [Export]
	public Node3D activeSceneContainer;


	private GameOverScreen gameOverScreen;
	private OutOfRunScreen outOfRunScreen;


	[Export]
	private PackedScene[] floors;

	private AudioStreamPlayer audioPlayer;

	public List<TrapObject> trapObjects;

	public void Save()
	{
		player.inventory.Save();
		scoreTracker.scoreObject.Save();
        itemContainer.Save();
    }

	public void PlayerDeath()
	{
		player.inventory = new Inventory(false);
		scoreTracker.scoreObject.FinalizeScore();
		//other stuff etc
		gameOverScreen.Open();
		player.OpenInventory();
        player.ToggleActiveActor(false);
        player.inputsActive = false;
		player.EquipmentUpdate();
        LoadScene(dropSellerScene);
        Save();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		trapObjects = new();
        audioPlayer = this.GetChildByType<AudioStreamPlayer>();

        player = GetTree().GetNodesInGroup("player")[0] as PlayerController;
		player.Setup();
        scoreTracker = GetTree().GetNodesInGroup("scoreTracker")[0] as ScoreTracker;
		scoreTracker.Setup();
        container = GetTree().GetNodesInGroup("container")[0] as ContainerInteractable;
        player.PlayerDeath += PlayerDeath;
		itemContainer = new(true);
        container.container = itemContainer;
		itemContainer.soldItem += scoreTracker.scoreObject.AddScore;
        LoadScene(dropSellerScene);
		container.Interact(player);
        gameOverScreen = Owner.GetChildByType<GameOverScreen>();
		outOfRunScreen = Owner.GetChildByType<OutOfRunScreen>();
		gameOverScreen.getScore = () => { return scoreTracker.scoreObject; };
		outOfRunScreen.OnQuitButtonPressed += QuitGame;
		outOfRunScreen.OnStartButtonPressed += EnterDungeon;
		gameOverScreen.OnButtonPressed += OpenOutOfGame;

        //Work around stuff because shit is weird
        CanvasLayer canvaslayer = Owner.GetChildByType<CanvasLayer>();
		Control control = new Control();
		CanvasLayer playerCanvas = player.GetChildByType<CanvasLayer>();

		Godot.Collections.Array<Node> nodes = playerCanvas.GetChildren();


        for (int i = nodes.Count-1; i > -1; i--)
		{
			Node n = nodes[i];
            n.Reparent(canvaslayer);
            if (i == 1)
            {
                canvaslayer.MoveChild(outOfRunScreen, 0);
            }
            canvaslayer.MoveChild(n, 0);
        }

        playerCanvas.Free();
    }

	public void OpenOutOfGame()
	{
		outOfRunScreen.Open();
		player.OpenInventory();
		container.Interact(player);
		player.ToggleActiveActor(false);
		player.inputsActive = false;
		scoreTracker.OutofRun(true);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        foreach (TrapObject trap in trapObjects)
        {
			if (IsInstanceValid(trap.area3D))
			{

				if (trap.area3D.GlobalPosition.DistanceTo(player.GlobalPosition) > 5)
				{
					trap.area3D.ProcessMode = ProcessModeEnum.Disabled;
					trap.weapon.ProcessMode = ProcessModeEnum.Disabled;
				}
				else
				{
					trap.area3D.ProcessMode = ProcessModeEnum.Inherit;
					trap.weapon.ProcessMode= ProcessModeEnum.Inherit;
				}
			}
		}
	}

	public void LoadedNewScene()
	{
        player.GlobalPosition = Vector3.Zero;
	}

	public void ExitDungeon()
	{
		int sold = 0;
		for(int i = 0; i < 36; i++)
		{
			if (player.inventory.inventoryItems[i] == null) { continue; }
            if (player.inventory.inventoryItems[i].SlotType != 0) { continue; }
			scoreTracker.scoreObject.AddScore(player.inventory.inventoryItems[i].Value);
			player.inventory.RemoveItemFromInventory(i);
			sold++;
        }
		if(sold > 0)
		{
			audioPlayer.Play();
        }

		Save();
		LoadScene(dropSellerScene);
		OpenOutOfGame();
    }


	public void QuitGame()
	{
        GD.Print("Quit pressed");

        GetTree().Quit();
    }

	private void LoadScene(PackedScene scene)
	{
        trapObjects.Clear();
        if (activeSceneContainer.GetChildCount() != 0)
		{
            activeSceneContainer.GetChild(0).QueueFree();
        }
        Node instance = scene.Instantiate();
		activeSceneContainer.AddChild(instance);
		LoadedNewScene();
    }


	public void LoadSceneByID(int id)
	{
		if(id == -1)
		{
			ExitDungeon();
			return;
		}
		LoadScene(floors[id]);
	}

	private void EnterDungeon()
	{
		player.health = 100;
		player.healthUpdate?.Invoke(100);
		LoadScene(floors[0]);
        player.CloseInventory();
        player.ToggleActiveActor(true);
        player.inputsActive = true;
        scoreTracker.OutofRun(false);
    }


}
