public class DeadState : IState
{
    private PlayerStateContexter temp_controller;

    public void EnterState(PlayerStateContexter controller)
    {
        temp_controller = controller;

        InputManager.Instance.LockMoveAndAttack();
        controller.animationContexter.playDead();
        controller.popUpUIManager.PopUpDeadUI();

        controller.popUpUIManager.deadUI.OnRevived += HandleRevive;
    }

    private void HandleRevive()
    {    
        temp_controller.TransitionState(States.CombatIdle);
        temp_controller.player.statManager.set_hp(temp_controller.player.statManager.get_max_hp());
    }

    public void UpdateState(PlayerStateContexter controller) { }

    public void ExitState(PlayerStateContexter controller)
    {

        controller.popUpUIManager.deadUI.OnRevived -= HandleRevive;

        InputManager.Instance.UnlockMoveAndAttack();
        controller.animationContexter.exitDead();
    }
}