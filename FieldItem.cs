using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class FieldItem : MonoBehaviour, IObtainable
{
    protected string id;
    public int resourceId;
    protected float obtainTime;
    protected bool isRespawnsible;
    public event Action EvObtained;
    protected IEnumerator obtainingCoroutine;

    protected abstract void Start();

    protected abstract IEnumerator Obtaining();
    public virtual void StartRooting()
    {
        obtainingCoroutine = Obtaining();
        StartCoroutine(obtainingCoroutine);
    }

    public virtual bool isNullCoroutine()
    {
        if (obtainingCoroutine == null)
            return true;
        else
            return false;
    }

    public virtual void StopRooting()
    {
        StopCoroutine(obtainingCoroutine);
        obtainingCoroutine = null;

    }

    protected void EndRooting()
    {
        if (EvObtained != null)
        {
            EvObtained();
        }
        obtainingCoroutine = null;
    }

    public virtual void initialize()
    { }


}