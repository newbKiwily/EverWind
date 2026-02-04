using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Playables;



public enum States
{
    Idle,
    Move,
    CombatRun,
    Interact,
    CombatIdle,
    Attack,
    Damaged,
    Dead
}

public class PlayerStateContexter : MonoBehaviour
{
    private IState currentState;
    private IState prevState;

    public Player player;
    public AnimationContexter animationContexter;

    private Vector3 armPosition = new Vector3(0.12f, 0.24f, 0.02f);
    private Quaternion armRotation = Quaternion.Euler(37.075f, 148.54f, 265.49f);
    private Vector3 backPosition = new Vector3(0.9459f, 0.8265f, -0.121f);
    private Quaternion backRotation = Quaternion.Euler(187.69f, -6.533f, 38.847f);

    public GameObject Weapon;
    public GameObject armObj;
    public GameObject backObj;

    private Dictionary<States, IState> stateTable = new Dictionary<States, IState>();
    public CombatManager combatManager;
    public PopUpUIManager popUpUIManager;
    private PlayerStatManager playerStatManager;
    public void TransitionState(States targetState, Transform target)
    {
        if (currentState != null)
            currentState.ExitState(this);

        prevState = currentState;

        if (targetState == States.Move)
        {
            var temp_state = new MoveState();
            currentState = temp_state;
            temp_state.EnterState(this, target);
        }
        else if (targetState == States.CombatRun)
        {
            var temp_state = new CombatRunState();
            currentState = temp_state;
            temp_state.EnterState(this, target);
        }
        else
        {
            TransitionState(targetState);
        }
    }
    public void TransitionState(States targetState)
    {
        if (currentState != null)
            currentState.ExitState(this);

        prevState = currentState;
        currentState = stateTable[targetState];

        currentState.EnterState(this);
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        animationContexter = GetComponent<AnimationContexter>();
        combatManager = GetComponent<CombatManager>();
        player.OnDied += HandleDied;
    }   
    private void Start()
    {
        stateTable.Add(States.Idle, new IdleState());
        stateTable.Add(States.Move, new MoveState());
        stateTable.Add(States.CombatRun, new CombatRunState());
        stateTable.Add(States.Interact, new InteractState());
        stateTable.Add(States.CombatIdle, new CombatIdleState());
        stateTable.Add(States.Attack, new AttackState());
        stateTable.Add(States.Damaged, new DamagedState());
        stateTable.Add(States.Dead, new DeadState());
        currentState = stateTable[States.Idle];
        // 초기 상태 설정
        TransitionState(States.Idle);

        offWeapon();

    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public IState getPrevState()
    {
        return prevState;
    }

    public IState getCurrState()
    {
        return currentState;
    }

    public void onWeapon()
    {
        Weapon.transform.parent = armObj.transform;
        Weapon.transform.localPosition = armPosition;
        Weapon.transform.localRotation = armRotation;
    }

    public void offWeapon()
    {
        Weapon.transform.parent = backObj.transform;
        Weapon.transform.localPosition = backPosition;
        Weapon.transform.localRotation = backRotation;

    }

    public void HandleDied()
    {
        TransitionState(States.Dead);
        combatManager.targetEnemy = null;
    }
}
