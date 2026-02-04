
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
public enum TutorialStep
{
    Camera,
    Move,
    Combat,
    Interact,
    Craft,
    Equip
}

public class TutorialGuide : MonoBehaviour
{
    private ITutorialStep currentStep;
    private TutorialStep currentStepType;
    public TextRenderManager textRenderManager;
    private Dictionary<TutorialStep, ITutorialStep> stateTable = new Dictionary<TutorialStep, ITutorialStep>();

    public GameObject moveT_taretBox;
    public GameObject dummy_enemy,dummy_enemy2;
    public GameObject obtainObj1, obtainObj2,obtainObj3,ObtainObj4;

    public void TransitionStep(TutorialStep targetState)
    {
        if (currentStep != null)
            currentStep.ExitStep(this);
        currentStepType = targetState;
        currentStep = stateTable[targetState];
        currentStep.EnterStep(this);
    }

    private void Start()
    {
        textRenderManager = TextRenderManager.Instance; 
        dummy_enemy.SetActive(false);
        dummy_enemy2.SetActive(false);
        stateTable.Add(TutorialStep.Camera, new CameraStep());
        stateTable.Add(TutorialStep.Move, new MoveStep());
        stateTable.Add(TutorialStep.Combat,new CombatStep());
        stateTable.Add(TutorialStep.Interact, new InteractStep());
        stateTable.Add(TutorialStep.Craft, new CraftStep());
        stateTable.Add(TutorialStep.Equip, new EquipStep());
        TransitionStep(TutorialStep.Combat);
    }

    private void Update()
    {
        if (currentStep != null)
        {
            currentStep.UpdateStep(this);
        }


    }
    public TutorialStep getTutorialStepType()
    {
        return currentStepType;
    }
    
}