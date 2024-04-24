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
            wait += delta;
            if(wait > 0.7 && !done)
            {
                done = true;
                ctx.weapon.EnableHitBoxes();
            }
            ctx.Velocity = Vector3.Zero;
        }

        public override void OnExit()
        {
            ctx.weapon.DisableHitBoxes();
        }
    }
}
