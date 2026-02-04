using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SmashSkill : Skill
{
    public override void Cast(Transform attacker)
    {
        coolTime = 1.5f;

        islocked = false;
        dmg = 15.0f;
        GameObject target = GetTarget(attacker);

        DealDamage(attacker, target, dmg);
        EffectManager.Instance.PlayEffect("Skill_smash", attacker.position);
    }

    protected override void Start()
    {
        coolTime = 1.5f;
        SkillAnimationName = "SmashSkill";
        
    }


    protected override void Update()
    {

    }
}
