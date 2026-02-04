using UnityEngine;
public class DamagedState : IState
{
    static public bool isFinished;

    public void EnterState(PlayerStateContexter controller)
    {
        DisplayUIManager.Instance.ChangeProfile(DisplayUIManager.ProfileState.Hit, 1.0f);
        controller.player.stopMoveto();
        controller.animationContexter.playDamaged(controller.combatManager.damagedCount);
        if(controller.combatManager.damagedCount>=3)
        {
            controller.combatManager.damagedCount = 0;
        }

        isFinished = false;
    }

    public void UpdateState(PlayerStateContexter controller)
    {
        if (!isFinished)
            return;

        if (controller.getPrevState() is CombatIdleState ||
            controller.getPrevState() is CombatRunState ||
            controller.getPrevState() is AttackState || 
            controller.getPrevState() is DamagedState)
        {
            controller.TransitionState(States.CombatIdle);
        }
        else
        {
            controller.TransitionState(States.Idle);
        }
    }
    public void ExitState(PlayerStateContexter controller)
    {
        controller.animationContexter.exitInteract();
    }
}
