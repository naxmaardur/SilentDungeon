using Godot;
using System;
using System.ComponentModel;

public partial class UIItemContainer : Control
{
    public ItemContainer currentContainer;
    private ChestSlot[] slots;



    // Called when the node enters the scene tree for the first time.
    public void Setup()
    {
        slots = this.GetAllChildrenByType<ChestSlot>();
    }

    public void OpenContainer(ItemContainer container)
    {
        currentContainer = container;
        for (int i = 0; i < 36; i++)
        {
            slots[i].Container = currentContainer;
            SetItemInInventorySlot(container.Items[i], i);
        }
    }

    public void SetItemInInventorySlot(InventoryItem item, int slot)
    {
        slots[slot].SetItem(item);
    }
}
