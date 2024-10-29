using UnityEngine;

public class TopOut : Parkour
{
    public override bool ActionCondition(Transform player, ref Parkour parkour)
    {
        return true;
    }
}
