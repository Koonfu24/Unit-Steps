using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class MultiButton : NetworkBehaviour
{
    public GameObject hiddenBlock;
    public int requiredPlayers = 4;

    private HashSet<ulong> playersOnButton = new HashSet<ulong>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var netObj = collision.gameObject.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                playersOnButton.Add(netObj.OwnerClientId);
                CheckPlayers();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var netObj = collision.gameObject.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                playersOnButton.Remove(netObj.OwnerClientId);
                CheckPlayers();
            }
        }
    }

    void CheckPlayers()
    {
        if (playersOnButton.Count >= requiredPlayers)
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
