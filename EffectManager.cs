using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    [SerializeField]
    private SerializedDictionary<string,ParticleSystem> particleTable = new SerializedDictionary<string,ParticleSystem>();

    private Dictionary<string,List<ParticleSystem>> particlePool = new Dictionary<string,List<ParticleSystem>>();

    [SerializeField]
    private int instance_amount = 3;
    [SerializeField]
    private int max_amount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        max_amount = instance_amount * 8;
        foreach(var particle in particleTable)
        {
            List<ParticleSystem> particleSystems = new List<ParticleSystem>();

            for(int i = 0; i < instance_amount; i++)
            {
                var temp = Instantiate(particle.Value, this.transform);
                temp.gameObject.SetActive(false);
                particleSystems.Add(temp);
            }

            particlePool.Add(particle.Key, particleSystems);

        }
    }


    public void PlayEffect(string key, Vector3 position)
    {
        if (!particlePool.ContainsKey(key)) return;
        
        bool isContaining = particlePool.TryGetValue(key,out var list);
        if (isContaining == false || list.Count > max_amount)
            return;

        var particle = list.Find(p => !p.gameObject.activeSelf);
        if (particle != null)
        {
            particle.transform.position = position;
            particle.gameObject.SetActive(true);
            particle.Play();
        }
        else
        {
            var complement = Instantiate(particleTable[key], this.transform);
            complement.gameObject.SetActive(false);
            particlePool[key].Add(complement);
            PlayEffect(key, position);
        }
        
    }
}
