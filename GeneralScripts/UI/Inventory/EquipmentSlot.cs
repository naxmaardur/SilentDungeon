using Godot;
using Godot.NativeInterop;
using System;

public partial class EquipmentSlot : TextureRect
{
    [Export]
    public InventoryItem Item { get; set; }
    [Export]
    public int Slot { get; set; }
    [Export]
    public int SlotType { get; set; }
    [Export]
    public bool IsEquipSlot { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SetItem(InventoryItem item)
    {
        GD.Print(item);
        Item = item;
        if(item == null)
        {
            Texture = null;
            return;
        }
        Texture = item.Texture;
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if(Item != null) { 
            var data = this;

            var rect = new TextureRect();

            rect.Texture = Texture;
            rect.ExpandMode = ExpandModeEnum.FitWidth;
            rect.Size = Size;
		    Control control = new Control();
		    control.AddChild(rect);
		    rect.Position = -0.5f * rect.Size;
		    SetDragPreview(control);

            return data;
        }
        return new Variant();
    }



    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        EquipmentSlot passedData = (EquipmentSlot)data;
        if (passedData == null) { return false; }

        //check if this slot is valid for this item
        if(SlotType != 0) 
        { 
            if(passedData.Item.SlotType != SlotType)
            {
                return false;
            }
        }

        //Swap validation check
        if (Item != null)
        {
            if (SlotType != 0)
            {
                if (Item.SlotType != passedData.Item.SlotType)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        try
        {
            ChestSlot chestData = (ChestSlot)data;
            if (Item == null)
            {
                chestData.Texture = null;
            }
            else
            {
                chestData.Texture = Item.Texture;
            }

            InventoryItem item = chestData.Item;
            chestData.Item = Item;
            Item = item;
            Texture = item.Texture;
            PlayerController player = GetTree().GetNodesInGroup("player")[0] as PlayerController;

            if (IsEquipSlot)
            {
                chestData.Container.MoveItemToEquipeSlot(chestData.Slot, Slot, ref player.inventory);
            }
            if (!IsEquipSlot)
            {
                chestData.Container.MoveItemToInventory(chestData.Slot, Slot, ref player.inventory);
            }
            return;
        }
        catch { }

        EquipmentSlot passedData = (EquipmentSlot)data;
        if (passedData != null)
        {
            if (Item == null)
            {
                passedData.Texture = null;
            }
            else
            {
                passedData.Texture = Item.Texture;
            }
            InventoryItem item = passedData.Item;
            passedData.Item = Item;
            Item = item;
            Texture = item.Texture;
            PlayerController player = GetTree().GetNodesInGroup("player")[0] as PlayerController;

            if (passedData.IsEquipSlot && IsEquipSlot)
            {
                player.inventory.MoveItemInEquipeSlot(passedData.Slot, Slot);
            }
            if (passedData.IsEquipSlot && !IsEquipSlot)
            {
                player.inventory.MoveEquipeToInventory(passedData.Slot, Slot);
            }
            if (!passedData.IsEquipSlot && IsEquipSlot)
            {
                player.inventory.MoveItemToEquipeSlot(passedData.Slot, Slot);
            }
            if (!passedData.IsEquipSlot && !IsEquipSlot)
            {
                player.inventory.MoveItemInInventory(passedData.Slot, Slot);
            }
        }
    }

   
}