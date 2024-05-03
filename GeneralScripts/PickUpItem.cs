using Godot;
using System;

public partial class PickUpItem : Node3D, Iinteractable
{
    MeshInstance3D[] meshInstances;
    [Export]
    private InventoryItem item;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        meshInstances = this.GetAllChildrenByType<MeshInstance3D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void EnableGlow()
    {
        foreach(MeshInstance3D meshInstance in meshInstances)
        {
            ShaderMaterial material = meshInstance.MaterialOverlay as ShaderMaterial;
            material.SetShaderParameter("strenght", 1);
        }
    }

    public void DisableGlow()
    {
       
        foreach (MeshInstance3D meshInstance in meshInstances)
        {
            ShaderMaterial material = meshInstance.MaterialOverlay as ShaderMaterial;
            material.SetShaderParameter("strenght", 0);
        }
    }


    public bool Interact(PlayerController player)
    {
        if (!player.inventory.InventoryHasSpace()) {return false;}
        player.inventory.AddItemToInventory(item);
        QueueFree();
        return true;
    }
}
