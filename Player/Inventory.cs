using Godot;
using System;

public partial class Inventory
{
	public InventoryItem[] inventoryItems;
	public InventoryItem[] EquipedItems = new InventoryItem[7];

	
	//EquipeSlots
	//0 hand R
	//1 hand L
	//2 head
	//3 body
	//4 feet
	//5 quick Potion
	//6 quick Spell
	//7 

	public Inventory() 
	{
		//Load Inventory of Save file if it exists
		Load();
    }

	public void AddItemToInventory(InventoryItem item)
	{
		//find first empty slot and add
		for(int i = 0; i < inventoryItems.Length; i++)
		{
			if (inventoryItems[i] == null)
			{
				inventoryItems[i] = item;
				return;
            }
		}
	}

	public void RemoveItemFromInventory(int slotId) 
	{
		inventoryItems[slotId] = null;
	}

	public void DropItemFromInventory(int slotId)
	{
        //SpawnItem near Player
        RemoveItemFromInventory(slotId);
        GD.PrintErr("Not Implemented: Inventory.DropItemFromInventory");
    }

    public void MoveItemInInventory(int currentSlot, int newSlot, bool OverWrite = false)
	{
        if (inventoryItems[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = inventoryItems[newSlot];
            MoveItemInInventory(newSlot, currentSlot, true);
            inventoryItems[newSlot] = oldItem;
        }
        else
        {
            inventoryItems[newSlot] = inventoryItems[currentSlot];
        }
    }

	public void MoveItemInEquipeSlot(int currentSlot, int newSlot, bool OverWrite = false)
	{
        if (EquipedItems[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = EquipedItems[newSlot];
            MoveItemInEquipeSlot(newSlot, currentSlot, true);
            EquipedItems[newSlot] = oldItem;
        }
        else
        {
            EquipedItems[newSlot] = EquipedItems[currentSlot];
        }
    }


	public void MoveItemToEquipeSlot(int currentSlot, int newSlot, bool OverWrite = false)
	{
		if (EquipedItems[newSlot] != null && !OverWrite)
		{
            InventoryItem oldItem = inventoryItems[newSlot];
			MoveEquipeToInventory(newSlot, currentSlot, true);
			EquipedItems[newSlot] = oldItem;
		}
		else
		{
            EquipedItems[newSlot] = inventoryItems[currentSlot];
        }
    }

    public void MoveEquipeToInventory(int currentSlot, int newSlot, bool OverWrite = false)
	{
        if (inventoryItems[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = EquipedItems[currentSlot];
            MoveItemToEquipeSlot(currentSlot, newSlot, true);
            inventoryItems[newSlot] = oldItem;
        }
		else
		{
            inventoryItems[newSlot] = EquipedItems[currentSlot];
        }
    }

	public bool InventoryHasSpace()
	{
		for (int i = 0; i < inventoryItems.Length; i++)
		{
			if (inventoryItems[i] == null) { return true; }
		}
		return false;
	}

	public void Save()
	{
        GD.PrintErr("Not Implemented: Inventory.Save");
    }

	public void Load() 
	{
		//If Nothing to load
		inventoryItems = new InventoryItem[36];
		for (int i = 0;i < inventoryItems.Length; i++)
		{
			inventoryItems[i] = null;
		}

        GD.PrintErr("Not Implemented: Inventory.Load");
    }
}
