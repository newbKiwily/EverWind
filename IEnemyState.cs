using UnityEngine;
using System;
public interface IEnemyState
{
    void EnterState(EnemyStateContexter controller);
    void UpdateState(EnemyStateContexter controller);

    void ExitState(EnemyStateContexter controller);


}