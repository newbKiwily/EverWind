using UnityEngine;

public class EnemyMoveState : IEnemyState
{
    private Transform target;
    private float attackDistance = 2.0f;
    private float speed = 3.0f;
    private EnemyStateContexter temp_controller;

    public void EnterState(EnemyStateContexter controller)
    {
        
        temp_controller = controller;
        controller.animator.SetBool("Move", true);
        controller.OnDamagedEvent += OnDamaged;
    }

    public void EnterState(EnemyStateContexter controller, Transform target)
    {
        this.target = target;
        EnterState(controller);
    }

    public void UpdateState(EnemyStateContexter controller)
    {
        if (target == null)
        {
            controller.TransitionState(EnemyStates.Idle);
            return;
        }

        Vector3 dir = target.position - controller.transform.position;
        float distance = dir.magnitude;

        if (distance <= attackDistance)
        {
            if (controller.DetectAttackable() == false)
            {
                controller.TransitionState(EnemyStates.Idle);
                target = null;

                return;
            }
            controller.TransitionState(EnemyStates.Attack, target);
            return;
        }

        Vector3 moveDir = dir.normalized;
        controller.transform.position += moveDir * speed * Time.deltaTime;

        Vector3 horizontalDir = new Vector3(moveDir.x, 0, moveDir.z);
        if (horizontalDir != Vector3.zero)
        {
            if (controller.GetComponent<Spider>() == null)
            {
                controller.transform.rotation = Quaternion.Slerp(
                    controller.transform.rotation,
                    Quaternion.LookRotation(horizontalDir),
                    Time.deltaTime * 10f
                );
            }
            else
            {
                controller.transform.rotation = Quaternion.Slerp(
                   controller.transform.rotation,
                   Quaternion.LookRotation(-horizontalDir),
                   Time.deltaTime * 10f
               );
            }
        }
    }

    public void ExitState(EnemyStateContexter controller)
    {
        controller.animator.SetBool("Move", false);
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