using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 200f;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.angularVelocity = speed;
    }
}
