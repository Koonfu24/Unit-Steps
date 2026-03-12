using Unity.Netcode;
using UnityEngine;

public class ButtonInvis : NetworkBehaviour
{
    public GameObject hiddenBlock;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ShowBlockClientRpc(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ShowBlockClientRpc(false);
        }
    }

    [ClientRpc]
    void ShowBlockClientRpc(bool state)
    {
        hiddenBlock.SetActive(state);
    }
}