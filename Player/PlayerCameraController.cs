using Godot;
using System;

public partial class PlayerController
{
    [ExportGroup("Base Settings")]
    [Export] private float sensitivity = 0.004f;
    [Export] private float controlerSensitivity = 0.02f;
    [Export] private float minXRotation = -89;
    [Export] private float maxXRotation = 89;

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


    private void CameraSetup()
    {
        camera = pivot.GetChildByType<Camera3D>();
    }
    
    private void CameraInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseDelta)
        {
            RotateY(-mouseDelta.Relative.X * sensitivity);
            camera.RotateX(-mouseDelta.Relative.Y * sensitivity);
            camera.Rotation = camera.Rotation with { X = Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(minXRotation), Mathf.DegToRad(maxXRotation)) };
        }
    }

    private void CameraInput()
    {
        Vector2 inputDir = Input.GetVector("look_left", "look_right", "look_up", "look_down");
        RotateY(-inputDir.X * controlerSensitivity);
        camera.RotateX(-inputDir.Y * controlerSensitivity);
        camera.Rotation = camera.Rotation with { X = Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(minXRotation), Mathf.DegToRad(maxXRotation)) };
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
    }
    

    private Vector3 Headbob(float time)
    {
        Vector3 pos = Vector3.Zero;
        pos.Y = Mathf.Sin(time * bobFrequency) * bobAmplitude;
        pos.X = Mathf.Cos(time * bobFrequency / 2) * bobAmplitude;
        return pos;
    }
}