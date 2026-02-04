using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class DeadUI : MonoBehaviour
{
    public Button revive;
    public Button quit;
    public TextMeshProUGUI introductionDead;

    private float timer;
    private bool isCountingDown = false;

    public event Action OnRevived;

    

    void OnEnable()
    {
        timer = 15f;
        isCountingDown = true;

        if (revive != null)
        {
            revive.interactable = false; 
        }
    }

    void Update()
    {
        if (isCountingDown)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                introductionDead.text = $"당신은 죽었습니다.\n{Mathf.CeilToInt(timer)}초 후에 살아날 수 있습니다...";
            }
            else
            {
                FinishCountdown();
            }
        }
    }

    void FinishCountdown()
    {
        isCountingDown = false;
        timer = 0;
        introductionDead.text = "이제 부활할 수 있습니다!\n포기하지마세요.";

        if (revive != null)
        {
            revive.interactable = true;
        }
    }

    public void OnRevive()
    {

        var popUpUIManager = GetComponentInParent<PopUpUIManager>();
        if (popUpUIManager != null)
        {
            popUpUIManager.closeDeadUI();
        }

        OnRevived?.Invoke();
    }
   
}