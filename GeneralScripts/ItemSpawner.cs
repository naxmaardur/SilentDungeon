using Godot;
using System;

public partial class ItemSpawner : Node3D
{
    [Export]
    private LootTable lootTable;
    [Export]
    private PackedScene dropItem; 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Node n = dropItem.Instantiate();
        PickUpItem pickUpItem = n as PickUpItem;
        pickUpItem.item = lootTable.RollDrop();
        Node v = pickUpItem.item.ObjectMesh.Instantiate();
        Node3D visuals = v as Node3D;
        pickUpItem.AddChild(visuals);
        pickUpItem.AddMesh(visuals);
        this.GetParent().AddChild(pickUpItem);
        pickUpItem.GlobalPosition = GlobalPosition;
        QueueFree();
    }
}
