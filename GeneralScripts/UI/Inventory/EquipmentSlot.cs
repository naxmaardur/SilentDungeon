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
        Item = item;
        if (item == null)
        {
            Texture = null;
            return;
        }
        Texture = item.Texture;
        TooltipText = "a";
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (Item != null)
        {
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
        if (SlotType != 0)
        {
            if (passedData.Item.SlotType != SlotType)
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
                chestData.TooltipText = "";
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
            TooltipText = "a";
            return;
        }
        catch { }

        EquipmentSlot passedData = (EquipmentSlot)data;
        if (passedData != null)
        {
            if (Item == null)
            {
                passedData.Texture = null;
                passedData.TooltipText = "";
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
            TooltipText = "a";
        }
    }



    public override GodotObject _MakeCustomTooltip(string forText)
    {
        Node node = ResourceLoader.Load<PackedScene>("res://Prefabs/UI/ToolTip.tscn").Instantiate();
        ToolTip toolTip = node as ToolTip;
        toolTip.SetValues(Item);

        var styleBox = new StyleBoxFlat();
        styleBox.BgColor = (new Color(0.2f, 0.2f, 0.2f,0.4f));
        styleBox.BorderColor = new Color(0.87f, 0.75f, 0.062f);
        styleBox.SetBorderWidthAll(2);
        styleBox.ContentMarginLeft = 4;
        styleBox.ContentMarginRight = 4;
        styleBox.ContentMarginTop = 4;
        styleBox.ContentMarginBottom = 4;

        Theme = new Theme();
        Theme.SetStylebox("panel", "TooltipPanel", styleBox);

        return toolTip;
    }
}