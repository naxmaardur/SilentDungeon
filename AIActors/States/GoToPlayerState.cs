using Godot;
using System;

namespace AIStates
{
	public partial class GotToPlayerState : State<ActorControler>
	{
        public override void OnEnter()
        {
            ctx.runLerp.setTarget(1);
            ctx.navigationAgent3D.MaxSpeed = 10;
            if(ctx.alertValue < 10)
            {
                ctx.alertValue = 10;
            }
        }

        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
            ctx.navigationAgent3D.TargetPosition = ctx.player.GlobalPosition;
            var nextNavPoint = ctx.navigationAgent3D.GetNextPathPosition();
            var desiredVelocity = (nextNavPoint - ctx.GlobalPosition).Normalized() * ctx.runSpeed;
            ctx.navigationAgent3D.Velocity = desiredVelocity;
            double newAgle = Mathf.LerpAngle(ctx.Rotation.Y, Mathf.Atan2(desiredVelocity.X, desiredVelocity.Z), delta * 70);
            double radian = ((Math.PI / 180) * newAgle) - ((Math.PI / 180) * ctx.Rotation.Y);
            ctx.RotateY((float)radian);
        }

        public override void OnExit() 
        {
            ctx.runLerp.setTarget(0);
            ctx.navigationAgent3D.MaxSpeed = 3;
        }
    }
}
