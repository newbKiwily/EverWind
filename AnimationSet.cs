using UnityEngine;

public enum OneshotAni
{
    Success
  
}

[CreateAssetMenu]
public class AnimationSet : ScriptableObject
{



    public AnimationClip keyIdle;
    public AnimationClip keyMove;
    public AnimationClip keyOneshot;
    public AnimationClip keyDamaged;
    public AnimationClip keyAttack;

    public AnimationClip idleNormal;
    public AnimationClip idleCombat;


    public AnimationClip moveNormal;
    public AnimationClip moveCombat;

    public AnimationClip OS_success;
    public AnimationClip Damaged1, Damaged2, Damaged3;

    public AnimationClip Attack1, Attack2, Attack3, Attack4, Attack5;

    public AnimationClip Dead;


}
