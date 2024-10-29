using UnityEngine;

public class WallRun : Parkour
{
    public override bool ActionCondition(Transform player, ref Parkour parkour)
    {
        return true;
    }
}
