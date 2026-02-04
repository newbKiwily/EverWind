using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CombatStep : ITutorialStep
{
    private bool _cleared = false;        
    private bool _readyForNext = false;   

    public void EnterStep(TutorialGuide step)
    {
        step.dummy_enemy.SetActive(true);
        step.dummy_enemy2.SetActive(true);

        step.textRenderManager.StartShow("CombatT");
        step.textRenderManager.AutoShow(0, 5);
    }

    public void UpdateStep(TutorialGuide step)
    {
    
        if (_cleared && !_readyForNext) return;

  
        bool enemy1Dead = step.dummy_enemy == null || !step.dummy_enemy.activeInHierarchy;
        bool enemy2Dead = step.dummy_enemy2 == null || !step.dummy_enemy2.activeInHierarchy;

        if (!_cleared && enemy1Dead && enemy2Dead)
        {
            ClearEvent(step);  // 텍스트 출력
            return;
        }

        if (_readyForNext && step.textRenderManager.isDoneShowingText())
        {
            if (InputManager.Instance.AnyKeyDownExcludeMouse())
            {
                step.TransitionStep(TutorialStep.Interact);
                _readyForNext = false;
            }
        }
    }

    public void ExitStep(TutorialGuide step)
    {
        _cleared = false;
        _readyForNext = false;
        step.dummy_enemy=null;
        step.dummy_enemy2=null;
    }

    public void ClearEvent(TutorialGuide step)
    {
        if (_cleared) return;
        _cleared = true;


        step.textRenderManager.AutoShow(6, 7);

        _readyForNext = true;
    }
}
