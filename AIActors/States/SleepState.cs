using Godot;
using System;

namespace AIStates
{
	public partial class SleepState : State<ActorControler>
	{

        public override void OnEnter()
        {
            ctx.AlertValue = -2f;
            ctx.tree.Set("parameters/conditions/Sleeping", true);
            ctx.tree.Set("parameters/conditions/Awake", false);
        }

        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
        }

        public override void OnExit()
        {
            ctx.tree.Set("parameters/conditions/Sleeping", false);
            ctx.tree.Set("parameters/conditions/Awake", true);
        }
    }
}
