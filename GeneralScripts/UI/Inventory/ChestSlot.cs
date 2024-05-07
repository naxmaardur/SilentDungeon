using Godot;
using System;

public partial class ChestSlot : EquipmentSlot
{
    public ItemContainer Container { get; set; }

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

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        try 
        {
            ChestSlot chestData = (ChestSlot)data;
            InventoryItem item = chestData.Item;

            if (Item == null)
            {
                chestData.Texture = null;
            }
            else
            {
                chestData.Texture = Item.Texture;
            }
            chestData.Item = Item;
            Item = item;
            Texture = item.Texture;
            chestData.Container.MoveItemInInventory(chestData.Slot, Slot);
            return;
        } catch { }

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

            if (passedData.IsEquipSlot)
            {
                Container.MoveEquipeToContainer(passedData.Slot, Slot, ref player.inventory);
            }
            if (!passedData.IsEquipSlot)
            {
                Container.MoveInventoryToContainer(passedData.Slot, Slot, ref player.inventory);
            }
        }
    }
}
