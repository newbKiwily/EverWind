using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    public void EnterState(PlayerStateContexter controller)
    {

        controller.animationContexter.playIdle(true);
        controller.player.stopMoveto();
        controller.offWeapon();
        
    }

    public void UpdateState(PlayerStateContexter controller)
    {
        if (InputManager.Instance == null)
            return;
        if (controller.player.getInputVector().sqrMagnitude > 0)
        {
            controller.TransitionState(States.Move);
            return;
        }
        if (InputManager.Instance.GetEnterCombatDown())
        {
            controller.TransitionState(States.CombatIdle);
            return;
        }

        if (InputManager.Instance.GetInteractDown())
        {
            var target = controller.player.detectObtainable();
            if (target != null)
            {
                controller.TransitionState(States.Move, target.transform);
                return;
            }

        }


    }

    public void ExitState(PlayerStateContexter controller)
    {
    }
}
