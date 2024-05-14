using Godot;
using System;

public partial class ItemSpawner : Node3D
{
    [Export]
    private LootTable lootTable;
    [Export]
    private PackedScene dropItem;

    public override void _Process(double delta)
    {
        Node n = dropItem.Instantiate();
        PickUpItem pickUpItem = n as PickUpItem;
        pickUpItem.item = lootTable.RollDrop();
        Node v = pickUpItem.item.ObjectMesh.Instantiate();
        Node3D visuals = v as Node3D;
        pickUpItem.AddChild(visuals);
        pickUpItem.AddMesh(visuals);
        GetParent().AddChild(pickUpItem);
        pickUpItem.GlobalPosition = GlobalPosition;
        QueueFree();
    }
}
