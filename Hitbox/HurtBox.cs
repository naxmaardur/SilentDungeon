using Godot;
using System;

public partial class HurtBox : Area3D
{
    public float dammage;
    public override void _Ready()
    {
        CollisionLayer = 2;
        CollisionMask = 0;
        AreaEntered += onEntered;
    }

    public override void _Process(double delta)
    {

    }

    private void onEntered(Area3D area3D)
    {
        HitBox hitBox = area3D as HitBox;
        if (hitBox == null) { return; }

        if (hitBox.Owner.HasMethod("take_damage"))
        {
            hitBox.Owner.Call("take_damage", dammage);
        }
    }
}
