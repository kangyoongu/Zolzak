using UnityEngine;

public class AnimationGetter : MonoBehaviour
{
    public Player player;
    public void SetPos()
    {
        player.playerParkour.SetPos();
    }
    public void ParkourEnd()
    {
        player.playerParkour.EndAnim();
    }
}
