using Godot;
using Godot.Collections;
using System;

public partial class PlayerController
{
	[ExportCategory("Hand Trees")]
	[Export]
	private AnimationTree rightHandTree;
	[Export]
	private AnimationTree leftHandTree;
	private Weapon[] weaponsRight;
	private Weapon activeWeaponRight;

	[Export]
	private Node3D rightHand;
	[Export]
	private Node3D leftHand;

	private AudioStreamPlayer3D AudioplayerR;

    private void AttackSetup()
	{
		weaponsRight = rightHand.GetAllChildrenByType<Weapon>();
        AudioplayerR = rightHand.GetChildByType<AudioStreamPlayer3D>();
        if (weaponsRight.Length == 0)
		{
			GD.PrintErr("Weapons Right Array is empty");
		}
        SetWeaponRight(1);
        HurtBox[] hurtBoxes = this.GetAllChildrenByType<HurtBox>();
        foreach (HurtBox box in hurtBoxes)
		{
			box.BoxOwner = this;
		}
	}

	public void SetWeaponRight(int id)
	{
		if (activeWeaponRight != null)
		{
			activeWeaponRight.Visible = false;
			activeWeaponRight.HitNonHitBox -= RightAttackBounceback;
		}
        activeWeaponRight = weaponsRight[id];
        activeWeaponRight.Visible = true;
        activeWeaponRight.HitNonHitBox += RightAttackBounceback;
        switch (activeWeaponRight.weaponType)
		{
			case weaponType.sword:
                rightHandTree.Set("parameters/conditions/HasAxe", false);
                rightHandTree.Set("parameters/conditions/HasSword", true);
                break;
			case weaponType.axe:
                rightHandTree.Set("parameters/conditions/HasAxe", true);
                rightHandTree.Set("parameters/conditions/HasSword", false);
                break;
		}
    }

	public void EnableHitBoxesRight()
	{
        activeWeaponRight.EnableHitBoxes();
	}

	public void DisableHitBoxesRight() 
	{
        activeWeaponRight.DisableHitBoxes();
    }


    private void attackProcess(double delta)
	{
		if (leftHandTree != null)
		{

		}
		if (rightHandTree != null)
		{
            if (Input.IsActionJustPressed("attack"))
			{
				SetAttack(rightHandTree, true);
			}

			if (Input.IsActionJustReleased("attack"))
			{
				SetAttack(rightHandTree, false);
			}
		}
	}

	private void SetAttack(AnimationTree tree, bool b)
	{
		tree.Set("parameters/conditions/Attack", b);
		tree.Set("parameters/conditions/AttackShouldStop", !b);
        tree.Set("parameters/conditions/AttackBounce", false);
    }

	private void RightAttackBounceback()
	{
		AudioplayerR.PitchScale = numberGenerator.RandfRange(0.7f, 1.2f);
        AudioplayerR.Play();
		SetAttack(rightHandTree, false);
        rightHandTree.Set("parameters/conditions/AttackBounce", true);
    }
}
