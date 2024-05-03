using Godot;
using System;

public partial class UIInvetory : Node
{
	private EquipmentSlot[] equipmentSlots;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		equipmentSlots = this.GetAllChildrenByType<EquipmentSlot>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetItemInInventorySlot(InventoryItem item, int slot)
	{
		equipmentSlots[slot].SetItem(item);
	}
}
