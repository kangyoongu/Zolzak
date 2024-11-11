using UnityEngine;

public class TopOut : Parkour
{
    public override bool ActionCondition(Transform player, float speed, ref Parkour parkour)
    {
        return true;
    }
}
