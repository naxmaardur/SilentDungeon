using Godot;
using System;

public partial class PlayerController
{

    private void SetupTransitions()
    {
        stateMachine.AddTransition(new Transition(typeof(WalkState), typeof(CrouchState), CrouchValid));
        stateMachine.AddTransition(new Transition(typeof(CrouchState), typeof(WalkState), ()=> !CrouchValid()));
    }

    private bool CrouchValid()
    {
        return crouchToggle;
    }


}
