using UnityEngine;
using System;
public interface IState
{
    void EnterState(PlayerStateContexter contexter);
    void UpdateState(PlayerStateContexter contexter);

    void ExitState(PlayerStateContexter contexter);

    
}
