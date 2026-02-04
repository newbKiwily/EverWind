using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class InteractStep : ITutorialStep
{
    private Action _deleteAction;

    public void EnterStep(TutorialGuide step)
    {
        step.obtainObj1.SetActive(true);
        step.obtainObj2.SetActive(true);
        step.obtainObj3.SetActive(true);
        step.ObtainObj4.SetActive(true);
        _deleteAction += () => ClearEvent(step);
        InteractState.InteractTClear += _deleteAction;
      
        step.textRenderManager.StartShow("InteractT");
        step.textRenderManager.AutoShow(0, 2);

    }
    public void UpdateStep(TutorialGuide step)
    {
        if (_deleteAction != null) return;

        if (!step.textRenderManager.isDoneShowingText()) return;

        bool keyboardInput = InputManager.Instance.AnyKeyDownExcludeMouse();

        if (keyboardInput)
        {
            step.TransitionStep(TutorialStep.Craft);
        }
    }

    public void ExitStep(TutorialGuide step)
    {
        if (_deleteAction != null)
            InteractState.InteractTClear -= _deleteAction;
        _deleteAction = null;
    }

    public void ClearEvent(TutorialGuide step)
    {
        step.textRenderManager.AutoShow(3, 7);
        if (_deleteAction != null)
            InteractState.InteractTClear -= _deleteAction;
        _deleteAction = null;

    }
}
