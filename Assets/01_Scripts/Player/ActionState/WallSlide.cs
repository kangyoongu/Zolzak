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

        //base.StartAction();
    }
    public override void EndAction()
    {
        //base.EndAction();
    }
}
