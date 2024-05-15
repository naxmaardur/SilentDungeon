using Godot;
using System;

public partial class PickUpItem : Node3D, Iinteractable
{
    MeshInstance3D[] meshInstances;
    [Export]
    public InventoryItem item;
    [Export]
    public Shader shader;

    [Export]
    public CollisionShape3D areaCollision;

    [Export]
    private SoundSource sound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        meshInstances = this.GetAllChildrenByType<MeshInstance3D>();
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void AddMesh(Node3D node)
    {
        CollisionShape3D collisionObject = node.GetChildByType<CollisionShape3D>();
        this.areaCollision.Shape = new BoxShape3D();
        this.areaCollision.Shape.Set("size", collisionObject.Shape.Get("size"));
        //areaCollision.Shape.Set("size", collisionObject.Shape.Get("size"));

        meshInstances = node.GetAllChildrenByType<MeshInstance3D>();
        foreach (MeshInstance3D child in meshInstances)
        {
            ShaderMaterial material = new ShaderMaterial();
            material.Shader = shader;
            child.MaterialOverlay = material;
            material.SetShaderParameter("emission", new Color(0.7f, 0.7f, 0.7f));
            material.SetShaderParameter("strenght", 0.1f);
        }

        this.CallDeferred("MoveDownToFloor");
    }

    private void MoveDownToFloor()
    {
        if (this.RayCast3D(GlobalPosition, GlobalPosition + Vector3.Down * 3, out var hit))
        {
            Vector3 vector3 = (Vector3)this.areaCollision.Shape.Get("size");
            GlobalPosition = hit.position + Vector3.Up * vector3.Y / 2;
            Rotation = Vector3.Zero;
        }
    }

    public void EnableGlow()
    {
        foreach(MeshInstance3D meshInstance in meshInstances)
        {
            ShaderMaterial material = meshInstance.MaterialOverlay as ShaderMaterial;
            material.SetShaderParameter("emission", new Color(0.7f, 0.7f, 0.7f));
            material.SetShaderParameter("strenght", 1);
        }
    }

    public void DisableGlow()
    {
       
        foreach (MeshInstance3D meshInstance in meshInstances)
        {
            ShaderMaterial material = meshInstance.MaterialOverlay as ShaderMaterial;
            material.SetShaderParameter("emission", new Color(0.7f, 0.7f, 0.7f));
            material.SetShaderParameter("strenght", 0.1f);
        }
    }


    public bool Interact(PlayerController player)
    {
        if (!player.inventory.InventoryHasSpace()) {return false;}
        player.inventory.AddItemToInventory(item);
        sound.SetRandomPitch(0.8f, 1.2f);
        sound.PlaySound();
        QueueFree();
        return true;
    }
}
