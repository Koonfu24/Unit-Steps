using Unity.Netcode;
using UnityEngine;

public class MultiButtonManager : NetworkBehaviour
{
    public GameObject hiddenBlock;
    public int totalButtons = 4;

    private int currentPressed = 0;
    private bool isDestroyed = false;

    public void PressButton()
    {
        if (!IsServer) return;

        currentPressed++;
        CheckButtons();
    }

    public void ReleaseButton()
    {
        if (!IsServer) return;

        currentPressed--;
        CheckButtons();
    }

    void CheckButtons()
    {
        if (currentPressed >= totalButtons && !isDestroyed)
        {
            DestroyBlock();
        }
    }

    void DestroyBlock()
    {
        isDestroyed = true;

        if (hiddenBlock.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            netObj.Despawn(true); 
        }
        else
        {
            Destroy(hiddenBlock); 
        }
    }
}