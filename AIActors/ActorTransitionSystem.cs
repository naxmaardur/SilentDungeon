using AIStates;
using Godot;
using System;
using System.ComponentModel;

public partial class ActorControler
{
    public void SetupTransitions()
    {
        stateMachine.AddTransition(new Transition(typeof(IdleState), typeof(GotToPlayerState), DetectedPlayer));
        stateMachine.AddTransition(new Transition(typeof(SleepState), typeof(GotToPlayerState), DetectedPlayer));
        stateMachine.AddTransition(new Transition(typeof(IdleState), typeof(InvestigateState), SearchPlayer));
        stateMachine.AddTransition(new Transition(typeof(SleepState), typeof(InvestigateState), SearchPlayer));
        stateMachine.AddTransition(new Transition(typeof(GotToPlayerState), typeof(InvestigateState), LostPlayer));
        stateMachine.AddTransition(new Transition(typeof(InvestigateState), typeof(GotToPlayerState), DetectedPlayer));
        stateMachine.AddTransition(new Transition(typeof(InvestigateState), typeof(GotToPlayerState), DetectedPlayer));
        stateMachine.AddTransition(new Transition(typeof(InvestigateState), typeof(IdleState), InvestigateFinished));
        stateMachine.AddTransition(new Transition(typeof(GotToPlayerState), typeof(OrbitPlayer), () => { return !isWarden && NearPlayer(); }));
        stateMachine.AddTransition(new Transition(typeof(OrbitPlayer), typeof(GotToPlayerState), LeftRange));
        stateMachine.AddTransition(new Transition(typeof(BackOffFromPlayer), typeof(GotToPlayerState), BackOffTimer));
        stateMachine.AddTransition(new Transition(typeof(OrbitPlayer), typeof(AttackState), canAttackPlayer));
        stateMachine.AddTransition(new Transition(typeof(AttackState), typeof(BackOffFromPlayer), () => { return !isWarden && AttackIsFinished(); }));
        stateMachine.AddTransition(new Transition(typeof(AttackState), typeof(GotToPlayerState), () => { return isWarden && AttackIsFinished(); }));

        stateMachine.AddTransition(new Transition(typeof(WanderState), typeof(GotToPlayerState), DetectedPlayer));
        stateMachine.AddTransition(new Transition(typeof(WanderState), typeof(InvestigateState), SearchPlayer));
        stateMachine.AddTransition(new Transition(typeof(IdleState), typeof(WanderState), IsWarden));
        stateMachine.AddTransition(new Transition(typeof(GotToPlayerState), typeof(AttackState), canAttackPlayer));
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
        if (damagedTimer.TimeLeft > 0) { PlayerAgrod?.Invoke(); return true; }
        if(AlertValue > playerDetectedValue) { GD.Print("Detected Player from sound"); PlayerAgrod?.Invoke(); return true; }
        bool check = AlertValue > playerDetectedNearValue && GlobalPosition.DistanceTo(player.GlobalPosition) < 1 || seeingPlayer();
        if (check)
        {
            PlayerAgrod?.Invoke();
        }
        return check;
    }

    private bool SearchPlayer()
    {
        if (AlertValue < 2) { return false; }
        if (hasSight && !seeingPlayer()) { return true; }
        return true;
    }

    private bool LostPlayer()
    {
        if(damagedTimer.TimeLeft > 0) { return false; }
        if(AlertValue < PlayerLostValue)
        {
            alertValue -= PlayerLostValue / 1.5f;
            positionOfIntrest = player.GlobalPosition;
            return true;
        }
        return false;
    }

    private bool seeingPlayer()
    {
        if (!hasSight) return false;
        if (stateMachine.CurrentState.GetType() == typeof(SleepState)) { return false; }
        if (this.Forward().Dot(player.GlobalPosition - GlobalPosition) < 0.4) { return false; }

        if(player.GlobalPosition.DistanceTo(GlobalPosition) > 6) {  return false; }
        if (player.sneaking && player.GlobalPosition.DistanceTo(GlobalPosition) > 2) { return false; }
        if (this.RayCast3D(GlobalPosition, player.GlobalPosition, out var hit, 0xffffffff, false))
        {
            if (hit.collider == player) { return true; }
        }
        return false;
    }


    private bool IsWarden()
    {
        return isWarden;
    }

    private bool InvestigateFinished()
    {
        return AlertValue < 0.0001f;
    }
}
