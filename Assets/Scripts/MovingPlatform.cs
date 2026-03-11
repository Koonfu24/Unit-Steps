using Unity.Netcode;
using UnityEngine;

public class MovingPlatform : NetworkBehaviour
{
    public float moveDistance = 3f;
    public float speed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool goRight = true;

    public override void OnNetworkSpawn()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.right * moveDistance;
    }

    void Update()
    {
        if (!IsServer) return;

        if (goRight)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                goRight = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPos,
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, startPos) < 0.01f)
            {
                goRight = true;
            }
        }
    }
}