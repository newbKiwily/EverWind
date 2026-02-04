using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class InteractState : IState
{
    public static Action InteractTClear;
    private Action onObtainedAction;
    private FieldItem item;
    public void EnterState(PlayerStateContexter controller)
    {
        item = controller.player.closetItem;

        if (item != null)
        {
            // 채집 완료 이벤트를 구독합니다.
            onObtainedAction = () => OnObtained(controller);
            item.EvObtained += onObtainedAction;

           // 채집 코루틴 시작
            item.StartRooting();
            controller.animationContexter.playInteract();
            controller.offWeapon();
            var packet = PacketMethod.BuildInteractRequest(NetworkClient.Instance.userDBId,true);
            NetworkClient.Instance.Send(packet);
        }
        else
        {
            // 목표물이 없으면 바로 Idle로 돌아갑니다.
            Debug.Log("InteractState 진입했으나 상호작용 목표물이 없습니다. Idle로 복귀.");
            controller.TransitionState(States.Idle);
        }


    }
    private void OnObtained(PlayerStateContexter controller)
    {
        InteractTClear?.Invoke();


        DisplayUIManager.Instance.ChangeProfile(DisplayUIManager.ProfileState.Success, 1.0f);
        controller.animationContexter.playOneshot(OneshotAni.Success);
        controller.TransitionState(States.Idle);
        
        var packet= PacketMethod.BuildResourceObtainedRequest(item.resourceId);
        NetworkClient.Instance.Send(packet);
    }

    public void UpdateState(PlayerStateContexter controller)
    {
        if (InputManager.Instance == null)
            return;
        if (controller.player.getInputVector().sqrMagnitude > 0)
        {                    
            controller.TransitionState(States.Move);       
            return;
        }

        if(!item.gameObject.activeSelf)
        {
            controller.TransitionState(States.Move);
            return;
        }
    }

    public void ExitState(PlayerStateContexter controller)
    {



        if (item != null)
        {
            if (onObtainedAction != null)
            {
                item.EvObtained -= onObtainedAction;
                onObtainedAction = null;
            }

            if (!item.isNullCoroutine())
            {
                item.StopRooting();
            }
        }


        controller.player.closetItem = null;
        controller.animationContexter.exitInteract();

    }


}