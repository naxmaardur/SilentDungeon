using Godot;

public class WalkState : State<PlayerController>
{
    private float speed;

    public override void OnPhysicsUpdate(double delta)
    {
        Vector3 tempVelocity = ctx.Velocity;

        // Add the gravity.
        if (!ctx.IsOnFloor())
        {
            tempVelocity.Y -= ctx.gravity * (float)delta;
        }

        // Handle Jump.
        if (ctx.inputsActive)
        {
            if (Input.IsActionJustPressed("jump") && ctx.IsOnFloor())
            {
                tempVelocity.Y = ctx.jumpVelocity;
            }

            // Handle Sprint.
            if (Input.IsActionJustPressed("run"))
            {
                speed = ctx.sprintSpeed * (ctx.SpeedMod + ctx.RunSpeedMod);
                ctx.stepSource.soundValue = 5;
            }
            
            if(Input.IsActionJustReleased("run"))
            {
                speed = ctx.walkSpeed * ctx.SpeedMod;
                ctx.stepSource.soundValue = 4;
            }
        }

        // Get the input direction and handle the movement/deceleration.
        Vector3 direction = (ctx.Transform.Basis * new Vector3(ctx.inputDir.X, 0, ctx.inputDir.Y)).Normalized();

        if (ctx.IsOnFloor())
        {
            if (direction != Vector3.Zero)
            {
                tempVelocity.X = direction.X * speed;
                tempVelocity.Z = direction.Z * speed;
            }
            else
            {
                tempVelocity.X = (float)Mathf.Lerp(tempVelocity.X, direction.X * speed, delta * 7.0f);
                tempVelocity.Z = (float)Mathf.Lerp(tempVelocity.Z, direction.Z * speed, delta * 7.0f);
            }
        }
        else
        {
            tempVelocity.X = (float)Mathf.Lerp(tempVelocity.X, direction.X * speed, delta * 3.0f);
            tempVelocity.Z = (float)Mathf.Lerp(tempVelocity.Z, direction.Z * speed, delta * 3.0f);
        }

        ctx.Velocity = tempVelocity;

        ctx.MoveAndSlide();
    }

    public override void OnEnter()
    {
        speed = ctx.walkSpeed * ctx.SpeedMod;
        ctx.stepSource.soundValue = 4;
        ctx.StepType = 0;
    }

    public override void OnExit()
    {
    }
}