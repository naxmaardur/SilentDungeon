using Godot;
using System;
using System.Collections.Generic;

public partial class HurtBox : Area3D
{
    public float damage;
    public Node BoxOwner;
    public bool canHit = false;
    private List<Node> hitNodes;

    public override void _Ready()
    {
        hitNodes = new();
        CollisionLayer = 0;
        CollisionMask = 3;
        AreaEntered += onEntered;
    }

    public override void _Process(double delta)
    {

    }

    public void clearList()
    {
        hitNodes.Clear();
    }

    private void onEntered(Area3D area3D)
    {
        if (!canHit) { return; }
        HitBox hitBox = area3D as HitBox;
        if (hitBox == null) { return; }
        if (area3D.Owner == BoxOwner) { return; }
        if (hitNodes.Contains(hitBox.Owner)) { return; }
        hitNodes.Add(hitBox.Owner);
        IDamagable damagable = hitBox.Owner as IDamagable;
        if (damagable != null)
        {
            damagable.TakeDamage(damage);
        }
    }
}
