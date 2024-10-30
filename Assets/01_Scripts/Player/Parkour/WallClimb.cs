using System.Collections.Generic;
using UnityEngine;

public class WallClimb : Parkour
{
    [SerializeField] float _height;
    [SerializeField] float _maxHeight;
    [SerializeField] float _dis;
    [SerializeField] float _width;
    [SerializeField] List<Parkour> failureActions;
    public override void Init(Player player)
    {
        foreach(Parkour action in failureActions)
            action.Init(player);
        base.Init(player);
    }
    public override bool ActionCondition(Transform player, ref Parkour parkour)
    {
        if (Physics.Raycast(player.position + new Vector3(0f, 0.4f, 0f), player.forward, out RaycastHit hit, _dis, castLayer))//장애물과 거리 체크
        {
            Vector3 hitPos = hit.point;
            Vector3 playerDir = player.forward;
            Vector3 origin = hitPos + (playerDir * 0.05f);

            if (!Physics.Raycast(new Ray(hitPos + (playerDir * -0.05f), Vector3.up), _height + 2f, castLayer))
            {
                if (Physics.Raycast(new Ray(origin + (Vector3.up * _height), Vector3.down), out RaycastHit hit2, _height, castLayer))//장애물 높이 체크
                {
                    origin = hit2.point + playerDir * _width + Vector3.up * 0.1f;
                    if (Physics.SphereCast(new Ray(origin, -playerDir), 0.5f, out RaycastHit hit3, _width, castLayer))// 장애물 두께 체크
                    {
                        originPos.Add(hit2.point);
                        originPos.Add(hit3.point);
                        parkour = this;
                        return true;
                    }
                    else
                    {
                        failureActions[0].originPos.Add(hit2.point);
                        failureActions[0].originPos.Add(hit2.point + (playerDir * 0.01f));
                        parkour = failureActions[0];
                        return true;
                    }
                }
                else
                {
                    if (!Physics.Raycast(new Ray(origin, Vector3.up), _maxHeight + 2f, castLayer))
                    {
                        if (Physics.Raycast(new Ray(origin + (Vector3.up * _maxHeight), Vector3.down), out RaycastHit hit4, _maxHeight, castLayer))//장애물 높이 체크
                        {
                            failureActions[1].originPos.Add(Vector3.Lerp(hitPos, hit.point, 0.43f));
                            failureActions[1].originPos.Add(hit4.point);
                            failureActions[1].originPos.Add(hit4.point + (playerDir * 0.01f));
                            parkour = failureActions[1];
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
