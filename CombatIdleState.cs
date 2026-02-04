using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CombatIdleState : IState
{

    private float combatIdleTime = 5.0f;
    public void EnterState(PlayerStateContexter controller)
    {
        controller.animationContexter.playIdle(false);

        controller.onWeapon();

        controller.player.stopMoveto();
    }

    public void UpdateState(PlayerStateContexter controller)
    {


        if (controller.combatManager.isAttackKeyDown())
        {
            combatIdleTime = 5.0f;
            return;
        }

        if (InputManager.Instance.GetChangeTargetDown())
        {

            if (controller.combatManager.changeTargetEnemy() != null)
            {

            }
            else
            {

            }
            combatIdleTime = 5.0f; // 타겟 변경 후 Idle 타이머 초기화
            return; // 상태 유지
        }

        if (controller.player.getInputVector().sqrMagnitude == 0)
        {


            combatIdleTime -= Time.deltaTime;

            if (combatIdleTime <= 0)
            {
                controller.TransitionState(States.Idle);
                return;
            }

        }

        if (controller.player.getInputVector().sqrMagnitude > 0)
        {
            combatIdleTime = 5.0f;
            controller.TransitionState(States.CombatRun);
            return;
        }


    }

    public void ExitState(PlayerStateContexter controller)
    {
        combatIdleTime = 5.0f;

    }

}
