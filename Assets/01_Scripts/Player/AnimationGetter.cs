using UnityEngine;
using UnityEngine.Events;

public class AnimationGetter : MonoBehaviour
{
    public Player player;
    public UnityEvent ClimbStart;
    public void SetPos()
    {
        player.playerParkour.SetPos();
    }
    public void ParkourEnd()
    {
        player.playerParkour.EndAnim();
    }
    public void StartAction(string name)
    {
        player.playerParkour.StartAction(name);
    }
    public void EndAction(string name)
    {
        player.playerParkour.EndAction(name);
    }
    public void ClimbEvent()
    {
        ClimbStart?.Invoke();
    }
}
