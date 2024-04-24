using Godot;
using System;

namespace AIStates
{
	public partial class InvestigateState : State<ActorControler>
	{
        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
        }
    }
}
