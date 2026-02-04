using System;
using UnityEngine;
public class MoveStep : ITutorialStep
{
    private Action _deleteAction;

    public void EnterStep(TutorialGuide step)
    {
        step.moveT_taretBox.SetActive(true);
        _deleteAction += () => ClearEvent(step);
        Player.OnMoveTClear += _deleteAction;
        Debug.Log("MoveTΩ√¿€");
        step.textRenderManager.StartShow("MoveT");
        step.textRenderManager.AutoShow(0, 1);
    }
    public void UpdateStep(TutorialGuide step)
    {
        if (_deleteAction != null) return;

        if (!step.textRenderManager.isDoneShowingText()) return;

        bool keyboardInput = InputManager.Instance.AnyKeyDownExcludeMouse();

        if (keyboardInput)
        {
            step.TransitionStep(TutorialStep.Combat);
        }
    }

    public void ExitStep(TutorialGuide step)
    {
        if (_deleteAction != null)
            Player.OnMoveTClear -= _deleteAction;
        _deleteAction = null;
    }
    public void ClearEvent(TutorialGuide step)
    {
        step.textRenderManager.AutoShow(2, 3);
        if (_deleteAction != null)
            Player.OnMoveTClear -= _deleteAction;
        _deleteAction = null;

    }
}
