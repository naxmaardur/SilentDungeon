using Godot;
using System;

public partial class PlayerController
{
    public float health { get; private set; } = 100;

    public Action<float> healthUpdate;
    public Action<bool> healthDisplay;

    public void TakeDamage(float damage)
    {
        startCameraShake(0.02f,5);

        damage *= protection;
        health -= damage;
        healthUpdate?.Invoke(health);
        if(health <= 0)
        {
            death();
        }
    }

    private void death()
    {
        GD.PrintErr("Player death not implemented");
    }
}