using UnityEngine;

public class WallRun : Parkour
{
    public override bool ActionCondition(Transform player, float speed, ref Parkour parkour)
    {
        return true;
    }
    public override void EndAnim()
    {
        base.EndAnim();
        _player.playerMovement.grounded = true;
    }
}
