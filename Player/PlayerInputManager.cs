using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerController
{
    public Vector2 inputDir { get; private set; }

    private void HandleInputs()
    {
        inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if (Input.IsActionJustPressed("crouch"))
        {
            if(crouchToggle)
            {
                List<Node> nodes = new();
                int childCount = this.GetChildCount();

                for (int i = 0; i < childCount; i++)
                {
                    Node child = this.GetChild(i);
                    nodes.Add(child);
                }
                foreach (var node3D in headBox.GetOverlappingBodies()) 
                {
                    
                    if (!nodes.Contains(node3D) && node3D != this)
                    {
                        return;
                    }

                }

            }
            crouchToggle = !crouchToggle;
        }

        if (Input.IsActionJustPressed("interact"))
        {
            Interact();
        }

    }
}
