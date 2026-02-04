using UnityEngine;

public class AnimationContexter : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController overrideController;

    [SerializeField] private AnimationSet animationSet;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideController;
        }
    }

    
    public void playIdle(bool isNormal)
    {
        AnimationClip key = animationSet.keyIdle;


        if (isNormal)
        {
            overrideController[key] = animationSet.idleNormal;
        }
        else
        {
            overrideController[key] = animationSet.idleCombat;
        }

        animator.SetBool("isMove", false);
    }

    public void playMove(bool isNormal)
    {
        AnimationClip key = animationSet.keyMove;

        if (isNormal)
        {
            overrideController[key] = animationSet.moveNormal;
        }
        else
        {
            overrideController[key] = animationSet.moveCombat;
        }


        animator.SetBool("isMove", true);
    }
    public void playDamaged(int count)
    {
        AnimationClip key = animationSet.keyDamaged;
        switch(count)
        {
            case 1:
                {
                    overrideController[key] = animationSet.Damaged1;
                    break;
                }
            case 2:
                {
                    overrideController[key] = animationSet.Damaged2;
                    break;
                }
            case 3:
                {
                    overrideController[key] = animationSet.Damaged3;

                    break;
                }
            default:
                return;
        }

        
        animator.SetTrigger("toDamaged");

    }

    public void playAttack(int idx)
    {
        AnimationClip key = animationSet.keyAttack;
        switch(idx)
        {
            case 1:
                {
                    animator.SetFloat("SkillSpeed", 2.0f);
                    overrideController[key] = animationSet.Attack1;
                    break;
                }
            case 2:
                {
                    animator.SetFloat("SkillSpeed", 1.4f);
                    overrideController[key] = animationSet.Attack2;
                    break;
                }
            case 3:
                {
                    animator.SetFloat("SkillSpeed", 1.5f);
                    overrideController[key] = animationSet.Attack3;
                    break;
                }
            case 4:
                {
                    animator.SetFloat("SkillSpeed", 2.0f);
                    overrideController[key] = animationSet.Attack4;
                    break;
                }
            case 5:
                {
                    animator.SetFloat("SkillSpeed", 2.0f);
                    overrideController[key] = animationSet.Attack5;
                    break;
                }
        }
        animator.SetTrigger("toAttack");

    }
    public void playInteract()
    {
        animator.SetBool("isInteract", true);
        animator.Play("Interact", 0, 0.0f);
    }
    public void exitInteract()
    {
        animator.SetBool("isInteract", false);
    }
    public void playDead()
    {
        animator.SetBool("isDie", true);
        
    }
    public void exitDead()
    {
        animator.SetBool("isDie", false);
    }

    public void playOneshot(OneshotAni ani)
    {
        AnimationClip key = animationSet.keyOneshot;
        switch (ani)
        {
            case OneshotAni.Success:
                {
                    overrideController[key] = animationSet.OS_success;
                    break;
                }
            default:
                return;
        }
        animator.SetTrigger("toOneshot");

    }
}
