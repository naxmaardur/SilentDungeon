using Godot;
using System;

namespace AIStates
{
	public partial class IdleState : State<ActorControler>
	{
        public override void OnUpdate(double delta)
        {
            ctx.navigationAgent3D.TargetPosition = ctx.startLocation;
            var nextNavPoint = ctx.navigationAgent3D.GetNextPathPosition();
            var desiredVelocity = (nextNavPoint - ctx.GlobalPosition).Normalized();
            ctx.navigationAgent3D.Velocity = desiredVelocity;
            double newAgle = Mathf.LerpAngle(ctx.Rotation.Y, Mathf.Atan2(desiredVelocity.X, desiredVelocity.Z), delta * 70);
            double radian = ((Math.PI / 180) * newAgle) - ((Math.PI / 180) * ctx.Rotation.Y);
            ctx.RotateY((float)radian);
        }
    }
}
