using Godot;
using System;

namespace AIStates
{
	public partial class BackOffFromPlayer : State<ActorControler>
	{

        public override void OnEnter()
        {
            ctx.backOffTimer.Start();
        }
        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
            ctx.navigationAgent3D.TargetPosition = ctx.GlobalPosition + -ctx.Forward() * 4;
            var nextNavPoint = ctx.navigationAgent3D.GetNextPathPosition();
            ctx.navigationAgent3D.Velocity = (nextNavPoint - ctx.GlobalPosition).Normalized();
            Vector3 directionToPlayer = (ctx.player.GlobalPosition - ctx.GlobalPosition).Normalized();
            double newAgle = Mathf.LerpAngle(ctx.Rotation.Y, Mathf.Atan2(directionToPlayer.X, directionToPlayer.Z), delta * 70);
            double radian = ((Math.PI / 180) * newAgle) - ((Math.PI / 180) * ctx.Rotation.Y);
            ctx.RotateY((float)radian);
        }
    }
}
