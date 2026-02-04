using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : IState
{
    Skill currSkill;

    public void EnterState(PlayerStateContexter controller)
    {


        currSkill = controller.combatManager.currentCastingSkill;
        var currSkillIdx = controller.combatManager.currentCastingSkillIndex;

        if (currSkill == null || controller.combatManager.skillCooldownRemain[currSkillIdx] >0)
        {

            controller.TransitionState(States.CombatIdle);
            return;
        }
        if (controller.combatManager.targetEnemy != null)
        {
            controller.player.lookAtTarget(controller.combatManager.targetEnemy.transform);
        }
        controller.animationContexter.playAttack(currSkillIdx+1);
    }

    public void UpdateState(PlayerStateContexter controller)
    {



    }

    public void ExitState(PlayerStateContexter controller)
    {
        controller.animationContexter.exitInteract();
        currSkill = null;
    
    }

}

