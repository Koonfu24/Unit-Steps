using Unity.Netcode;
using UnityEngine;

public class SingleButton : NetworkBehaviour
{
    public MultiButtonManager manager;
    private bool isPressed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !isPressed)
        {
            isPressed = true;
            manager.PressButton();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && isPressed)
        {
            isPressed = false;
            manager.ReleaseButton();
        }
    }
}
