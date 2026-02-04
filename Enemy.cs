using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable
{
  
    [SerializeField] protected float hp = 100f;

    protected Animator animator;
    protected EnemyStateContexter controller;
    protected AnimatorOverrideController overrideController;

    public AnimationClip damaged1;
    public AnimationClip damaged2;
    public AnimationClip damaged3;

    protected bool isDead = false;
    public event System.Action<Enemy> OnEnemyDied;
    public EnemyHpUIManager hpUIManager;
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<EnemyStateContexter>();
        hpUIManager = GameObject.Find("EnemyHpUIManager").GetComponent<EnemyHpUIManager>();
        if (animator != null && animator.runtimeAnimatorController != null)
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);


        if (overrideController != null)
            animator.runtimeAnimatorController = overrideController;
    }

    public void SwitchDamagedMotion(int damagedCount)
    {
        if (overrideController == null) return;

        switch (damagedCount)
        {
            case 1:
                if (damaged1 != null)
                    overrideController["damaged1"] = damaged1;
                break;
            case 2:
                if (damaged2 != null)
                    overrideController["damaged1"] = damaged2;
                break;
            case 3:
                if (damaged3 != null)   
                    overrideController["damaged1"] = damaged3;
                break;
        }
    }

    public virtual void takeDamage(float damage, Transform attacker)
    {
        if (isDead) return;

        EffectManager.Instance.PlayEffect("Damaged", this.transform.position);
        hp -= damage;
        hpUIManager.ShowDamageText(transform.position, (int)damage);
        if (hp < 0) hp = 0;

        Debug.Log($"[{gameObject.name}] ³²Àº HP : {hp}");

        if (controller != null)
            controller.OnDamaged(attacker);

        if (hp <= 0)
        {
            var temp_attacker=attacker.GetComponent<CombatManager>();
            
            temp_attacker.targetEnemy = null;
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        Destroy(this.gameObject);
        OnEnemyDied?.Invoke(this);
        Debug.Log($"[{gameObject.name}] »ç¸Á");
    }

    public float getHp()
    {
        return hp;
    }
    public void setHp(float t_hp)
    {
        hp += t_hp;
    }


}
