using UnityEngine;
using System;

public class CameraStep : ITutorialStep
{
    private Action _deleteAction;

    public void EnterStep(TutorialGuide step)
    {
        _deleteAction += () => ClearEvent(step);
        CameraMoving.OnCameraTClear += _deleteAction;
        
        step.textRenderManager.StartShow("CameraT");
        step.textRenderManager.AutoShow(0, 1);
        
    }
    public void UpdateStep(TutorialGuide step)
    {
        if (_deleteAction != null) return; 

        if (!step.textRenderManager.isDoneShowingText()) return;

        bool keyboardInput = InputManager.Instance.AnyKeyDownExcludeMouse();

        if (keyboardInput)
        {
            step.TransitionStep(TutorialStep.Move);
        }
    }

    public void ExitStep(TutorialGuide step)
    {
        if (_deleteAction != null)
            CameraMoving.OnCameraTClear -= _deleteAction;
        _deleteAction = null;
    }

    public void ClearEvent(TutorialGuide step)
    {
        step.textRenderManager.AutoShow(2, 3);
        if (_deleteAction != null)
            CameraMoving.OnCameraTClear -= _deleteAction;
        _deleteAction = null;

    }
}
