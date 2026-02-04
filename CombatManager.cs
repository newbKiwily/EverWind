using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class CombatManager : MonoBehaviour
{
    
    public PlayerStateContexter playerStateContexter;
    public Skill currentCastingSkill;
    public int currentCastingSkillIndex;
    public Skill[] skills = new Skill[5];

    private KeyCode prevKeyCode;
    private KeyCode currKeyCode;

    private GameObject prevTargetEnemy;
    public GameObject targetEnemy;
    public float target_radius;
    public int damagedCount;

    private List<GameObject> inRadiusEnemy;


    public EnemyHpUIManager enemyHpUIManager;

    public float[] skillCooldownRemain = new float[5];
    public float[] skillCooldownMax = new float[5];
    public GameObject targetingUI;

    void Start()
    {   
        
        playerStateContexter = GetComponent<PlayerStateContexter>();
             
        currKeyCode = KeyCode.None;
        targetEnemy = null;
        prevTargetEnemy = null;
        target_radius = 50.0f;
        damagedCount = 0;
        inRadiusEnemy = new List<GameObject>();
        enemyHpUIManager=GameObject.Find("EnemyHpUIManager").GetComponent<EnemyHpUIManager>();
        playerStateContexter.player.EvArrive += OnTargetArrived;
        InitSkillCooldownMax();
        targetingUI.SetActive(false);
    }


    void Update()
    {
        for (int i = 0; i < skillCooldownRemain.Length; i++)
        {
            if (skillCooldownRemain[i] > 0)
                skillCooldownRemain[i] -= Time.deltaTime;
        }

        UpdateTargetingUI();
    }

    private void UpdateTargetingUI()
    {
        
        if (targetEnemy == null)
        {
            if (targetingUI.activeSelf) targetingUI.SetActive(false);
            return;
        }
        if (!targetingUI.activeSelf) targetingUI.SetActive(true);

        Vector3 offset = new Vector3(0, 4.2f, 0);
        targetingUI.transform.position = targetEnemy.transform.position + offset;

        targetingUI.transform.LookAt(targetingUI.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                     Camera.main.transform.rotation * Vector3.up);
    }
    public void knockBack()
    {

        var curr = playerStateContexter.getCurrState();

        // 공격 중이면 슈퍼아머
        if (curr is AttackState)
            return;

        damagedCount++;

        playerStateContexter.TransitionState(States.Damaged);
    }
    public bool isAttackKeyDown()
    {
        var curr = playerStateContexter.getCurrState();

        if (curr is DamagedState)
            return true;

        if (curr is AttackState)
            return true;

        int keyPressed = InputManager.Instance.GetAttackKeyDown();

        if (keyPressed == 0)
            return false; // 1~5 키 중 입력 없음

        KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + keyPressed);

        

        // 공격 중이 아닌 경우 스킬 시전 로직
        var curr_skill = skills[keyPressed - 1]; // 0-based 배열
        currentCastingSkillIndex = keyPressed - 1;
        currentCastingSkill = curr_skill;
        prevKeyCode = currKeyCode;
        currKeyCode = key;

        GameObject target = targetEnemy ?? targetingEnemy();

        if (target == null)
        {
            Debug.Log("스킬 시도: 타겟 없음. 스킬 등록 취소.");
            currentCastingSkill = null;
            return false;
        }

        Debug.Log($"스킬 시전 시작: 타겟({target.name})에게 이동.");
        playerStateContexter.TransitionState(States.CombatRun, target.transform);

        return true;
    }

    public void Attack()
    {
        skillCooldownMax[currentCastingSkillIndex] = currentCastingSkill.coolTime;
        skillCooldownRemain[currentCastingSkillIndex] = currentCastingSkill.coolTime;

        if (currentCastingSkill != null)  
            currentCastingSkill.Cast(this.transform);
    }

    public void AttackEnd()
    {

        prevKeyCode = KeyCode.None;
        currKeyCode = KeyCode.None;

        playerStateContexter.TransitionState(States.CombatIdle);
        currentCastingSkill = null;

    }
    public void EndDamaged()
    {
        
        DamagedState.isFinished = true;
    }

    public GameObject targetingEnemy()
    {
        
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, target_radius);

        if (colliders.Count() <= 0)
            return null;

        float min_distance = float.MaxValue;
        GameObject closetObj = null;
      
        Vector3 currentPosition = this.transform.position;
        inRadiusEnemy.Clear();

        foreach (var it in colliders)
        {
            var temp = it.gameObject.GetComponent<IDamageable>();
            if (temp == null||it.CompareTag("Player"))
                continue;
         
            inRadiusEnemy.Add(it.gameObject);

            float distance = (it.transform.position - currentPosition).sqrMagnitude;

            if (distance < min_distance)
            {
                closetObj = it.gameObject;
                min_distance = distance;
            }

        }

        enemyHpUIManager.updateEnemyList(inRadiusEnemy);

        if (inRadiusEnemy.Count > 0)
        {
            inRadiusEnemy.Sort((a, b) =>
            {
                float dist_sqr_A = (a.transform.position - currentPosition).sqrMagnitude;
                float dist_sqr_B = (b.transform.position - currentPosition).sqrMagnitude;
                return dist_sqr_A.CompareTo(dist_sqr_B);
            });

            closetObj = inRadiusEnemy[0];
        }
        else
            return null;
              
        prevTargetEnemy = closetObj;
        targetEnemy = closetObj;
        playerStateContexter.player.lookAtTarget(targetEnemy.transform);
        return closetObj;

    }

    public GameObject changeTargetEnemy()
    {
        if (targetEnemy == null)
            return null;

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, target_radius);

        if (colliders.Count() <= 0)
            return null;

        
 
        int currentIndex = inRadiusEnemy.IndexOf(targetEnemy);

     
        int nextIndex = currentIndex + 1;

        if (nextIndex >= inRadiusEnemy.Count)
        {
            nextIndex = 0;
        }

        GameObject nextTarget = inRadiusEnemy[nextIndex];

        prevTargetEnemy = targetEnemy;
        targetEnemy = nextTarget;
        playerStateContexter.player.lookAtTarget(targetEnemy.transform);
        return nextTarget;
    }

    public void OnTargetArrived()
    {
        
        if (currentCastingSkill != null)
        {
            playerStateContexter.TransitionState(States.Attack);
            Debug.Log($"타겟 도착: AttackState로 전이. 스킬 시전 시작: {currentCastingSkill.SkillAnimationName}");
        }
        else
        {

            playerStateContexter.TransitionState(States.CombatIdle);
            Debug.Log("타겟 도착: 등록된 스킬 없음. CombatIdle로 복귀.");
        }

    }

    private void InitSkillCooldownMax()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null)
                skillCooldownMax[i] = skills[i].coolTime;   // 기준 쿨타임만 세팅
            else
                skillCooldownMax[i] = 0f;

            // 남은 쿨타임은 시작 시 0으로 (사용 순간에만 올라가게)
            skillCooldownRemain[i] = 0f;
        }
    }
}