using System;
using UnityEngine;
using System.Collections;


public interface IObtainable
{
    event Action EvObtained;

    void StartRooting();

    void StopRooting();

    bool isNullCoroutine();
}
