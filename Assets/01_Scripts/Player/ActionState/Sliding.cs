using UnityEngine;

public class Sliding : ActionState
{
    bool _sliding = false;
    public override void Init(Player player)
    {
        base.Init(player);
        _player.playerInput.DownCtrl += StartAction;
        _player.playerInput.UpCtrl += EndAction;
    }
    private void Update()
    {
        if (_sliding)
        {
            if (transform.InverseTransformDirection(_player.Rigidbody.linearVelocity).z < 5f)
                EndAction();
        }
    }
    public override void StartAction()
    {
        AnimatorStateInfo stateInfo = _player.playerAnim.anim.GetCurrentAnimatorStateInfo(0);
        if (_sliding || _player.parkouring || transform.InverseTransformDirection(_player.Rigidbody.linearVelocity).z < 5f || !stateInfo.IsName("Move")) return;

        Vector3 velocity = _player.Rigidbody.linearVelocity;
        base.StartAction();
        _sliding = true;
        _player.Rigidbody.isKinematic = false;
        _player.Rigidbody.linearVelocity = velocity * 1.5f;
        _player.playerAnim.anim.SetBool("Slide", true);
        _player.playerMovement.LockY(true);
    }
    public override void EndAction()
    {
        if (!_sliding) return;

        base.EndAction();
        _sliding = false;
        _player.playerAnim.anim.SetBool("Slide", false);
        _player.playerMovement.UnlockY(true);
    }
}
