using Godot;
using System;

public partial class PlayerController
{
    public Vector2 inputDir { get; private set; }

    private void HandleInputs()
    {
        inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    }
}
