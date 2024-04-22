using Godot;
using System;

namespace AIStates
{
	public partial class IdleState : State<ActorControler>
	{
        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
        }
    }
}
