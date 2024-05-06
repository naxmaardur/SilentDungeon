using Godot;
using System;

public partial class UIInvetory : Control
{
	private EquipmentSlot[] inventorySlots = new EquipmentSlot[0];
    private EquipmentSlot[] equipmentSlots;

	private const string InvSlotScenePath = "res://Prefabs/UI/InvSlot.tscn";
	private PackedScene invSlotScene;
	private PlayerController player;
    [Export]
	private Node GridContainer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        invSlotScene = GD.Load<PackedScene>(InvSlotScenePath);
        player = GetTree().GetNodesInGroup("player")[0] as PlayerController;
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OpenInventory()
	{
		if (inventorySlots.Length == 0)
		{
            OpenInventoryFirstTime();

        }
        for (int i = 0; i < 36; i++)
        {
            GD.Print(player.inventory.inventoryItems[i]);
            SetItemInInventorySlot(player.inventory.inventoryItems[i], i);
        }

        for(int i = 0; i < 7; i++)
        {
            SetItemInEquipmentSlot(player.inventory.EquipedItems[i], i);
        }
    }

	private void OpenInventoryFirstTime()
	{
        foreach (Node n in GridContainer.GetChildren())
        {
            GridContainer.RemoveChild(n);
            n.Free();
        }
        equipmentSlots = this.GetAllChildrenByType<EquipmentSlot>();
        inventorySlots = new EquipmentSlot[36];
        for (int i = 0; i < 36; i++)
        {
            Node scene = invSlotScene.Instantiate();
            GridContainer.AddChild(scene);
            EquipmentSlot slot = scene.GetChildByType<EquipmentSlot>();
            inventorySlots[i] = slot;
            slot.Slot = i;
        }
    }

	public void SetItemInInventorySlot(InventoryItem item, int slot)
	{
		inventorySlots[slot].SetItem(item);
	}
    public void SetItemInEquipmentSlot(InventoryItem item, int slot)
    {
        equipmentSlots[slot].SetItem(item);
    }
}
