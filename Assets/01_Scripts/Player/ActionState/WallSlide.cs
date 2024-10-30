using UnityEngine;

public class WallSlide : ActionState
{

    public override void Init(Player player)
    {
        base.Init(player);
        _player.playerInput.DownJump += StartAction;
        _player.playerInput.UpJump += EndAction;
    }
    public override void StartAction()
    {
        if (_player.parkouring) return;
        AnimatorStateInfo stateInfo = _player.playerAnim.anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Falling Idle")) return;
        base.StartAction();
    }
    public override void EndAction()
    {
        base.EndAction();
    }
}
