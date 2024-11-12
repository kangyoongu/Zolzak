using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallClimb : Parkour
{
    [SerializeField] List<Parkour> failureActions;
    [Header("Params")]
    [Tooltip("앞에 장애물 있는지 Ray 쏘는데 쏠 높이")]
    [SerializeField] float _obsCheckHeight = 0.5f;
    [Tooltip("쏠 거리")]
    [SerializeField] float _dis;
    [Tooltip("낮은 장애물 넘어가기 나오는지 검사할 높이")]
    [SerializeField] float _height;
    [Tooltip("높은 장애물(벽 걸어서 올라가는거) 검사 높이")]
    [SerializeField] float _maxHeight;
    [Tooltip("Kong Vault를 재생할 장애물 두께")]
    [SerializeField] float _width;
    [Tooltip("wallClimb할 때 거리")]
    [SerializeField] float _freeDis;
    [Tooltip("wallClimb할 때 높이")]
    [SerializeField] float _freeHeight;

    public override void Init(Player player)
    {
        foreach(Parkour action in failureActions)
            action.Init(player);
        base.Init(player);
    }

    public override bool ActionCondition(Transform player, float speed, ref Parkour parkour)
    {
        if (Physics.Raycast(player.position + new Vector3(0f, _obsCheckHeight, 0f), player.forward, out RaycastHit hit, _dis, castLayer))//장애물과 거리 체크
        {
            Vector3 hitPos = hit.point;
            Vector3 playerDir = player.forward;
            Vector3 origin = hitPos + (playerDir * 0.05f);
            Vector3 backOrigin = hitPos + (playerDir * -0.05f);

            if (!Physics.Raycast(new Ray(backOrigin, Vector3.up), _height + 2f, castLayer))//위로 레이 쏘기
            {
                if (Physics.Raycast(new Ray(origin + (Vector3.up * _height), Vector3.down), out RaycastHit hit2, _height, castLayer) && _player.playerMovement.grounded)//장애물 높이 체크
                {
                    origin = hit2.point + playerDir * _width + Vector3.up * 0.1f;
                    if (Physics.SphereCast(new Ray(origin, -playerDir), 0.5f, out RaycastHit hit3, _width, castLayer))// 장애물 두께 체크
                    {
                        originPos.Add(hit2.point);//콩볼트
                        originPos.Add(hit3.point);
                        parkour = this;
                        return true;
                    }
                    else//두께 넓으면
                    {
                        failureActions[0].originPos.Add(hit2.point);//그냥 오르기
                        failureActions[0].originPos.Add(hit2.point + (playerDir * 0.01f));
                        parkour = failureActions[0];
                        return true;
                    }
                }
                else//Height보다 높으면
                {
                    if (!_player.playerMovement.grounded)//공중이면
                    {
                        if (hit.distance > _freeDis) return false;
                        if (Physics.Raycast(new Ray(origin + (Vector3.up * _freeHeight), Vector3.down), out RaycastHit hit5, _freeHeight, castLayer))
                        {
                            failureActions[2].originPos.Add(hit.point);
                            failureActions[2].originPos.Add(new Vector3(hit.point.x, hit5.point.y, hit.point.z));
                            failureActions[2].originPos.Add(hit5.point);
                            parkour = failureActions[2];
                            return true;
                        }
                        return false;
                    }

                    if (Physics.Raycast(new Ray(backOrigin, Vector3.up), _maxHeight + 2f, castLayer)) return false;

                    if (Physics.Raycast(new Ray(origin + (Vector3.up * _maxHeight), Vector3.down), out RaycastHit hit4, _maxHeight, castLayer))//장애물 높이 체크
                    {
                        failureActions[1].originPos.Add(Vector3.Lerp(hitPos, hit.point, 0.43f));//벽타기
                        failureActions[1].originPos.Add(hit4.point);
                        failureActions[1].originPos.Add(hit4.point);
                        parkour = failureActions[1];
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
