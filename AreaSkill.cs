using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaSkill : Skill
{
    public GameObject colliderPrefab;
    protected float detectTime;

    protected HashSet<GameObject> collectedTargets;

    public void ReceiveTargets(HashSet<GameObject> targets)
    {
        collectedTargets = targets;
    }

}
