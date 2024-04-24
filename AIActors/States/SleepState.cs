using Godot;
using System;

namespace AIStates
{
	public partial class SleepState : State<ActorControler>
	{

        public override void OnEnter()
        {
            ctx.alertValue = -2f;
        }

        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
        }
    }
}
