using Godot;
using System;

namespace AIStates
{
	public partial class WanderState : State<ActorControler>
	{
        private Vector3 randomPointAroundPlayer;


        public override void OnEnter()
        {
            randomPointAroundPlayer = getRandomPoint();
        }

        public override void OnUpdate(double delta)
        {
            if (ctx.navigationAgent3D.GetCurrentNavigationPathIndex() == ctx.navigationAgent3D.GetCurrentNavigationPath().Length-1 && ctx.navigationAgent3D.GetNextPathPosition().DistanceTo(ctx.GlobalPosition) < 2)
            {
                randomPointAroundPlayer =  getRandomPoint();
            }
            if (ctx.GlobalPosition.DistanceTo(ctx.player.GlobalPosition) > 15)
            {
                randomPointAroundPlayer = getRandomPoint();
            }

            ctx.navigationAgent3D.TargetPosition = randomPointAroundPlayer;
            var nextNavPoint = ctx.navigationAgent3D.GetNextPathPosition();
            var desiredVelocity = (nextNavPoint - ctx.GlobalPosition).Normalized();
            ctx.navigationAgent3D.Velocity = desiredVelocity;
            double newAgle = Mathf.LerpAngle(ctx.Rotation.Y, Mathf.Atan2(desiredVelocity.X, desiredVelocity.Z), delta * 70);
            double radian = ((Math.PI / 180) * newAgle) - ((Math.PI / 180) * ctx.Rotation.Y);
            ctx.RotateY((float)radian);
        }


        private Vector3 getRandomPoint()
        {
            Vector3 playerPoint = ctx.GlobalPosition;
            Vector3 randomPoint = playerPoint + new Vector3(ctx.randomNumberGenerator.RandfRange(-15,15),0, ctx.randomNumberGenerator.RandfRange(-15, 15));
            return randomPoint;
        }
    }
}
