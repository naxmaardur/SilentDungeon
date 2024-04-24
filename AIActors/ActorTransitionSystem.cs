using AIStates;
using Godot;
using System;

public partial class ActorControler
{
    public void SetupTransitions()
    {
        stateMachine.AddTransition(new Transition(typeof(IdleState), typeof(GotToPlayerState), DetectedPlayer));
        stateMachine.AddTransition(new Transition(typeof(SleepState), typeof(GotToPlayerState), DetectedPlayer));
        //stateMachine.AddTransition(new Transition(typeof(OrbitPlayer), typeof(BackOffFromPlayer), shouldBackOff));
        stateMachine.AddTransition(new Transition(typeof(GotToPlayerState), typeof(OrbitPlayer), NearPlayer));
        stateMachine.AddTransition(new Transition(typeof(OrbitPlayer), typeof(GotToPlayerState), LeftRange));
        //stateMachine.AddTransition(new Transition(typeof(BackOffFromPlayer), typeof(GotToPlayerState), () => !NearPlayer()));
        stateMachine.AddTransition(new Transition(typeof(BackOffFromPlayer), typeof(GotToPlayerState), BackOffTimer));
        stateMachine.AddTransition(new Transition(typeof(OrbitPlayer), typeof(AttackState), canAttackPlayer));
        stateMachine.AddTransition(new Transition(typeof(AttackState), typeof(BackOffFromPlayer), AttackIsFinished));
    }


    private bool playerInFollowRange()
    {
        return true;
    }

    private bool shouldBackOff()
    {
        return NearPlayer() && inRangeTimer.TimeLeft <= 0;
    }
    private bool NearPlayer()
    {
        return player.GlobalPosition.DistanceTo(GlobalPosition) < 2;
    }
    private bool LeftRange()
    {
        return player.GlobalPosition.DistanceTo(GlobalPosition) > 4;
    }

    private bool BackOffTimer()
    {
        return backOffTimer.TimeLeft <= 0;
    }

    private bool canAttackPlayer()
    {
        if (inRangeTimer.TimeLeft <= 0)
        {
            if (player.GlobalPosition.DistanceTo(GlobalPosition) < 1.5)
            {
                return true;
            }
            else
            {
                inRangeTimer.Start();
            }
        }
        return false;
    }

    private bool AttackIsFinished()
    {
        return !(bool)tree.Get("parameters/Alive/OneShotAttack/active");
    }

    private bool DetectedPlayer()
    {
        return alertValue > 6;
    }
}
