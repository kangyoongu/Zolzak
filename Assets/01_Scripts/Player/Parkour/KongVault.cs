using UnityEngine;

public class KongVault : Parkour
{
    [SerializeField] float _height;
    [SerializeField] float _dis;
    [SerializeField] float _width;
    public override bool ActionCondition(Transform player)
    {
        if (Physics.Raycast(player.position + new Vector3(0f, 0.4f, 0f), player.forward, out RaycastHit hit, _dis, castLayer))
        {
            Vector3 hitPos = hit.point;
            Vector3 playerDir = player.forward;
            Vector3 origin = hitPos + (playerDir * 0.05f) + (Vector3.up * _height);

            if (Physics.Raycast(new Ray(origin, Vector3.down), out RaycastHit hit2, _height, castLayer))
            {
                origin = hit2.point + playerDir * _width + Vector3.up * 0.1f;
                if (Physics.SphereCast(new Ray(origin, -playerDir), 0.5f, out RaycastHit hit3, _width, castLayer))
                {
                    originPos.Add(hit2.point);
                    originPos.Add(hit3.point);
                    return true;
                }
            }
        }
        return false;
    }
}
