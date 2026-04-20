using Unity.Netcode;
using UnityEngine;

public class ButtonInvis : NetworkBehaviour
{
    public GameObject hiddenBlock;

    private int playerCount = 0;

    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(false);

    void Start()
    {
        // ตั้งค่าตอนเริ่ม
        UpdateBlock(isActive.Value);

        // ฟังตอนค่าถูกเปลี่ยน
        isActive.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateBlock(newValue);
        };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerCount++;

            if (playerCount > 0)
                isActive.Value = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerCount--;

            if (playerCount <= 0)
            {
                playerCount = 0;
                isActive.Value = false;
            }
        }
    }

    void UpdateBlock(bool state)
    {
        if (hiddenBlock != null)
            hiddenBlock.SetActive(state);
    }
}