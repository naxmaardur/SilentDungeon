using Godot;
using System;

public partial class PlayerController
{
	private Iinteractable interactable;
	private GodotObject godotObject;

	private void CheckForInteractable()
	{
        if (this.RayCast3D(camera.GlobalPosition, camera.GlobalPosition + (camera.Forward() * 4f),out var hitInfo, 128))
		{
            Iinteractable iinteractable = hitInfo.collider as Iinteractable;
			GodotObject godotObject = hitInfo.collider;

            if (iinteractable == null)
			{
				Node node = hitInfo.collider as Node;
                iinteractable = node.Owner as Iinteractable;
                godotObject = node.Owner;
            }
            if (iinteractable != null)
			{
                if (interactable != iinteractable)
				{
					DisableOldInteractable();
                    interactable = iinteractable;
					this.godotObject = godotObject;

                    interactable.EnableGlow();
                }
			}
			else
			{
				DisableOldInteractable();
            }
		}
		else
		{
			DisableOldInteractable();
        }
	}


	private void DisableOldInteractable()
	{
		if(interactable == null) { return; }
		if (!IsInstanceValid(godotObject)) { interactable = null; godotObject = null; return; }
		interactable.DisableGlow();
		interactable = null;
	}

	private void Interact()
	{
		if(interactable == null) { return ; }
        if (!IsInstanceValid(godotObject)) { interactable = null; godotObject = null; return; }
        interactable.Interact(this);
	}
}
