using UnityEngine;
using Unity.Netcode;

public class Die : NetworkBehaviour
{
    private static Vector3 spawnPosition;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dieSound;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            spawnPosition = GameObject.Find("SpawnPoint").transform.position;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Die"))
        {
            // เรียกเสียง (ส่งไปทุก client)
            PlayDieSoundServerRpc();

            RespawnAllPlayersServerRpc();
        }
    }

    // 🔊 ส่งคำสั่งให้ทุก client เล่นเสียง
    [ServerRpc(RequireOwnership = false)]
    void PlayDieSoundServerRpc()
    {
        PlayDieSoundClientRpc();
    }

    [ClientRpc]
    void PlayDieSoundClientRpc()
    {
        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RespawnAllPlayersServerRpc()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                RespawnSpecificPlayerClientRpc(client.PlayerObject.NetworkObjectId);
            }
        }
    }

    [ClientRpc]
    void RespawnSpecificPlayerClientRpc(ulong networkObjectId)
    {
        NetworkObject obj;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects
            .TryGetValue(networkObjectId, out obj))
        {
            var rb = obj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.position = spawnPosition;
            }
        }
    }
}