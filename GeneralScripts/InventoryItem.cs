using Godot;
using System;

[GlobalClass]
public partial class InventoryItem : Resource
{
    [Export]
    public Texture2D Texture { get; set; }
    [Export]
    public int SlotType { get; set; }
}
