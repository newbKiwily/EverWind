using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class Skill : MonoBehaviour
{

    public string SkillAnimationName;
    public float coolTime;
    public bool isCasted;
    protected float dmg;
    protected bool islocked = false;

    public abstract void Cast(Transform init_transform);
    protected virtual void DealDamage(Transform attacker, GameObject target, float dmg)
    {
        if (target == null) return;
        IDamageable dmgComp = target.GetComponent<IDamageable>();
        float result=attacker.GetComponent<PlayerStatManager>().calculate_final_damage(dmg);
        if (dmgComp != null)
            dmgComp.takeDamage(result, attacker);
    }

    public virtual GameObject GetTarget(Transform attacker)
    {
        CombatManager combatManager = attacker.GetComponent<CombatManager>();
        if (combatManager != null && combatManager.targetEnemy != null)
        {
            return combatManager.targetEnemy;
        }
        else
            return null;
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }


}
