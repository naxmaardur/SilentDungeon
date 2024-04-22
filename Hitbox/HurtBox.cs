using Godot;
using System;
using System.Collections.Generic;

public partial class HurtBox : Area3D
{
    public float damage;
    public Node BoxOwner;
    public bool canHit = false;
    public Weapon Weapon;
    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = 3;
        Monitoring = false;
        AreaEntered += onEntered;
    }

    public override void _Process(double delta)
    {

    }

    

    private void onEntered(Area3D area3D)
    {
        if (area3D.Owner == BoxOwner) { return; }
        if (!canHit) { return; }
        HitBox hitBox = area3D as HitBox;
        if (hitBox == null) { return; }
        if (Weapon.hitNodes.Contains(hitBox.Owner)) { return; }
        Weapon.hitNodes.Add(hitBox.Owner);
        IDamagable damagable = hitBox.Owner as IDamagable;
        if (damagable != null)
        {
            damagable.TakeDamage(damage);
        }
    }
}
