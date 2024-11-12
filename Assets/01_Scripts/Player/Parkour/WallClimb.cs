using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallClimb : Parkour
{
    [SerializeField] List<Parkour> failureActions;
    [Header("Params")]
    [Tooltip("�տ� ��ֹ� �ִ��� Ray ��µ� �� ����")]
    [SerializeField] float _obsCheckHeight = 0.5f;
    [Tooltip("�� �Ÿ�")]
    [SerializeField] float _dis;
    [Tooltip("���� ��ֹ� �Ѿ�� �������� �˻��� ����")]
    [SerializeField] float _height;
    [Tooltip("���� ��ֹ�(�� �ɾ �ö󰡴°�) �˻� ����")]
    [SerializeField] float _maxHeight;
    [Tooltip("Kong Vault�� ����� ��ֹ� �β�")]
    [SerializeField] float _width;
    [Tooltip("wallClimb�� �� �Ÿ�")]
    [SerializeField] float _freeDis;
    [Tooltip("wallClimb�� �� ����")]
    [SerializeField] float _freeHeight;

    public override void Init(Player player)
    {
        foreach(Parkour action in failureActions)
            action.Init(player);
        base.Init(player);
    }

    public override bool ActionCondition(Transform player, float speed, ref Parkour parkour)
    {
        if (Physics.Raycast(player.position + new Vector3(0f, _obsCheckHeight, 0f), player.forward, out RaycastHit hit, _dis, castLayer))//��ֹ��� �Ÿ� üũ
        {
            Vector3 hitPos = hit.point;
            Vector3 playerDir = player.forward;
            Vector3 origin = hitPos + (playerDir * 0.05f);
            Vector3 backOrigin = hitPos + (playerDir * -0.05f);

            if (!Physics.Raycast(new Ray(backOrigin, Vector3.up), _height + 2f, castLayer))//���� ���� ���
            {
                if (Physics.Raycast(new Ray(origin + (Vector3.up * _height), Vector3.down), out RaycastHit hit2, _height, castLayer) && _player.playerMovement.grounded)//��ֹ� ���� üũ
                {
                    origin = hit2.point + playerDir * _width + Vector3.up * 0.1f;
                    if (Physics.SphereCast(new Ray(origin, -playerDir), 0.5f, out RaycastHit hit3, _width, castLayer))// ��ֹ� �β� üũ
                    {
                        originPos.Add(hit2.point);//�ẼƮ
                        originPos.Add(hit3.point);
                        parkour = this;
                        return true;
                    }
                    else//�β� ������
                    {
                        failureActions[0].originPos.Add(hit2.point);//�׳� ������
                        failureActions[0].originPos.Add(hit2.point + (playerDir * 0.01f));
                        parkour = failureActions[0];
                        return true;
                    }
                }
                else//Height���� ������
                {
                    if (!_player.playerMovement.grounded)//�����̸�
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

                    if (Physics.Raycast(new Ray(origin + (Vector3.up * _maxHeight), Vector3.down), out RaycastHit hit4, _maxHeight, castLayer))//��ֹ� ���� üũ
                    {
                        failureActions[1].originPos.Add(Vector3.Lerp(hitPos, hit.point, 0.43f));//��Ÿ��
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
