using Godot;
using System;

public partial class Weapon : Node3D
{
	private HurtBox[] hurtBoxes;
    private Area3D[] areaBoxes;
    [Export]
    public weaponType weaponType { get; private set; }

    public Action HitNonHitBox;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        areaBoxes = this.GetAllChildrenByType<Area3D>();
        hurtBoxes = this.GetAllChildrenByType<HurtBox>();
       
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void EnableHitBoxes()
    {
        foreach (HurtBox box in hurtBoxes)
        {
            box.canHit = true;
        }
        foreach (var item in areaBoxes)
        {
            item.BodyEntered += onEntered;
        }
    }

    public void DisableHitBoxes()
    {
        foreach (HurtBox box in hurtBoxes)
        {
            box.canHit = false;
            box.clearList();
        }
        foreach (var item in areaBoxes)
        {
            item.BodyEntered -= onEntered;
        }
    }

    private void onEntered(Node3D node3D)
    {
        if (node3D == hurtBoxes[0].BoxOwner) { return; }
        HitBox hitBox = node3D as HitBox;
        HurtBox hurtBox = node3D as HurtBox;
        if(hitBox == null && hurtBox == null)
        {
            HitNonHitBox.Invoke();
            DisableHitBoxes();
        }
    }
}

public enum weaponType{
    sword,
    axe
}
