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

	private SoundSource SoundSourceR;

    private void AttackSetup()
	{
		weaponsRight = rightHand.GetAllChildrenByType<Weapon>();
        SoundSourceR = rightHand.GetChildByType<SoundSource>();
        if (weaponsRight.Length == 0)
		{
			GD.PrintErr("Weapons Right Array is empty");
		}
        SetWeaponRight(0);
        foreach (Weapon weapon in weaponsRight)
		{
			weapon.SetOwner(this);
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
        SetAttack(rightHandTree, false);
        rightHandTree.Set("parameters/conditions/AttackBounce", true);
        SoundSourceR.SetRandomPitch(0.7f, 1.2f);
        SoundSourceR.PlaySound();
    }
}
