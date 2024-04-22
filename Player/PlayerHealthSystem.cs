using Godot;
using System;

public partial class PlayerController
{
    public float health { get; private set; } = 100;

    public Action<float> healthUpdate;

    public void TakeDamage(float damage)
    {
        startCameraShake(0.02f,5);
        health -= damage;
        healthUpdate?.Invoke(health);
        if(health <= 0)
        {
            death();
        }
    }

    private void death()
    {
        throw new NotImplementedException();
    }
}