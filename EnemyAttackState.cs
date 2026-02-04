using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private EnemyStateContexter temp_controller;

    public void EnterState(EnemyStateContexter controller)
    {
                 
        temp_controller = controller;
        controller.animator.SetTrigger("Attack");
        controller.OnDamagedEvent += OnDamaged;
    }

    public void UpdateState(EnemyStateContexter controller)
    {
        // 공격 애니메이션 중에는 상태 전환 없음
    }

    public void ExitState(EnemyStateContexter controller)
    {
        controller.OnDamagedEvent -= OnDamaged;
        controller.animator.ResetTrigger("Attack");
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