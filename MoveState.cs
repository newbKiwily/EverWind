using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveState : IState
{
    private Action onArriveAction;
    public void EnterState(PlayerStateContexter controller)
    {


        controller.animationContexter.playMove(true);
        controller.combatManager.damagedCount = 0;
        controller.player.stopMoveto();
    }

    public void EnterState(PlayerStateContexter controller, Transform target)
    {
      
        if (target != null)
        {
       
            controller.animationContexter.playMove(true);
            controller.player.startMoveto(target);
            onArriveAction = () => OnArrive(controller);
            controller.player.EvArrive += onArriveAction;
        }
        else
        {
            // 타겟이 없으면 일반 MoveState 진입 로직을 따릅니다.
            EnterState(controller);
        }
    }

    public void UpdateState(PlayerStateContexter controller)
    {
        if (InputManager.Instance == null)
            return;
        if (controller.player.getInputVector().sqrMagnitude == 0)
        {
            controller.TransitionState(States.Idle);
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
                controller.player.startMoveto(target.transform);
                return;
            }

        }
    }
    public void ExitState(PlayerStateContexter controller)
    {
        if (onArriveAction != null)
        {
            controller.player.EvArrive -= onArriveAction;
            onArriveAction = null;
        }

    }

    private void OnArrive(PlayerStateContexter controller)
    {
       
        controller.TransitionState(States.Interact);
    }
}
