using Godot;

public class CrouchState : State<PlayerController>
{
    private float speed = 2.5f;

    public override void OnPhysicsUpdate(double delta)
    {
        Vector3 tempVelocity = ctx.Velocity;

        // Add the gravity.
        if (!ctx.IsOnFloor())
        {
            tempVelocity.Y -= ctx.gravity * (float)delta;
        }

        if (ctx.inputsActive)
        {
            // Handle Jump.
            if (Input.IsActionJustPressed("jump") && ctx.IsOnFloor())
            {
                tempVelocity.Y = ctx.jumpVelocity;
            }
        }

        // Get the input direction and handle the movement/deceleration.
        Vector3 direction = (ctx.Transform.Basis * new Vector3(ctx.inputDir.X, 0, ctx.inputDir.Y)).Normalized();

        if (ctx.IsOnFloor())
        {
            if (direction != Vector3.Zero)
            {
                tempVelocity.X = direction.X * speed * (ctx.SpeedMod +ctx.SneakSpeedMod);
                tempVelocity.Z = direction.Z * speed * (ctx.SpeedMod + ctx.SneakSpeedMod);
            }
            else
            {
                tempVelocity.X = (float)Mathf.Lerp(tempVelocity.X, direction.X * speed * (ctx.SpeedMod + ctx.SneakSpeedMod), delta * 7.0f);
                tempVelocity.Z = (float)Mathf.Lerp(tempVelocity.Z, direction.Z * speed * (ctx.SpeedMod + ctx.SneakSpeedMod), delta * 7.0f);
            }
        }
        else
        {
            tempVelocity.X = (float)Mathf.Lerp(tempVelocity.X, direction.X * speed * (ctx.SpeedMod + ctx.SneakSpeedMod), delta * 3.0f);
            tempVelocity.Z = (float)Mathf.Lerp(tempVelocity.Z, direction.Z * speed * (ctx.SpeedMod + ctx.SneakSpeedMod), delta * 3.0f);
        }

        ctx.Velocity = tempVelocity;

        ctx.MoveAndSlide();
    }

    public override void OnEnter()
    {
        ctx.CameraHeightAnimation.Play("StandingToCrouch");
        ctx.crouchingCollisionShape.Disabled = false;
        ctx.standingCollisionShape.Disabled = true;
        ctx.sneaking = true;
    }

    public override void OnExit()
    {
        ctx.CameraHeightAnimation.PlayBackwards("StandingToCrouch");
        ctx.standingCollisionShape.Disabled = false;
        ctx.crouchingCollisionShape.Disabled = true;
        ctx.sneaking = false;
    }
}