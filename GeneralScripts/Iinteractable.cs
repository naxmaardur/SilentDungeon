using Godot;
using System;

public interface Iinteractable
{

	public void EnableGlow();
	public void DisableGlow();

	public bool Interact(PlayerController player);

}
