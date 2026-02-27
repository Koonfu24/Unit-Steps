using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI timerText;
    public Button startButton;
    public GameObject canvasObject;

    [Header("Settings")]
    public int maxPlayers = 4;

    private NetworkVariable<float> networkTimer =
        new NetworkVariable<float>(0f);

    private NetworkVariable<bool> gameStarted =
        new NetworkVariable<bool>(false);

    void Update()
    {
        UpdatePlayerCount();
        UpdateTimerUI();

        if (IsServer)
        {
            // เปิดปุ่มเมื่อครบจำนวน
            startButton.interactable =
                NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers;

            // เดินเวลาเมื่อเกมเริ่ม
            if (gameStarted.Value)
            {
                networkTimer.Value += Time.deltaTime;
            }
        }

        // Client ห้ามเห็นปุ่ม
        if (!IsServer)
        {
            startButton.gameObject.SetActive(false);
        }
    }

    void UpdatePlayerCount()
    {
        int current = NetworkManager.Singleton.ConnectedClients.Count;
        playerCountText.text = current + " / " + maxPlayers;
    }

    void UpdateTimerUI()
    {
        float time = networkTimer.Value;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = minutes.ToString("00") + ":" +
                         seconds.ToString("00");
    }

    public void StartGame()
    {
        if (!IsServer) return;

        gameStarted.Value = true;

        DestroyWalls(); // ทำลายกำแพง

        StartGameClientRpc(); // ซ่อน Canvas ทุกเครื่อง
    }

    void DestroyWalls()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("Wall"))
            {
                if (obj.TryGetComponent<NetworkObject>(out NetworkObject netObj))
                {
                    netObj.Despawn(true); // ทำลายแบบ Network
                }
                else
                {
                    Destroy(obj);
                }
            }
        }
    }

    [ClientRpc]
    void StartGameClientRpc()
    {
        canvasObject.SetActive(false);
    }
}
