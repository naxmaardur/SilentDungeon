using Godot;
using System;

public partial class HitBox : Area3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CollisionLayer = 2;
        CollisionMask = 0;
    }
}
