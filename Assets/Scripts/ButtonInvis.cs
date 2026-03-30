using Unity.Netcode;
using UnityEngine;

public class ButtonInvis : NetworkBehaviour
{
    public GameObject hiddenBlock;

    private bool isActivated = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (isActivated) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isActivated = true;

            // 🔥 เปิดบล็อก
            ShowBlockClientRpc(true);

            // 🔥 ลบปุ่ม (ทั้ง server + client)
            DestroyButtonClientRpc();
        }
    }

    [ClientRpc]
    void ShowBlockClientRpc(bool state)
    {
        hiddenBlock.SetActive(state);
    }

    [ClientRpc]
    void DestroyButtonClientRpc()
    {
        Destroy(gameObject);
    }
}