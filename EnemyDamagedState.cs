using UnityEngine;

public class EnemyDamagedState : IEnemyState
{
    private EnemyStateContexter temp_controller;
    

    public void EnterState(EnemyStateContexter controller)
    {
        temp_controller = controller;
        Enemy enemy = controller.GetComponent<Enemy>();

        if (enemy != null)
            enemy.SwitchDamagedMotion(controller.damagedCount);

        controller.animator.Play("Damaged", 0, 0.0f);

        controller.damagedCount++;
        if (controller.damagedCount > 3)
            controller.damagedCount = 1;

        controller.OnDamagedEvent += OnDamaged;
        
    }

    public void UpdateState(EnemyStateContexter controller)
    {
        AnimatorStateInfo stateInfo = controller.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1.0f)
        {
            if (controller.target == null)
            {
                controller.TransitionState(EnemyStates.Idle);
                return;
            }

            float distance = Vector3.Distance(controller.transform.position, controller.target.position);
            float attackDistance = 2.0f;

            if (distance <= attackDistance)
                controller.TransitionState(EnemyStates.Attack, controller.target);
            else
                controller.TransitionState(EnemyStates.Move, controller.target);
        }
    }

    public void ExitState(EnemyStateContexter controller)
    {
        controller.OnDamagedEvent -= OnDamaged;
        temp_controller = null;
    }

    private void OnDamaged(Transform attacker)
    {
        if (temp_controller != null)
        {
            
            temp_controller.TransitionState(EnemyStates.Damaged, attacker);
        }
    }
}