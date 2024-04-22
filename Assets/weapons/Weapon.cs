using Godot;
using System;
using System.Collections.Generic;

public partial class Weapon : Node3D
{
    [Export]
    private float damage = 1;
	private HurtBox[] hurtBoxes;
    private Area3D[] areaBoxes;
    [Export]
    public weaponType weaponType { get; private set; }

    public Action HitNonHitBox;
    public List<Node> hitNodes;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        hitNodes = new();
        areaBoxes = this.GetAllChildrenByType<Area3D>();
        hurtBoxes = this.GetAllChildrenByType<HurtBox>();
        foreach (var item in areaBoxes)
        {
            item.BodyEntered += onEntered;
        }
    }
    public void ClearList()
    {
        hitNodes.Clear();
    }

    public void SetOwner(Node node)
    {
        foreach (var item in hurtBoxes)
        {
            item.damage = damage;
            item.Weapon = this;
            item.BoxOwner = node;
        }
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void EnableHitBoxes()
    {
        GD.Print("Enable");
        foreach (HurtBox box in hurtBoxes)
        {
            box.canHit = true;
            box.Monitoring = true;
        }
    }

    public void DisableHitBoxes()
    {
        foreach (HurtBox box in hurtBoxes)
        {
            box.Monitoring = false;
            box.canHit = false;
        }
        ClearList();
    }

    private void onEntered(Node3D node3D)
    {
        if (!hurtBoxes[0].canHit) { return; }
        if (node3D == hurtBoxes[0].BoxOwner) { return; }
        HitBox hitBox = node3D as HitBox;
        HurtBox hurtBox = node3D as HurtBox;
        if(hitBox == null && hurtBox == null)
        {
            if (HitNonHitBox != null)
            {
                HitNonHitBox?.Invoke();
                DisableHitBoxes();
            }
        }
    }
}

public enum weaponType{
    sword,
    axe
}
