using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerColor : NetworkBehaviour
{
    public SpriteRenderer sprite;
    public TextMeshProUGUI nameText;

    private NetworkVariable<int> playerID = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerID.Value = OwnerClientId < 4 ? (int)OwnerClientId : (int)(OwnerClientId % 4);
        }

        UpdateVisual(playerID.Value);

        playerID.OnValueChanged += (oldVal, newVal) =>
        {
            UpdateVisual(newVal);
        };
    }

    void UpdateVisual(int id)
    {
        Color[] colors = { Color.blue, Color.red, Color.green, Color.yellow };

        id = id % colors.Length;

        sprite.color = colors[id];
        nameText.text = "P" + (id + 1);
        nameText.color = colors[id];
    }
}
