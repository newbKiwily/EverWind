using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;



public enum EnemyStates
{
    Idle,
    Move,
    Attack,
    Damaged
}

public class EnemyStateContexter : MonoBehaviour
{
    private IEnemyState currentState;
    private IEnemyState prevState;

    public Transform target; // 플레이어
    public Animator animator;

    public int damagedCount;

    public  event Action<Transform> OnDamagedEvent;
    

    private Dictionary<EnemyStates, IEnemyState> stateTable = new Dictionary<EnemyStates, IEnemyState>();

    public bool isAttacking;
    public void TransitionState(EnemyStates targetState, Transform target)
    {

        

        if (currentState != null)
            currentState.ExitState(this);

        prevState = currentState;
        this.target = target;
        currentState = stateTable[targetState];

        if (currentState is EnemyMoveState moveState)
            moveState.EnterState(this, target);
        else
            currentState.EnterState(this);
    }

    public void TransitionState(EnemyStates targetState)
    {   


        if (currentState != null)
            currentState.ExitState(this);

        prevState = currentState;
        currentState = stateTable[targetState];

        currentState.EnterState(this);
    }

    private void Start()
    {
        isAttacking = false;
        animator = GetComponent<Animator>();
        damagedCount = 1;

        stateTable.Add(EnemyStates.Idle, new EnemyIdleState());
        stateTable.Add(EnemyStates.Move, new EnemyMoveState());
        stateTable.Add(EnemyStates.Attack, new EnemyAttackState());
        stateTable.Add(EnemyStates.Damaged, new EnemyDamagedState());
        currentState = stateTable[EnemyStates.Idle];
        // 초기 상태 설정
        TransitionState(EnemyStates.Idle);


    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public IEnemyState getPrevState()
    {
        return prevState;
    }

    public IEnemyState getCurrState()
    {
        return currentState;
    }

    public void OnDamaged(Transform attacker)
    {
        // target이 없으면 새로 지정
        if (target == null)
            target = attacker;

        // Damaged 상태로 전환
        OnDamagedEvent?.Invoke(attacker);
    }
    public void EndAttack()
    {
        isAttacking = false;
        if (target != null)
        {

            TransitionState(EnemyStates.Move, target);
        }
        else
        {
            TransitionState(EnemyStates.Idle);
        }
        damagedCount = 1;
    }

    public void Attack()
    {   

        var temp_target = target.GetComponent<IDamageable>();
        if (temp_target!=null)
        {
            isAttacking = true;
            temp_target.takeDamage(10.0f, this.transform);
        }
        
    }

    public bool DetectAttackable()
    {
        var target_state = target.GetComponent<PlayerStateContexter>();
        if(target_state != null)
        {
            if (target_state.getCurrState() is DeadState)
            {
                target = null;
                return false;
            }
            else
                return true;
        }
        target = null;
        return false;
    }
}
