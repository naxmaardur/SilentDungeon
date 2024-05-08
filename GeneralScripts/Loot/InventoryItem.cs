using Godot;
using System;

[GlobalClass]
public partial class InventoryItem : Resource
{
    [Export]
    public Texture2D Texture { get; set; }
    [Export]
    public int SlotType { get; set; }

    [Export]
    public PackedScene ObjectMesh { get; set; }

    [Export]
    public int WeaponID;

    [Export]
    public int ItemID { get; set; }

    [Export]
    public float SoundMod { get; set; }

    [Export]
    public float SpeedMod { get; set; }

    [Export]
    public float Protection {  get; set; }

    [Export]
    public float Speed { get; set; }



}
