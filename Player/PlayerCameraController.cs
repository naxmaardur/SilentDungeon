using Godot;
using System;

public partial class PlayerController
{
    [ExportGroup("Base Settings")]
    [Export] private float sensitivity = 0.004f;
    [Export] private float controlerSensitivity = 0.02f;
    [Export] private float minXRotation = -89;
    [Export] private float maxXRotation = 89;
    [Export] public AnimationPlayer CameraHeightAnimation { get; private set; }
    [Export] private Node3D shakePivot;

    [ExportGroup("FOV")]
    [Export] private float baseFOV = 75.0f;
    [Export] private float FOVChange = 1.5f;
    [Export] private float maxFOVMultiplier = 16.0f;
    [Export] private float minFOVMultiplier = 0.5f;

    [ExportGroup("View Bobbing")]
    [Export] private float bobFrequency = 2;
    [Export] private float bobAmplitude = 0.06f;

    private Camera3D camera;
    private float bobTime = 0;

    private float shakeFade = 5;
    private float shakeStrength;

    private void CameraSetup()
    {
        camera = pivot.GetChildByType<Camera3D>();
    }
    
    private void CameraInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseDelta)
        {
            RotateY(-mouseDelta.Relative.X * sensitivity);
            pivot.RotateX(-mouseDelta.Relative.Y * sensitivity);
            pivot.Rotation = pivot.Rotation with { X = Mathf.Clamp(pivot.Rotation.X, Mathf.DegToRad(minXRotation), Mathf.DegToRad(maxXRotation)) };
        }
    }

    private void CameraInput()
    {
        Vector2 inputDir = Input.GetVector("look_left", "look_right", "look_up", "look_down");
        RotateY(-inputDir.X * controlerSensitivity);
        pivot.RotateX(-inputDir.Y * controlerSensitivity);
        pivot.Rotation = pivot.Rotation with { X = Mathf.Clamp(pivot.Rotation.X, Mathf.DegToRad(minXRotation), Mathf.DegToRad(maxXRotation)) };
    }

    private void CameraPhysicsProcess(double delta)
    {
        // View Bobbing
        bobTime += (float)delta * Velocity.Length() * Convert.ToSingle(IsOnFloor());
        camera.Transform = camera.Transform with { Origin = Headbob(bobTime) };

        // FOV
        float velocityClamped = Mathf.Clamp(Velocity.Length(), minFOVMultiplier, maxFOVMultiplier);
        float targetFov = baseFOV + FOVChange * velocityClamped;
        camera.Fov = (float)Mathf.Lerp(camera.Fov, targetFov, delta * 8.0f);

        if(shakeStrength > 0)
        {
            shakeStrength = (float)Mathf.Lerp(shakeStrength, 0, shakeFade * delta);

            shakePivot.Position = CameraShake();
            if(shakeStrength <= 0)
            {
                shakePivot.Position = Vector3.Zero;
            }
        }
    }
    

    private Vector3 Headbob(float time)
    {
        Vector3 pos = Vector3.Zero;
        pos.Y = Mathf.Sin(time * bobFrequency) * bobAmplitude;
        pos.X = Mathf.Cos(time * bobFrequency / 2) * bobAmplitude;

        float lowPos = bobAmplitude - 0.05f;

        if (pos.Y > -lowPos)
        {
            canPlay = true;
        }

        if(pos.Y < -lowPos && canPlay)
        {
            canPlay = false;
            stepIndex++;
            if(stepIndex > 3)
            {
                stepIndex = 0;
            }
            switch (StepType)
            {
                default:
                    stepSource.SetAudio(stepSounds[stepIndex]);
                    break;
                case 1:
                    stepSource.SetAudio(SneakSounds[stepIndex]);
                    break;
            }
            stepSource.SetRandomPitch(0.8f, 1.2f);
            stepSource.PlaySound();
        }


        return pos;
    }
    
    private Vector3 CameraShake()
    {
        return new Vector3(numberGenerator.RandfRange(-shakeStrength, shakeStrength), numberGenerator.RandfRange(-shakeStrength, shakeStrength), numberGenerator.RandfRange(-shakeStrength, shakeStrength));
    }

    private void startCameraShake(float strenght, float fade)
    {
        shakeStrength = strenght;
        shakeFade = fade;
    }

    
}
