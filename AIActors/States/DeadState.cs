using Godot;
using System;

namespace AIStates
{
	public partial class DeadState : State<ActorControler>
	{
        public override void OnEnter()
        {
            ctx.tree.Set("parameters/conditions/Dead", true);
            Node scene = ctx.LootScene.Instantiate();
            Node3D node3D = scene as Node3D;
            if (node3D != null)
            {
                node3D.GlobalPosition = ctx.GlobalPosition;
                ctx.GetParent().AddChild(node3D);
                ctx.visuals.Reparent(node3D);
                ctx.QueueFree();
            }
        }


        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
        }
    }
}
