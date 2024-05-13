using Godot;
using System;
using System.Security.Cryptography;

namespace AIStates
{
	public partial class AttackState : State<ActorControler>
	{
        double wait = 0;
        bool done;
        public override void OnEnter()
        {
            ctx.navigationAgent3D.Velocity = Vector3.Zero;
            wait = 0;
            done = false;
            float f =  (float)ctx.randomNumberGenerator.RandiRange(0, 5) / 10;
            GD.Print(f);
            ctx.tree.Set("parameters/Alive/RandomAttack/blend_position", f);
            ctx.tree.Set("parameters/Alive/OneShotAttack/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            
        }

        public override void OnUpdate(double delta)
        {
            if (ctx.isWarden)
            {
                Vector3 direction = ctx.GlobalPosition.DirectionTo(ctx.player.GlobalPosition);

                double newAgle = Mathf.LerpAngle(ctx.Rotation.Y, Mathf.Atan2(direction.X, direction.Z), 1);
                double radian = ((Math.PI / 180) * newAgle) - ((Math.PI / 180) * ctx.Rotation.Y);
                ctx.RotateY((float)radian);
            }
            wait += delta;
            if(wait > ctx.attackTime && !done)
            {
                done = true;
                ctx.weapon.EnableHitBoxes();
                if(ctx.isWarden) 
                {
                    ctx.player.TakeDamage(35); // you can't run
                }
            }
            ctx.Velocity = Vector3.Zero;
        }

        public override void OnExit()
        {
            ctx.weapon.DisableHitBoxes();
        }
    }
}
