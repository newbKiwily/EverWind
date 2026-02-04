using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CombatRunState : IState
{

    private Action onArriveAction;
    public void EnterState(PlayerStateContexter controller)
    {
        controller.animationContexter.playMove(false);
        controller.combatManager.damagedCount = 0;
        controller.player.stopMoveto();
    }
    public void EnterState(PlayerStateContexter controller, Transform target)
    {
    
        if (target != null)
        {
         
            controller.animationContexter.playMove(false);
            controller.player.startMoveto(target);
            onArriveAction = () => OnArrive(controller);
            controller.player.EvArrive += onArriveAction;
        }
        else
        {
            
            EnterState(controller);
        }
    }
    public void UpdateState(PlayerStateContexter controller)
    {
        if (InputManager.Instance == null)
            return;
        if (InputManager.Instance.GetChangeTargetDown())
        {

            GameObject newTarget = controller.combatManager.changeTargetEnemy();
            if (newTarget != null)
            {
               
                
                // **새로운 타겟에게 이동을 시작하기 위해 CombatRun 상태로 재전이 (새 타겟 전달)**
                controller.TransitionState(States.CombatRun, newTarget.transform);
                return; 
            }
            else
            {
              
                // 타겟이 없다면 이동을 중지하고 CombatIdle로 복귀
                controller.TransitionState(States.CombatIdle);
                return;
            }
        }

        if (controller.player.getInputVector().sqrMagnitude == 0)
        {
            controller.TransitionState(States.CombatIdle);
            
            return;
        }
        
        if (controller.combatManager.isAttackKeyDown())
        {
           
            return;
        }


    }

    public void ExitState(PlayerStateContexter controller)
    {
    }
    private void OnArrive(PlayerStateContexter controller)
    {
        
        controller.TransitionState(States.Attack);
    }
}
