using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MultiPlayerFinish : NetworkBehaviour
{
    public int requiredPlayers = 1;

    private HashSet<ulong> players = new HashSet<ulong>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var netObj = other.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                players.Add(netObj.OwnerClientId);
                CheckPlayers();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var netObj = other.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                players.Remove(netObj.OwnerClientId);
            }
        }
    }

    void CheckPlayers()
    {
        if (players.Count >= requiredPlayers)
        {
            ChangeSceneClientRpc();
        }
    }

    [ClientRpc]
    void ChangeSceneClientRpc()
    {
        SceneManager.LoadScene("Win");
    }
}
