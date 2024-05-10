using Godot;
using System;

public partial class ItemContainer : Resource
{
    [Export]
    public InventoryItem[] Items = new InventoryItem[36];
    public bool mainChest { get; private set; }
    public Action<int> soldItem;
    private const string SAVEPATH = "user://chest.tres";

    public ItemContainer()
    {
        
    }
    public ItemContainer(bool LoadSave = false)
    {
        //Load Inventory of Save file if it exists
        if (LoadSave)
        {
            mainChest = true;
            Load();
        }
        else
        {
            //If Nothing to load
            Items = new InventoryItem[36];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = null;
            }
        }
    }

    public void AddItem(InventoryItem item)
    {
        //find first empty slot and add
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
            {
                Items[i] = item;
                return;
            }
        }
    }

    public void AddItem(InventoryItem item, int index)
    {
        Items[index] = item;
    }

    public void RemoveItem(int slotId)
    {
        Items[slotId] = null;
    }

    public void MoveItemInInventory(int currentSlot, int newSlot, bool OverWrite = false)
    {
        if (Items[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = Items[newSlot];
            MoveItemInInventory(newSlot, currentSlot, true);
            Items[newSlot] = oldItem;
        }
        else
        {
            Items[newSlot] = Items[currentSlot];
            if (!OverWrite)
            {
                Items[currentSlot] = null;
            }
        }
    }

    public void MoveItemToEquipeSlot(int currentSlot, int newSlot, ref Inventory inventory, bool OverWrite = false)
    {
        if (inventory.EquipedItems[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = Items[newSlot];
            MoveEquipeToContainer(newSlot, currentSlot, ref inventory, true);
            inventory.EquipedItems[newSlot] = oldItem;
            inventory.EquipmentUpdated?.Invoke();
        }
        else
        {
            inventory.EquipedItems[newSlot] = Items[currentSlot];
            if (!OverWrite)
            {
                Items[currentSlot] = null;
                inventory.EquipmentUpdated?.Invoke();
            }
        }
    }

    public void MoveEquipeToContainer(int currentSlot, int newSlot, ref Inventory inventory, bool OverWrite = false)
    {
        if (Items[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = inventory.EquipedItems[currentSlot];
            MoveItemToEquipeSlot(currentSlot, newSlot, ref inventory, true);
            Items[newSlot] = oldItem;
            inventory.EquipmentUpdated?.Invoke();
        }
        else
        {
            Items[newSlot] = inventory.EquipedItems[currentSlot];
            if (!OverWrite)
            {
                inventory.EquipedItems[currentSlot] = null;
                inventory.EquipmentUpdated?.Invoke();
            }
        }
    }

    public void MoveInventoryToContainer(int currentSlot, int newSlot, ref Inventory inventory, bool OverWrite = false)
    {
        if (Items[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = inventory.inventoryItems[currentSlot];
            MoveItemToInventory(currentSlot, newSlot, ref inventory, true);
            Items[newSlot] = oldItem;
        }
        else
        {
            Items[newSlot] = inventory.inventoryItems[currentSlot];
            if (!OverWrite)
            {
                inventory.inventoryItems[currentSlot] = null;
            }
        }
    }

    public void MoveItemToInventory(int currentSlot, int newSlot, ref Inventory inventory, bool OverWrite = false)
    {
        if (inventory.inventoryItems[newSlot] != null && !OverWrite)
        {
            InventoryItem oldItem = Items[newSlot];
            MoveInventoryToContainer(newSlot, currentSlot, ref inventory, true);
            inventory.inventoryItems[newSlot] = oldItem;
        }
        else
        {
            inventory.inventoryItems[newSlot] = Items[currentSlot];
            if (!OverWrite)
            {
                Items[currentSlot] = null;
            }
        }
    }

    public void Save()
    {
        ResourceSaver.Save(this, SAVEPATH);
    }

    public void Load()
    {
        if (ResourceLoader.Exists(SAVEPATH))
        {
            ItemContainer container = (ItemContainer)ResourceLoader.Load(SAVEPATH) ;
            Items = container.Items;

            return;
        }
        //If Nothing to load
        Items = new InventoryItem[36];
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = null;
        }
    }
}
