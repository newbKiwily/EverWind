using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NormalSkill : Skill
{   

    public override void Cast(Transform attacker)
    {
        coolTime = 0.3f;

        islocked = false;
        dmg = 10.0f;
        GameObject target = GetTarget(attacker);

        DealDamage(attacker, target, dmg);
        EffectManager.Instance.PlayEffect("Skill_normal", attacker.position);
    }
   
    protected override void Start()
    {
        SkillAnimationName = "NormalSkill";
        coolTime = 0.3f;
      
        islocked = false;
        dmg = 10.0f;
    }


    protected override void Update()
    {
        
    }
}


