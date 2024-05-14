using Godot;
using System;

namespace AIStates
{
	public partial class OrbitPlayer : State<ActorControler>
	{
        private Vector3 destination;
        float radius = 1.5f;
        double waitOnSpot = 2;
        double waited = 0;
        public override void OnEnter()
        {
            ctx.inRangeTimer.Start();
            GetNewRandomPosition();
            ctx.navigationAgent3D.TargetPosition = destination;
        }
        public override void OnUpdate(double delta)
        {
            ctx.Velocity = Vector3.Zero;
            Vector3 directionToPlayer = (ctx.player.GlobalPosition - ctx.GlobalPosition).Normalized() * ctx.walkSpeed;
            double newAgle = Mathf.LerpAngle(ctx.Rotation.Y, Mathf.Atan2(directionToPlayer.X, directionToPlayer.Z), delta * 70);
            double radian = ((Math.PI / 180) * newAgle) - ((Math.PI / 180) * ctx.Rotation.Y);
            ctx.RotateY((float)radian);
            if (ctx.GlobalPosition.DistanceTo(destination) < 1f)
            {
                waited += delta;
                if(waited > waitOnSpot)
                {
                    GetNewRandomPosition();
                    ctx.navigationAgent3D.TargetPosition = destination;
                    waited = 0;
                }
                else
                {
                    ctx.Velocity = Vector3.Zero;
                    return;
                }
            }

            var nextNavPoint = ctx.navigationAgent3D.GetNextPathPosition();
            var desiredVelocity = (nextNavPoint - ctx.GlobalPosition).Normalized();
            ctx.navigationAgent3D.Velocity = desiredVelocity;
        }


        private void GetNewRandomPosition()
        {
            Vector3 currentRelativePosition = ctx.GlobalPosition - ctx.player.GlobalPosition;
            float distance = ctx.GlobalPosition.DistanceTo(ctx.player.GlobalPosition);
            if (distance > radius)
            {
                currentRelativePosition -=  ctx.Forward();
            }
            if(distance < radius)
            {
                currentRelativePosition += ctx.Forward();
            }

            switch(ctx.randomNumberGenerator.RandiRange(0, 3))
            {
                case 0:
                    currentRelativePosition -= ctx.Right();
                    break;
                case 1:
                    currentRelativePosition += ctx.Right();
                break;
            }
            destination = currentRelativePosition + ctx.player.Position;
        }
    }
}
