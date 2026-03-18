using Unity.Netcode;
using UnityEngine;

public class MultiButtonManager : NetworkBehaviour
{
    public GameObject hiddenBlock;
    public int totalButtons = 4;

    private int currentPressed = 0;

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
        if (currentPressed >= totalButtons)
        {
            SetBlockClientRpc(true);
        }
        else
        {
            SetBlockClientRpc(false);
        }
    }

    [ClientRpc]
    void SetBlockClientRpc(bool state)
    {
        hiddenBlock.SetActive(state);
    }
}