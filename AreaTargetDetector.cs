using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTargetDetector : MonoBehaviour
{
    public HashSet<GameObject> detectedEnemies = new HashSet<GameObject>();
    public AreaSkill owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {           
            detectedEnemies.Add(other.gameObject);
        }
    }


    public void Finish()
    {
        owner.ReceiveTargets(detectedEnemies);
        Destroy(gameObject);
    }
}
