using Godot;
using System;

public partial class DropItem : Control
{
	[Export]
	private PackedScene dropItem;
    private PlayerController player;
    private GameManager gameManager;
    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup("player")[0] as PlayerController;
        gameManager = GetTree().Root.GetChildByType<GameManager>();

    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return true;
    }


    public override void _DropData(Vector2 atPosition, Variant data)
    {
        EquipmentSlot passedData = (EquipmentSlot)data;
        if (passedData != null)
        {
            passedData.Texture = null;
            InventoryItem item = passedData.Item;
            passedData.Item = null;
            Node n = dropItem.Instantiate();
            PickUpItem pickUpItem = n as PickUpItem;
            pickUpItem.item = item;
            Node v = item.ObjectMesh.Instantiate();
            Node3D visuals = v as Node3D;
            pickUpItem.AddChild(visuals);
            pickUpItem.AddMesh(visuals);
            gameManager.activeSceneContainer.GetChild(0).AddChild(pickUpItem);
            pickUpItem.GlobalPosition = player.GlobalPosition + player.Forward();

            ChestSlot chestdata = passedData as ChestSlot;
            if(chestdata != null)
            {
                chestdata.Container.RemoveItem(passedData.Slot);
                return;
            }
            if (passedData.IsEquipSlot)
            {
                player.inventory.RemoveItemFromEquipeSlot(passedData.Slot);
            }
            else
            {
                player.inventory.RemoveItemFromInventory(passedData.Slot);
            }
        }
    }
}
