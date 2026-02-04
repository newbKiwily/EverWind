using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private EnemyStateContexter temp_controller;
    public void EnterState(EnemyStateContexter controller)
    {
        temp_controller = controller;
        Debug.Log("enterEnemyState");
        controller.animator.SetBool("Idle", true);
        controller.OnDamagedEvent += OnDamaged;

    }
    public void UpdateState(EnemyStateContexter controller)
    {
       
        
    }

    public void ExitState(EnemyStateContexter controller)
    {
        controller.animator.SetBool("Idle", false);
        controller.OnDamagedEvent -= OnDamaged;
        temp_controller = null;

    }

    private void OnDamaged(Transform attacker)
    {
        if (temp_controller != null)
        {
            //Attack 중 피격 무시
            if (temp_controller.isAttacking) return;

            temp_controller.TransitionState(EnemyStates.Damaged, attacker);
        }

    }
}
