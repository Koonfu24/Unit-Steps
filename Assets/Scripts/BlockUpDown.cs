using Unity.Netcode;
using UnityEngine;

public class BlockUpDown : NetworkBehaviour
{
    public float moveDistance = 3f;
    public float speed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool goUp = true;

    public override void OnNetworkSpawn()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveDistance;
    }

    void Update()
    {
        if (!IsServer) return;

        if (goUp)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                goUp = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, startPos) < 0.01f)
                goUp = true;
        }
    }
}
