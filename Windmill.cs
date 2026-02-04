using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : AreaSkill
{   
    public override void Cast(Transform attacker)
    {
        coolTime = 2.0f;
        islocked = false;
        detectTime = 0.15f;
        dmg = 10.0f;
        GameObject detectorObj = Instantiate(colliderPrefab, attacker.position, attacker.rotation);
        AreaTargetDetector detector = detectorObj.GetComponent<AreaTargetDetector>();

        detector.owner = this;  
        CombatManager cm = attacker.GetComponent<CombatManager>();
        cm.StartCoroutine(DetectionRoutine(detector,attacker));
        EffectManager.Instance.PlayEffect("Skill_windmill", attacker.position);

    }

    protected override void Start()
    {
        SkillAnimationName = "Windmill";
        coolTime = 2.0f;

    }
    private IEnumerator DetectionRoutine(AreaTargetDetector detector,Transform attacker)
    {
        // 즉발 감지 시간
        yield return new WaitForSeconds(detectTime);

        
        detector.Finish();

        // 3) 대미지 처리
        if (collectedTargets != null)
        {
            foreach (var enemy in collectedTargets)
            {
                DealDamage(attacker, enemy, dmg);
            }
        }

    }
}
