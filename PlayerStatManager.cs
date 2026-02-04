using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    [Header("Stat")]
    public float speed;
    public int attack_power;
    public int defence_power;
    public float hp;
    public float max_hp;
    

    private void Awake()
    {
        attack_power = 10;
        defence_power = 10;
        hp = 30.0f;
        max_hp = 100.0f;
        speed = 4.0f;
    }
    public float calculate_damaged(float dmg)
    {
        float result = 0;
        result = dmg * (100.0f / (100.0f + defence_power));
        return result;
    }

    public float calculate_final_damage(float ratio)
    {
        float result = 0;
        result = attack_power + (attack_power * (ratio/100.0f));
        return result;
    }

    public float get_hp()
    {
        return hp;
    }
    public void set_hp(float hp)
    {
        this.hp = hp;
    }
    public float get_max_hp()
    {
        return max_hp;
    }
    public void set_max_hp(float max_hp)
    {
        this.max_hp = max_hp;
    }
    public void set_attack_power(int power)
    {
        this.attack_power = power;
    }

    public int get_attack_power()
    {
        return attack_power; 
    }
    public int get_defence_power()
    {
        return defence_power; 
    }
    public void set_defence_power(int power)
    {
        this.defence_power = power;
    }
}
