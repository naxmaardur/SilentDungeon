using Godot;
using System;

public partial class PlayerController
{
    public float health { get; private set; } = 100;

    public Action<float> healthUpdate;
    public Action<bool> healthDisplay;
    public Action PlayerDeath;
    public void TakeDamage(float damage)
    {
        hurtSource.SetRandomPitch(0.9f,1.1f);
        hurtSource.PlaySound();
        startCameraShake(0.02f,5);

        damage *= protection;
        health -= damage;
        healthUpdate?.Invoke(health);
        if(health <= 0.999f)
        {
            death();
        }
    }

    private void death()
    {
        health = 100; healthUpdate?.Invoke(health);
        PlayerDeath?.Invoke();
        ToggleActiveActor(false);
        inputsActive = false;
    }
}