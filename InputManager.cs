using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public TutorialGuide tutorialGuide;
    private bool isRestricted=false;
    private void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool GetInventoryKeyDown()
    {
        if (isRestricted)
            return false;
        return Input.GetKeyDown(KeyCode.I);
    }
    public bool GetCraftUIKeyDown()
    {
        if(tutorialGuide.getTutorialStepType() == TutorialStep.Camera || tutorialGuide.getTutorialStepType() == TutorialStep.Move ||
            tutorialGuide.getTutorialStepType() == TutorialStep.Combat||tutorialGuide.getTutorialStepType() == TutorialStep.Interact|| isRestricted)
            return false;

        return Input.GetKeyDown(KeyCode.C);
    }

    public bool GetUICloseKeyDown()
    {
        if (isRestricted)
            return false;
        return Input.GetKeyDown(KeyCode.Escape);
    }
    // 이동 입력
    public float GetHorizontal()
    {   

        if (tutorialGuide.getTutorialStepType() == TutorialStep.Camera||isRestricted)
            return 0;

        return Input.GetAxis("Horizontal");
    }

    public float GetVertical()
    {
        if (tutorialGuide.getTutorialStepType() == TutorialStep.Camera|| isRestricted)
            return 0;
        return Input.GetAxis("Vertical");
    }

    // 점프 입력
    public bool GetInteractDown()
    {
        if (tutorialGuide.getTutorialStepType() == TutorialStep.Camera || tutorialGuide.getTutorialStepType()==TutorialStep.Move||
            tutorialGuide.getTutorialStepType()==TutorialStep.Combat || isRestricted)
            return false;
        return Input.GetKeyDown(KeyCode.Space);
    }

    // 공격 입력
    public bool GetEnterCombatDown()
    {
        if (tutorialGuide.getTutorialStepType() == TutorialStep.Camera|| tutorialGuide.getTutorialStepType() == TutorialStep.Move || isRestricted)
            return false;
        return Input.GetMouseButtonDown(0); // 왼쪽 클릭
    }
    public bool GetControlCamera()
    {   
        return Input.GetMouseButton(1); // 왼쪽 클릭
    }
    public bool GetChangeTargetDown()
    {
        if (tutorialGuide.getTutorialStepType() == TutorialStep.Camera|| tutorialGuide.getTutorialStepType() == TutorialStep.Move || isRestricted)
            return false;
        return Input.GetKeyDown(KeyCode.E);
    }

    // "아무 키 입력" (마우스 제외)
    public bool AnyKeyDownExcludeMouse()
    {
        if (Input.anyKeyDown)
        {
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                return true;
        }
        return false;
    }
    public int GetAttackKeyDown()
    {
        if (tutorialGuide.getTutorialStepType() == TutorialStep.Camera|| tutorialGuide.getTutorialStepType() == TutorialStep.Move || isRestricted)
            return 0;
        for (int i = 1; i <= 5; i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + i);
            if (Input.GetKeyDown(key))
                return i; // 1~5 반환
        }
        return 0; // 입력 없음
    }

    public void LockMoveAndAttack()
    {
        isRestricted= true;
    }

    public void UnlockMoveAndAttack()
    {
        isRestricted = false;
    }

    public bool IsLocked()
    {
        return isRestricted;
    }
}
